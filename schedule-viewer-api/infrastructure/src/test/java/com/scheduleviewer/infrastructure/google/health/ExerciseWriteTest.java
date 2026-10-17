package com.scheduleviewer.infrastructure.google.health;

import com.fasterxml.jackson.databind.*;
import com.fasterxml.jackson.databind.node.ObjectNode;
import com.google.api.client.auth.oauth2.*;
import com.scheduleviewer.domain.entity.ExerciseSaveRequest;
import org.junit.jupiter.api.*;
import java.net.http.*;
import java.nio.ByteBuffer;
import java.nio.charset.StandardCharsets;
import java.time.*;
import java.util.*;
import java.util.concurrent.Flow;
import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.*;
import static org.mockito.ArgumentMatchers.*;

class ExerciseWriteTest {
    private final ObjectMapper mapper = new ObjectMapper();
    private final GoogleHealthAuthService auth = mock(GoogleHealthAuthService.class);
    private final HttpClient http = mock(HttpClient.class);
    private final Queue<HttpResponse<String>> responses = new ArrayDeque<>();
    private final List<HttpRequest> requests = new ArrayList<>();
    private final GoogleHealthApiService service = new GoogleHealthApiService(auth, http);
    private final String name = "users/123/dataTypes/exercise/dataPoints/987";
    private final LocalDateTime start = LocalDateTime.of(2026, 4, 11, 23, 30);
    private final String notes = "チェストプレス 30kg × 45回\n<script>x</script>";

    @BeforeEach @SuppressWarnings("unchecked") void setup() throws Exception {
        when(auth.getCredential()).thenReturn(new Credential.Builder(BearerToken.authorizationHeaderAccessMethod()).build().setAccessToken("fake-token"));
        when(auth.getClientId()).thenReturn("our-client");
        when(http.send(any(HttpRequest.class), any(HttpResponse.BodyHandler.class))).thenAnswer(invocation -> {
            requests.add(invocation.getArgument(0)); return responses.remove();
        });
    }
    private ExerciseSaveRequest input(String recordName) {
        return new ExerciseSaveRequest(recordName, "STRENGTH_TRAINING", start, start.plusHours(1), notes);
    }
    @SuppressWarnings("unchecked") private void respond(int code, String json) {
        HttpResponse<String> response = mock(HttpResponse.class);
        when(response.statusCode()).thenReturn(code); when(response.body()).thenReturn(json); responses.add(response);
    }
    private ObjectNode existing() throws Exception {
        return (ObjectNode)mapper.readTree("""
            {"name":"users/123/dataTypes/exercise/dataPoints/987",
             "dataSource":{"recordingMethod":"MANUAL","platform":"GOOGLE_WEB_API","application":{"googleWebClientId":"our-client"}},
             "exercise":{"exerciseType":"STRENGTH_TRAINING","displayName":"Strength",
              "interval":{"startTime":"2026-04-11T14:30:00Z","endTime":"2026-04-11T15:30:00Z"},
              "activeDuration":"1800.5s","metricsSummary":{"caloriesKcal":100},"notes":"old",
              "createTime":"2026-04-12T00:00:00Z","updateTime":"2026-04-12T00:00:00Z"}}
            """);
    }
    private void saved() throws Exception { respond(200, mapper.createObjectNode().put("done", true).set("response", existing()).toString()); }
    private JsonNode body(HttpRequest request) throws Exception {
        var subscriber = HttpResponse.BodySubscribers.ofString(StandardCharsets.UTF_8);
        request.bodyPublisher().orElseThrow().subscribe(new Flow.Subscriber<ByteBuffer>() {
            public void onSubscribe(Flow.Subscription subscription) { subscriber.onSubscribe(subscription); }
            public void onNext(ByteBuffer item) { subscriber.onNext(List.of(item)); }
            public void onError(Throwable error) { subscriber.onError(error); }
            public void onComplete() { subscriber.onComplete(); }
        });
        return mapper.readTree(subscriber.getBody().toCompletableFuture().get());
    }
    @Test void createUsesTokyoInstantsNotesAndServerGeneratedId() throws Exception {
        saved(); assertEquals("saved", service.saveExercise(input(null), false).status());
        assertEquals("POST", requests.get(0).method());
        assertEquals("https://health.googleapis.com/v4/users/me/dataTypes/exercise/dataPoints", requests.get(0).uri().toString());
        var payload = body(requests.get(0)); assertFalse(payload.has("name"));
        var exercise = payload.path("exercise");
        assertEquals("2026-04-11T14:30:00Z", exercise.path("interval").path("startTime").asText());
        assertEquals("2026-04-11T15:30:00Z", exercise.path("interval").path("endTime").asText());
        assertEquals("32400s", exercise.path("interval").path("startUtcOffset").asText());
        assertEquals("3600s", exercise.path("activeDuration").asText());
        assertEquals(notes, exercise.path("notes").asText());
        assertTrue(exercise.has("metricsSummary"));
        assertEquals("MANUAL", payload.path("dataSource").path("recordingMethod").asText());
        assertTrue(GoogleHealthAuthService.SCOPES.contains("https://www.googleapis.com/auth/googlehealth.activity_and_fitness.writeonly"));
    }
    @Test void updateUsesResourceIdAndPreservesActiveDurationAndMetrics() throws Exception {
        respond(200, existing().toString()); saved();
        assertEquals("saved", service.saveExercise(input(name), true).status());
        assertEquals("GET", requests.get(0).method()); assertEquals("PATCH", requests.get(1).method());
        assertEquals("https://health.googleapis.com/v4/" + name, requests.get(1).uri().toString());
        var payload = body(requests.get(1)); assertEquals(name, payload.path("name").asText());
        var exercise = payload.path("exercise");
        assertEquals("1800.5s", exercise.path("activeDuration").asText());
        assertEquals(100, exercise.path("metricsSummary").path("caloriesKcal").asInt());
        assertEquals(notes, exercise.path("notes").asText());
        assertFalse(exercise.has("createTime")); assertFalse(exercise.has("updateTime"));
    }
    @Test void timestampEditRecalculatesDurationAndEmptyNotesAreWritten() throws Exception {
        respond(200, existing().toString()); saved();
        service.saveExercise(new ExerciseSaveRequest(name, "WALKING", start, start.plusMinutes(10), ""), true);
        var exercise = body(requests.get(1)).path("exercise");
        assertEquals("600s", exercise.path("activeDuration").asText());
        assertEquals("WALKING", exercise.path("exerciseType").asText());
        assertTrue(exercise.has("notes")); assertEquals("", exercise.path("notes").asText());
    }
    @Test void rejectsForeignRecordsAndUntrustedResourcePathsBeforeWriting() throws Exception {
        var point = existing(); ((ObjectNode)point.path("dataSource").path("application")).put("googleWebClientId", "other-client");
        respond(200, point.toString());
        assertEquals(403, assertThrows(GoogleHealthApiService.ExerciseWriteException.class, () -> service.saveExercise(input(name), true)).getStatus());
        assertEquals(1, requests.size());
        for (String invalid : List.of("https://evil.example/", "users/me/dataTypes/weight/dataPoints/abc", "users/me/dataTypes/exercise/dataPoints/../identity")) {
            assertEquals(400, assertThrows(GoogleHealthApiService.ExerciseWriteException.class, () -> service.saveExercise(input(invalid), true)).getStatus());
        }
        assertEquals(1, requests.size());
    }
    @Test void validatesDatesTypesAndNotesBeforeNetworkRequest() {
        for (var request : List.of(new ExerciseSaveRequest(null, "FAKE", start, start.plusHours(1), ""),
                new ExerciseSaveRequest(null, "WALKING", start, start, ""),
                new ExerciseSaveRequest(null, "WALKING", null, start, ""),
                new ExerciseSaveRequest(null, "WALKING", start, start.plusHours(1), "x".repeat(10001)))) {
            assertEquals(400, assertThrows(GoogleHealthApiService.ExerciseWriteException.class, () -> service.saveExercise(request, false)).getStatus());
        }
        assertTrue(requests.isEmpty());
    }
    @Test void operationErrorsAreNotSuccessAndDoNotLeakHealthData() throws Exception {
        respond(200, "{\"done\":true,\"error\":{\"code\":7,\"message\":\"private notes fake-token\"}}");
        var error = assertThrows(GoogleHealthApiService.ExerciseWriteException.class, () -> service.saveExercise(input(null), false));
        assertEquals(403, error.getStatus()); assertFalse(error.toString().contains("private")); assertNull(error.getCause());
        respond(200, "{\"name\":\"operations/123\",\"done\":false}");
        assertEquals("pending", service.saveExercise(input(null), false).status());
        respond(403, "private notes fake-token");
        var forbidden = assertThrows(GoogleHealthApiService.HealthAccessException.class, () -> service.saveExercise(input(null), false));
        assertFalse(forbidden.toString().contains("private"));
        assertEquals(3, requests.size());
    }
    @Test void deleteSendsExactlySelectedNameToBatchDelete() throws Exception {
        respond(200, existing().toString());
        respond(200, "{\"done\":true,\"response\":{\"@type\":\"type.googleapis.com/google.devicesandservices.health.v4.BatchDeleteDataPointsResponse\"}}");
        assertEquals("deleted", service.deleteExercise(name).status());
        assertEquals(2, requests.size());
        assertEquals("GET", requests.get(0).method());
        assertEquals("POST", requests.get(1).method());
        assertEquals("https://health.googleapis.com/v4/users/123/dataTypes/exercise/dataPoints:batchDelete", requests.get(1).uri().toString());
        var names = body(requests.get(1)).path("names");
        assertEquals(1, names.size());
        assertEquals(name, names.get(0).asText());
    }

    @Test void deleteRejectsInvalidNamesAndForeignRecordsWithoutDeleting() throws Exception {
        assertEquals(400, assertThrows(GoogleHealthApiService.ExerciseWriteException.class,
                () -> service.deleteExercise("users/me/dataTypes/weight/dataPoints/123")).getStatus());
        assertTrue(requests.isEmpty());
        var point = existing();
        ((ObjectNode)point.path("dataSource").path("application")).put("googleWebClientId", "other-client");
        respond(200, point.toString());
        assertEquals(403, assertThrows(GoogleHealthApiService.ExerciseWriteException.class,
                () -> service.deleteExercise(name)).getStatus());
        assertEquals(1, requests.size());
    }

    @Test void deleteDistinguishesPendingAndOperationError() throws Exception {
        respond(200, existing().toString());
        respond(200, "{\"done\":false,\"name\":\"operations/123\"}");
        assertEquals("pending", service.deleteExercise(name).status());
        respond(200, existing().toString());
        respond(200, "{\"done\":true,\"error\":{\"code\":7,\"message\":\"private health data\"}}");
        var error = assertThrows(GoogleHealthApiService.ExerciseWriteException.class, () -> service.deleteExercise(name));
        assertEquals(403, error.getStatus());
        assertFalse(error.toString().contains("private"));
        assertNull(error.getCause());
    }

    @Test void ownershipIsExposedForOnlyOwnManualRecords() throws Exception {
        var point = existing(); var root = mapper.createObjectNode(); root.putArray("dataPoints").add(point);
        var record = GoogleHealthApiService.parseExercises(root, LocalDate.of(2026, 4, 11), "our-client").getFirst();
        assertEquals(name, record.name()); assertTrue(record.canEdit());
        assertFalse(GoogleHealthApiService.parseExercises(root, LocalDate.of(2026, 4, 11), "other-client").getFirst().canEdit());
        assertFalse(GoogleHealthApiService.parseExercises(root, LocalDate.of(2026, 4, 11), "").getFirst().canEdit());
    }
}
