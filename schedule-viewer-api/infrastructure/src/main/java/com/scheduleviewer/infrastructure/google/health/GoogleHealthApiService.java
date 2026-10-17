package com.scheduleviewer.infrastructure.google.health;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.google.api.client.auth.oauth2.Credential;
import com.scheduleviewer.domain.entity.ExerciseEntity;
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

    @org.springframework.beans.factory.annotation.Autowired
    public GoogleHealthApiService(GoogleHealthAuthService authService) {
        this(authService, HttpClient.newHttpClient());
    }

    GoogleHealthApiService(GoogleHealthAuthService authService, HttpClient httpClient) {
        this.authService = authService;
        this.httpClient = httpClient;
        this.mapper = new ObjectMapper();
    }

    public FitbitProfileEntity getProfile() {
        // Google Health profile intentionally exposes fewer personal fields than legacy Fitbit.
        return new FitbitProfileEntity("", 0, "", 0, 0, List.of());
    }

    /** Sessions are attributed to their start date in Asia/Tokyo, including overnight sessions. */
    public List<ExerciseEntity> getExercises(LocalDate date) throws Exception {
        // Exercise supports civil start filters. Broaden for recording offsets, then
        // filter physical timestamps in Tokyo; civil time alone is not a Tokyo date.
        String filter = "exercise.interval.civil_start_time >= \"" + date.minusDays(2)
                + "T00:00:00\" AND exercise.interval.civil_start_time < \"" + date.plusDays(2) + "T00:00:00\"";
        String base = BASE + "/dataTypes/exercise/dataPoints?pageSize=25&dataSourceFamily="
                + encode(ALL_SOURCES) + "&filter=" + encode(filter);
        var result = new java.util.ArrayList<ExerciseEntity>();
        var tokens = new java.util.HashSet<String>();
        String token = "";
        String clientId = authService.getClientId();
        do {
            JsonNode page = requestJson("GET", base + (token.isEmpty() ? "" : "&pageToken=" + encode(token)), null, true);
            result.addAll(parseExercises(page, date, clientId));
            token = page.path("nextPageToken").asText("");
            if (!token.isEmpty() && !tokens.add(token)) throw new IOException("Repeated exercise page token.");
        } while (!token.isEmpty());
        result.sort(Comparator.comparing(ExerciseEntity::startTime));
        return result;
    }

    static List<ExerciseEntity> parseExercises(JsonNode root, LocalDate date) throws IOException {
        return parseExercises(root, date, "");
    }

    static List<ExerciseEntity> parseExercises(JsonNode root, LocalDate date, String clientId) throws IOException {
        var result = new java.util.ArrayList<ExerciseEntity>();
        try {
            for (JsonNode point : root.path("dataPoints")) {
                JsonNode exercise = point.path("exercise");
                var interval = exercise.path("interval");
                var start = OffsetDateTime.parse(interval.path("startTime").asText()).atZoneSameInstant(ZoneId.of("Asia/Tokyo"));
                var end = OffsetDateTime.parse(interval.path("endTime").asText()).atZoneSameInstant(ZoneId.of("Asia/Tokyo"));
                if (end.isBefore(start)) throw new IllegalArgumentException();
                if (!start.toLocalDate().equals(date)) continue;
                String active = exercise.path("activeDuration").asText("");
                Duration duration = active.isEmpty() ? Duration.between(start, end)
                        : Duration.parse("PT" + active.toUpperCase(java.util.Locale.ROOT));
                if (duration.isNegative()) throw new IllegalArgumentException();
                result.add(new ExerciseEntity(
                        exercise.path("exerciseType").asText("EXERCISE_TYPE_UNSPECIFIED"),
                        exercise.path("displayName").asText(""), start.toOffsetDateTime(), end.toOffsetDateTime(),
                        duration.getSeconds() + duration.getNano() / 1_000_000_000d,
                        !active.isEmpty(), exercise.path("notes").asText(""),
                        point.path("name").asText(""), canEditExercise(point, clientId), exerciseCalories(exercise)));
            }
        } catch (RuntimeException e) {
            // Do not attach payload/parser exceptions: notes are private health data.
            throw new IOException("Invalid exercise response.");
        }
        result.sort(Comparator.comparing(ExerciseEntity::startTime));
        return result;
    }

    private static Double exerciseCalories(JsonNode exercise) {
        JsonNode value = exercise.path("metricsSummary").path("caloriesKcal");
        if (!value.isNumber()) return null;
        double calories = value.asDouble();
        return Double.isFinite(calories) && calories >= 0 ? calories : null;
    }

    static boolean canEditExercise(JsonNode point, String clientId) {
        return clientId != null && !clientId.isBlank()
                && clientId.equals(point.path("dataSource").path("application").path("googleWebClientId").asText())
                && "GOOGLE_WEB_API".equals(point.path("dataSource").path("platform").asText())
                && "MANUAL".equals(point.path("dataSource").path("recordingMethod").asText())
                && validExerciseName(point.path("name").asText());
    }

    static boolean validExerciseName(String name) {
        return name != null && name.matches("users/[A-Za-z0-9_-]+/dataTypes/exercise/dataPoints/[A-Za-z0-9_-]+");
    }

    /** Google generates the resource name; create is never automatically retried after a timeout. */
    public ExerciseSaveResult saveExercise(com.scheduleviewer.domain.entity.ExerciseSaveRequest input, boolean update) throws Exception {
        if (input == null || !ExerciseTypes.contains(input.exerciseType()) || input.startTime() == null
                || input.endTime() == null || !input.endTime().isAfter(input.startTime())
                || input.notes() == null || input.notes().length() > 10000
                || (update && !validExerciseName(input.name()))
                || (!update && input.name() != null && !input.name().isEmpty())) {
            throw new ExerciseWriteException(400);
        }
        var start = input.startTime().atZone(ZoneId.of("Asia/Tokyo")).toInstant();
        var end = input.endTime().atZone(ZoneId.of("Asia/Tokyo")).toInstant();
        var body = mapper.createObjectNode();
        com.fasterxml.jackson.databind.node.ObjectNode exercise;
        boolean timeChanged = true;
        if (update) {
            JsonNode existing = requestJson("GET", "https://health.googleapis.com/v4/" + input.name(), null, true);
            if (!canEditExercise(existing, authService.getClientId())) throw new ExerciseWriteException(403);
            exercise = (com.fasterxml.jackson.databind.node.ObjectNode) existing.path("exercise").deepCopy();
            var previous = exercise.path("interval");
            timeChanged = !start.equals(java.time.Instant.parse(previous.path("startTime").asText()))
                    || !end.equals(java.time.Instant.parse(previous.path("endTime").asText()));
            // Do not discard measured segments when editing a manually imported record.
            if (timeChanged && (!exercise.path("exerciseEvents").isEmpty()
                    || !exercise.path("splits").isEmpty() || !exercise.path("splitSummaries").isEmpty())) {
                throw new ExerciseWriteException(409);
            }
            exercise.remove(List.of("createTime", "updateTime"));
            body.put("name", input.name());
        } else {
            exercise = mapper.createObjectNode();
            exercise.putObject("metricsSummary");
        }
        body.putObject("dataSource").put("recordingMethod", "MANUAL");
        var interval = exercise.putObject("interval");
        interval.put("startTime", start.toString());
        interval.put("endTime", end.toString());
        interval.put("startUtcOffset", "32400s");
        interval.put("endUtcOffset", "32400s");
        exercise.put("exerciseType", input.exerciseType());
        exercise.put("displayName", ExerciseTypes.label(input.exerciseType()));
        exercise.put("notes", input.notes());
        if (timeChanged) {
            Duration elapsed = Duration.between(start, end);
            exercise.put("activeDuration", java.math.BigDecimal.valueOf(elapsed.getSeconds())
                    .add(java.math.BigDecimal.valueOf(elapsed.getNano(), 9)).stripTrailingZeros().toPlainString() + "s");
        }
        body.set("exercise", exercise);
        JsonNode operation = requestJson(update ? "PATCH" : "POST", update
                ? "https://health.googleapis.com/v4/" + input.name() : BASE + "/dataTypes/exercise/dataPoints",
                mapper.writeValueAsString(body), true);
        return parseExerciseOperation(operation, true);
    }

    public ExerciseSaveResult deleteExercise(String name) throws Exception {
        if (!validExerciseName(name)) throw new ExerciseWriteException(400);
        JsonNode existing = requestJson("GET", "https://health.googleapis.com/v4/" + name, null, true);
        if (!canEditExercise(existing, authService.getClientId())) throw new ExerciseWriteException(403);
        var body = mapper.createObjectNode();
        body.putArray("names").add(name);
        String parent = name.substring(0, name.indexOf("/dataPoints/"));
        JsonNode operation = requestJson("POST", "https://health.googleapis.com/v4/" + parent + "/dataPoints:batchDelete",
                mapper.writeValueAsString(body), true);
        var result = parseExerciseOperation(operation, false);
        return new ExerciseSaveResult("saved".equals(result.status()) ? "deleted" : "pending");
    }

    private static ExerciseSaveResult parseExerciseOperation(JsonNode operation, boolean requiresDataPoint) throws IOException {
        if (operation.has("error")) {
            int code = operation.path("error").path("code").asInt();
            throw new ExerciseWriteException(switch (code) { case 3 -> 400; case 5 -> 404; case 7 -> 403;
                case 16 -> 401; case 6, 9, 10 -> 409; default -> 502; });
        }
        boolean done = operation.path("done").asBoolean();
        if (done && (!operation.path("response").isObject() || (requiresDataPoint
                && (!validExerciseName(operation.path("response").path("name").asText())
                    || !operation.path("response").path("exercise").isObject()))))
            throw new IOException("Invalid exercise operation.");
        if (!done && operation.path("name").asText().isBlank()) throw new IOException("Invalid exercise operation.");
        return new ExerciseSaveResult(done ? "saved" : "pending");
    }

    public record ExerciseSaveResult(String status) { }

    public static class ExerciseWriteException extends IOException {
        private final int status;
        public ExerciseWriteException(int status) { super("Unable to save exercise."); this.status = status; }
        public int getStatus() { return status; }
    }

    public static class HealthAccessException extends IOException {
        private final int status;
        public HealthAccessException(int status) { super("Google Health authorization required."); this.status = status; }
        public int getStatus() { return status; }
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
            throw new HealthAccessException(401);
        }

        HttpRequest.Builder builder = HttpRequest.newBuilder(URI.create(url))
                .timeout(Duration.ofSeconds(60))
                .header("Authorization", "Bearer " + credential.getAccessToken())
                .header("Accept", "application/json")
                .header("Accept-Language", "ja-JP");
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
            if (response.statusCode() == 401 || response.statusCode() == 403) throw new HealthAccessException(response.statusCode());
            if (url.contains("/dataTypes/exercise/dataPoints"))
                throw new ExerciseWriteException(switch (response.statusCode()) {
                    case 400, 404, 409 -> response.statusCode(); default -> 502; });
            throw new IOException("Google Health API error " + response.statusCode());
        }
        try {
            return mapper.readTree(response.body());
        } catch (IOException e) {
            throw new IOException("Invalid Google Health JSON response.");
        }
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
