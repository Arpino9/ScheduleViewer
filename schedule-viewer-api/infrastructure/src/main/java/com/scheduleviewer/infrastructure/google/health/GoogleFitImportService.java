package com.scheduleviewer.infrastructure.google.health;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.node.ObjectNode;
import com.google.api.client.auth.oauth2.Credential;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.io.IOException;
import java.io.InputStream;
import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.nio.charset.StandardCharsets;
import java.security.MessageDigest;
import java.time.Instant;
import java.time.ZoneId;
import java.util.ArrayList;
import java.util.Comparator;
import java.util.HexFormat;
import java.util.List;
import java.util.Locale;

/** Imports supported Google Fit Takeout JSON records into Google Health API v4. */
@Service
public class GoogleFitImportService {

    private static final String BASE = "https://health.googleapis.com/v4/users/me/dataTypes/";
    private static final int MAX_POINTS_PER_FILE = 10_000;

    private final GoogleHealthAuthService authService;
    private final ObjectMapper mapper;
    private final HttpClient httpClient;

    @Autowired
    public GoogleFitImportService(GoogleHealthAuthService authService) {
        this(authService, new ObjectMapper(), HttpClient.newHttpClient());
    }

    GoogleFitImportService(GoogleHealthAuthService authService, ObjectMapper mapper, HttpClient httpClient) {
        this.authService = authService;
        this.mapper = mapper;
        this.httpClient = httpClient;
    }

    public ImportResult importJson(String fileName, InputStream input) throws Exception {
        Credential credential = authService.getImportCredential();
        if (credential == null || credential.getAccessToken() == null) {
            throw new ImportAuthorizationException("Google Health のインポート認証が必要です。");
        }

        List<HealthPoint> points = parse(fileName, input);
        int imported = 0;
        int duplicates = 0;
        List<String> errors = new ArrayList<>();
        for (HealthPoint point : points) {
            try {
                int status = create(point, credential, true);
                if (status == 409) duplicates++;
                else imported++;
            } catch (Exception e) {
                errors.add(e.getMessage() == null ? e.getClass().getSimpleName() : e.getMessage());
            }
        }
        return new ImportResult(fileName, points.size(), imported, duplicates, errors);
    }

    List<HealthPoint> parse(String fileName, InputStream input) throws IOException {
        JsonNode root = mapper.readTree(input);
        if (root == null || !root.isObject()) {
            throw new UnsupportedFormatException("JSON オブジェクトではありません。");
        }
        if (root.hasNonNull("fitnessActivity") && root.hasNonNull("startTime")) {
            return List.of(parseExercise(root));
        }

        JsonNode dataPoints = root.path("Data Points");
        if (!dataPoints.isArray()) {
            throw new UnsupportedFormatException("Google Fit Takeout の JSON 形式を認識できません。");
        }
        if (dataPoints.size() > MAX_POINTS_PER_FILE) {
            throw new UnsupportedFormatException("1ファイルの上限 " + MAX_POINTS_PER_FILE + " 件を超えています。");
        }

        List<HealthPoint> result = new ArrayList<>();
        List<JsonNode> sleepPoints = new ArrayList<>();
        for (int i = 0; i < dataPoints.size(); i++) {
            JsonNode point = dataPoints.get(i);
            String type = point.path("dataTypeName").asText();
            switch (type) {
                case "com.google.weight" -> result.add(parseWeight(point));
                case "com.google.sleep.segment" -> sleepPoints.add(point);
                default -> { }
            }
        }
        result.addAll(parseSleepSessions(sleepPoints));
        if (result.isEmpty()) {
            throw new UnsupportedFormatException(
                    "この JSON のデータ型は Google Health API で新規作成できません。対応: 運動セッション、睡眠、体重");
        }
        return result;
    }

    private HealthPoint parseExercise(JsonNode root) {
        String start = Instant.parse(root.path("startTime").asText()).toString();
        String end = Instant.parse(root.path("endTime").asText()).toString();
        String activity = root.path("fitnessActivity").asText("other");

        ObjectNode exercise = mapper.createObjectNode();
        exercise.set("interval", interval(start, end));
        exercise.put("exerciseType", exerciseType(activity));
        exercise.put("displayName", activity.replace('_', ' '));
        String duration = normalizeDuration(root.path("duration").asText(""), start, end);
        exercise.put("activeDuration", duration);

        ObjectNode metrics = exercise.putObject("metricsSummary");
        for (JsonNode aggregate : root.path("aggregate")) {
            String metric = aggregate.path("metricName").asText();
            double value = numericValue(aggregate);
            switch (metric) {
                case "com.google.calories.expended" -> metrics.put("caloriesKcal", value);
                case "com.google.distance.delta" -> metrics.put("distanceMillimeters", value * 1_000d);
                case "com.google.step_count.delta" -> metrics.put("steps", Long.toString(Math.round(value)));
                case "com.google.speed.summary" -> metrics.put("averageSpeedMillimetersPerSecond", value * 1_000d);
                default -> { }
            }
        }

        ObjectNode body = baseBody();
        body.set("exercise", exercise);
        return new HealthPoint("exercise", stableName("exercise", start, end, activity), body);
    }

    private HealthPoint parseWeight(JsonNode point) {
        Instant sample = instantFromNanos(point.path("endTimeNanos").asLong());
        double weightGrams = numericValue(point) * 1_000d;
        ObjectNode weight = mapper.createObjectNode();
        weight.set("sampleTime", sampleTime(sample));
        weight.put("weightGrams", weightGrams);
        weight.put("notes", "Google Fit Takeout import");

        ObjectNode body = baseBody();
        body.set("weight", weight);
        return new HealthPoint("weight", stableName("weight", sample.toString(), Double.toString(weightGrams)), body);
    }

    private List<HealthPoint> parseSleepSessions(List<JsonNode> points) {
        if (points.isEmpty()) return List.of();
        List<SleepStage> stages = points.stream()
                .map(point -> new SleepStage(
                        instantFromNanos(point.path("startTimeNanos").asLong()),
                        instantFromNanos(point.path("endTimeNanos").asLong()),
                        sleepStage((int) Math.round(numericValue(point)))))
                .filter(stage -> stage.end().isAfter(stage.start()))
                .sorted(Comparator.comparing(SleepStage::start))
                .toList();

        List<HealthPoint> result = new ArrayList<>();
        List<SleepStage> session = new ArrayList<>();
        for (SleepStage stage : stages) {
            if (!session.isEmpty()) {
                SleepStage previous = session.getLast();
                boolean contiguous = previous.end().equals(stage.start());
                boolean sameModel = isDetailedStage(previous.type()) == isDetailedStage(stage.type());
                if (!contiguous || !sameModel) {
                    result.add(buildSleepSession(session));
                    session = new ArrayList<>();
                }
            }
            session.add(stage);
        }
        if (!session.isEmpty()) result.add(buildSleepSession(session));
        return result;
    }

    private HealthPoint buildSleepSession(List<SleepStage> stages) {
        Instant start = stages.getFirst().start();
        Instant end = stages.getLast().end();
        ObjectNode sleep = mapper.createObjectNode();
        sleep.set("interval", interval(start.toString(), end.toString()));
        sleep.put("type", isDetailedStage(stages.getFirst().type()) ? "STAGES" : "CLASSIC");
        var stageArray = sleep.putArray("stages");
        for (SleepStage stage : stages) {
            ObjectNode stageNode = stageArray.addObject();
            stageNode.put("startTime", stage.start().toString());
            stageNode.put("startUtcOffset", utcOffset(stage.start()));
            stageNode.put("endTime", stage.end().toString());
            stageNode.put("endUtcOffset", utcOffset(stage.end()));
            stageNode.put("type", stage.type());
        }

        ObjectNode body = baseBody();
        body.set("sleep", sleep);
        return new HealthPoint("sleep", stableName("sleep", start.toString(), end.toString()), body);
    }

    private int create(HealthPoint point, Credential credential, boolean allowRefresh) throws Exception {
        HttpRequest request = HttpRequest.newBuilder(URI.create(BASE + point.dataType() + "/dataPoints"))
                .header("Authorization", "Bearer " + credential.getAccessToken())
                .header("Content-Type", "application/json")
                .POST(HttpRequest.BodyPublishers.ofString(mapper.writeValueAsString(point.body())))
                .build();
        HttpResponse<String> response = httpClient.send(request, HttpResponse.BodyHandlers.ofString());
        if (response.statusCode() == 401 && allowRefresh && credential.getRefreshToken() != null
                && credential.refreshToken()) {
            return create(point, credential, false);
        }
        if (response.statusCode() == 409) return 409;
        if (response.statusCode() < 200 || response.statusCode() >= 300) {
            throw new IOException("Google Health API error " + response.statusCode() + ": " + response.body());
        }
        return response.statusCode();
    }

    private ObjectNode baseBody() {
        ObjectNode body = mapper.createObjectNode();
        body.putObject("dataSource").put("recordingMethod", "MANUAL");
        return body;
    }

    private ObjectNode interval(String start, String end) {
        Instant startInstant = Instant.parse(start);
        Instant endInstant = Instant.parse(end);
        ObjectNode interval = mapper.createObjectNode();
        interval.put("startTime", startInstant.toString());
        interval.put("startUtcOffset", utcOffset(startInstant));
        interval.put("endTime", endInstant.toString());
        interval.put("endUtcOffset", utcOffset(endInstant));
        return interval;
    }

    private ObjectNode sampleTime(Instant instant) {
        ObjectNode sample = mapper.createObjectNode();
        sample.put("physicalTime", instant.toString());
        sample.put("utcOffset", utcOffset(instant));
        return sample;
    }

    private static String utcOffset(Instant instant) {
        return ZoneId.systemDefault().getRules().getOffset(instant).getTotalSeconds() + "s";
    }

    private static Instant instantFromNanos(long nanos) {
        if (nanos <= 0) throw new UnsupportedFormatException("日時が不正なデータポイントです。");
        return Instant.ofEpochSecond(Math.floorDiv(nanos, 1_000_000_000L), Math.floorMod(nanos, 1_000_000_000L));
    }

    private static double numericValue(JsonNode point) {
        JsonNode value = point.path("fitValue");
        if (value.isArray() && !value.isEmpty()) value = value.get(0).path("value");
        if (value.has("intVal")) return value.path("intVal").asDouble();
        if (value.has("fpVal")) return value.path("fpVal").asDouble();
        if (point.has("intValue")) return point.path("intValue").asDouble();
        return point.path("floatValue").asDouble();
    }

    private static String normalizeDuration(String duration, String start, String end) {
        if (duration.matches("\\d+(\\.\\d+)?s")) return duration;
        long seconds = Math.max(0, java.time.Duration.between(Instant.parse(start), Instant.parse(end)).toSeconds());
        return seconds + "s";
    }

    private static String exerciseType(String value) {
        String type = value.toUpperCase(Locale.ROOT).replace('-', '_').replace(' ', '_');
        return switch (type) {
            case "WALKING", "RUNNING", "HIKING", "BIKING", "SWIMMING", "YOGA" -> type;
            case "CYCLING" -> "BIKING";
            default -> "OTHER";
        };
    }

    private static String sleepStage(int value) {
        return switch (value) {
            case 1, 3 -> "AWAKE";
            case 4 -> "LIGHT";
            case 5 -> "DEEP";
            case 6 -> "REM";
            default -> "ASLEEP";
        };
    }

    private static boolean isDetailedStage(String stage) {
        return stage.equals("LIGHT") || stage.equals("DEEP") || stage.equals("REM");
    }

    private static String stableName(String type, String... values) {
        try {
            MessageDigest digest = MessageDigest.getInstance("SHA-256");
            digest.update(type.getBytes(StandardCharsets.UTF_8));
            for (String value : values) digest.update(value.getBytes(StandardCharsets.UTF_8));
            return "users/me/dataTypes/" + type + "/dataPoints/gfit-"
                    + HexFormat.of().formatHex(digest.digest(), 0, 16);
        } catch (Exception e) {
            throw new IllegalStateException(e);
        }
    }

    record HealthPoint(String dataType, String name, ObjectNode body) {
        HealthPoint {
            body.put("name", name);
        }
    }

    private record SleepStage(Instant start, Instant end, String type) { }

    public record ImportResult(String fileName, int detected, int imported, int duplicates, List<String> errors) { }

    public static class UnsupportedFormatException extends IllegalArgumentException {
        public UnsupportedFormatException(String message) { super(message); }
    }

    public static class ImportAuthorizationException extends IOException {
        public ImportAuthorizationException(String message) { super(message); }
    }
}
