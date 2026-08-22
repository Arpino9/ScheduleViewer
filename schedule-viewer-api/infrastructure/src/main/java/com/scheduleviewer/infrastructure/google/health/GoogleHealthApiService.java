package com.scheduleviewer.infrastructure.google.health;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.google.api.client.auth.oauth2.Credential;
import com.scheduleviewer.domain.entity.FitbitActivityEntity;
import com.scheduleviewer.domain.entity.FitbitHeartEntity;
import com.scheduleviewer.domain.entity.FitbitProfileEntity;
import com.scheduleviewer.domain.entity.FitbitSleepEntity;
import com.scheduleviewer.domain.entity.FitbitWeightEntity;
import org.springframework.stereotype.Service;

import java.io.IOException;
import java.net.URI;
import java.net.URLEncoder;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.nio.charset.StandardCharsets;
import java.time.Duration;
import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.OffsetDateTime;
import java.time.ZoneId;
import java.util.Comparator;
import java.util.List;

/** Reads reconciled wellness data from Google Health API v4. */
@Service
public class GoogleHealthApiService {

    private static final String BASE = "https://health.googleapis.com/v4/users/me";
    private static final String ALL_SOURCES = "users/me/dataSourceFamilies/all-sources";

    private final GoogleHealthAuthService authService;
    private final HttpClient httpClient;
    private final ObjectMapper mapper;

    public GoogleHealthApiService(GoogleHealthAuthService authService) {
        this.authService = authService;
        this.httpClient = HttpClient.newHttpClient();
        this.mapper = new ObjectMapper();
    }

    public FitbitProfileEntity getProfile() {
        // Google Health profile intentionally exposes fewer personal fields than legacy Fitbit.
        return new FitbitProfileEntity("", 0, "", 0, 0, List.of());
    }

    public FitbitActivityEntity getActivity(LocalDate date) throws Exception {
        return parseActivity(
                dailyRollup("steps", date),
                dailyRollup("total-calories", date),
                dailyRollup("altitude", date),
                dailyRollup("distance", date));
    }

    public FitbitHeartEntity getHeart(LocalDate date) throws Exception {
        String filter = "daily_resting_heart_rate.date >= \"" + date
                + "\" AND daily_resting_heart_rate.date < \"" + date.plusDays(1) + "\"";
        return parseHeart(reconcile("daily-resting-heart-rate", filter));
    }

    public FitbitWeightEntity getWeight(LocalDate date) throws Exception {
        return parseWeight(dailyRollup("weight", date));
    }

    public FitbitSleepEntity getSleep(LocalDate date) throws Exception {
        String filter = "sleep.interval.civil_end_time >= \"" + date + "T00:00:00\""
                + " AND sleep.interval.civil_end_time < \"" + date.plusDays(1) + "T00:00:00\"";
        return parseSleep(reconcile("sleep", filter));
    }

    private JsonNode dailyRollup(String dataType, LocalDate date) throws Exception {
        var body = mapper.createObjectNode();
        var range = body.putObject("range");
        putCivilDate(range.putObject("start"), date);
        putCivilDate(range.putObject("end"), date.plusDays(1));
        body.put("windowSizeDays", 1);
        body.put("dataSourceFamily", ALL_SOURCES);

        return requestJson("POST", BASE + "/dataTypes/" + dataType + "/dataPoints:dailyRollUp",
                mapper.writeValueAsString(body), true);
    }

    private JsonNode reconcile(String dataType, String filter) throws Exception {
        String query = "dataSourceFamily=" + encode(ALL_SOURCES) + "&filter=" + encode(filter);
        return requestJson("GET", BASE + "/dataTypes/" + dataType + "/dataPoints:reconcile?" + query,
                null, true);
    }

    private JsonNode requestJson(String method, String url, String body, boolean allowRefresh) throws Exception {
        Credential credential = authService.getCredential();
        if (credential == null || credential.getAccessToken() == null) {
            throw new IOException("Google Health is not authorized.");
        }

        HttpRequest.Builder builder = HttpRequest.newBuilder(URI.create(url))
                .header("Authorization", "Bearer " + credential.getAccessToken())
                .header("Accept", "application/json");
        if (body == null) {
            builder.GET();
        } else {
            builder.header("Content-Type", "application/json")
                    .method(method, HttpRequest.BodyPublishers.ofString(body));
        }

        HttpResponse<String> response = httpClient.send(builder.build(), HttpResponse.BodyHandlers.ofString());
        if (response.statusCode() == 401 && allowRefresh && credential.getRefreshToken() != null
                && credential.refreshToken()) {
            return requestJson(method, url, body, false);
        }
        if (response.statusCode() < 200 || response.statusCode() >= 300) {
            throw new IOException("Google Health API error " + response.statusCode() + ": " + response.body());
        }
        return mapper.readTree(response.body());
    }

    private static void putCivilDate(com.fasterxml.jackson.databind.node.ObjectNode node, LocalDate date) {
        var value = node.putObject("date");
        value.put("year", date.getYear());
        value.put("month", date.getMonthValue());
        value.put("day", date.getDayOfMonth());
    }

    private static String encode(String value) {
        return URLEncoder.encode(value, StandardCharsets.UTF_8);
    }

    static FitbitActivityEntity parseActivity(
            JsonNode stepsRoot,
            JsonNode caloriesRoot,
            JsonNode altitudeRoot,
            JsonNode distanceRoot) {
        double steps = number(firstRollup(stepsRoot).path("steps"), "countSum");
        double calories = number(firstRollup(caloriesRoot).path("totalCalories"), "kcalSum");
        double elevationMeters = number(firstRollup(altitudeRoot).path("altitude"), "gainMillimetersSum") / 1_000d;
        double distanceKm = number(firstRollup(distanceRoot).path("distance"), "millimetersSum") / 1_000_000d;
        return new FitbitActivityEntity(steps, calories, elevationMeters, distanceKm);
    }

    static FitbitHeartEntity parseHeart(JsonNode root) {
        double bpm = streamDataPoints(root)
                .map(point -> point.path("dailyRestingHeartRate"))
                .mapToDouble(value -> number(value, "beatsPerMinute"))
                .filter(value -> value > 0)
                .findFirst()
                .orElse(0);
        return new FitbitHeartEntity(bpm);
    }

    static FitbitWeightEntity parseWeight(JsonNode root) {
        double kilograms = number(firstRollup(root).path("weight"), "weightGramsAvg") / 1_000d;
        return new FitbitWeightEntity(0, kilograms);
    }

    static FitbitSleepEntity parseSleep(JsonNode root) {
        JsonNode sleep = streamDataPoints(root)
                .map(point -> point.path("sleep"))
                .filter(value -> !value.isMissingNode() && !value.isNull())
                .max(Comparator.comparing(GoogleHealthApiService::sleepEndForSort))
                .orElse(null);
        if (sleep == null) return emptySleep();

        JsonNode interval = sleep.path("interval");
        LocalDateTime start = localDateTime(interval, "civilStartTime", "startTime", "startUtcOffset");
        LocalDateTime end = localDateTime(interval, "civilEndTime", "endTime", "endUtcOffset");
        if (start == null || end == null || end.isBefore(start)) return emptySleep();

        long awake = 0;
        long restless = 0;
        long rem = 0;
        long asleep = 0;
        for (JsonNode stage : sleep.path("summary").path("stagesSummary")) {
            long minutes = longNumber(stage.path("minutes"));
            switch (stage.path("type").asText()) {
                case "AWAKE" -> awake += minutes;
                case "RESTLESS" -> restless += minutes;
                case "REM" -> rem += minutes;
                case "LIGHT", "DEEP", "ASLEEP" -> asleep += minutes;
                default -> { }
            }
        }

        long intervalMinutes = Math.max(0, Duration.between(start, end).toMinutes());
        awake = Math.min(awake, intervalMinutes);
        return new FitbitSleepEntity(start, end,
                Duration.ofMinutes(awake),
                Duration.ofMinutes(restless),
                Duration.ofMinutes(rem),
                Duration.ofMinutes(asleep));
    }

    private static java.util.stream.Stream<JsonNode> streamDataPoints(JsonNode root) {
        JsonNode points = root.path("dataPoints");
        if (!points.isArray()) return java.util.stream.Stream.empty();
        return java.util.stream.StreamSupport.stream(points.spliterator(), false);
    }

    private static JsonNode firstRollup(JsonNode root) {
        JsonNode points = root.path("rollupDataPoints");
        return points.isArray() && !points.isEmpty()
                ? points.get(0)
                : com.fasterxml.jackson.databind.node.MissingNode.getInstance();
    }

    private static double number(JsonNode node, String field) {
        return doubleNumber(node.path(field));
    }

    private static double doubleNumber(JsonNode node) {
        if (node.isNumber()) return node.asDouble();
        if (node.isTextual()) {
            try { return Double.parseDouble(node.asText()); }
            catch (NumberFormatException ignored) { }
        }
        return 0;
    }

    private static long longNumber(JsonNode node) {
        if (node.canConvertToLong()) return node.asLong();
        if (node.isTextual()) {
            try { return Long.parseLong(node.asText()); }
            catch (NumberFormatException ignored) { }
        }
        return 0;
    }

    private static LocalDateTime sleepEndForSort(JsonNode sleep) {
        LocalDateTime value = localDateTime(sleep.path("interval"), "civilEndTime", "endTime", "endUtcOffset");
        return value == null ? LocalDateTime.MIN : value;
    }

    private static LocalDateTime localDateTime(
            JsonNode interval,
            String civilField,
            String physicalField,
            String offsetField) {
        JsonNode civil = interval.path(civilField);
        JsonNode date = civil.path("date");
        if (date.path("year").asInt() > 0) {
            JsonNode time = civil.path("time");
            return LocalDateTime.of(
                    date.path("year").asInt(),
                    date.path("month").asInt(),
                    date.path("day").asInt(),
                    time.path("hours").asInt(),
                    time.path("minutes").asInt(),
                    time.path("seconds").asInt());
        }

        String physical = interval.path(physicalField).asText("");
        if (physical.isEmpty()) return null;
        try {
            OffsetDateTime parsed = OffsetDateTime.parse(physical);
            long offsetSeconds = parseDurationSeconds(interval.path(offsetField).asText("0s"));
            return parsed.toInstant().atOffset(java.time.ZoneOffset.ofTotalSeconds((int) offsetSeconds)).toLocalDateTime();
        } catch (Exception ignored) {
            try { return OffsetDateTime.parse(physical).atZoneSameInstant(ZoneId.systemDefault()).toLocalDateTime(); }
            catch (Exception ignoredAgain) { return null; }
        }
    }

    private static long parseDurationSeconds(String value) {
        if (value == null || !value.endsWith("s")) return 0;
        try { return Math.round(Double.parseDouble(value.substring(0, value.length() - 1))); }
        catch (NumberFormatException ignored) { return 0; }
    }

    private static FitbitSleepEntity emptySleep() {
        LocalDateTime empty = LocalDateTime.of(1, 1, 1, 0, 0);
        return new FitbitSleepEntity(empty, empty,
                Duration.ZERO, Duration.ZERO, Duration.ZERO, Duration.ZERO);
    }
}
