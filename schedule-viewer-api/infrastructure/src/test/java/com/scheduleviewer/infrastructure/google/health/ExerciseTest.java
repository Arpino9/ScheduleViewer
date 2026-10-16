package com.scheduleviewer.infrastructure.google.health;

import com.fasterxml.jackson.databind.ObjectMapper;
import com.google.api.client.auth.oauth2.Credential;
import org.junit.jupiter.api.Test;
import java.time.LocalDate;
import java.io.IOException;
import java.net.http.*;
import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.*;
import static org.mockito.ArgumentMatchers.*;

class ExerciseTest {
    private final ObjectMapper mapper = new ObjectMapper();
    private final LocalDate date = LocalDate.of(2026, 4, 11);

    private String point(String start, String end, String extra) {
        return "{\"exercise\":{\"exerciseType\":\"STRENGTH_TRAINING\",\"displayName\":\"筋力トレーニング\","
            + "\"interval\":{\"startTime\":\"" + start + "\",\"endTime\":\"" + end + "\"}" + extra + "}}";
    }

    @Test void sortsPreservesNotesAndPrefersFractionalActiveDuration() throws Exception {
        String notes = "チェストプレス 40kg × 10回\n<script>alert(1)</script>";
        var root = mapper.readTree("{\"dataPoints\":["
            + point("2026-04-11T14:30:00Z", "2026-04-11T16:00:00Z", "") + ","
            + point("2026-04-10T15:00:00Z", "2026-04-10T16:00:00Z",
                ",\"activeDuration\":\"1800.5s\",\"notes\":" + mapper.writeValueAsString(notes)) + "]}");
        var records = GoogleHealthApiService.parseExercises(root, date);
        assertEquals(2, records.size());
        assertEquals(notes, records.get(0).notes());
        assertEquals(1800.5, records.get(0).durationSeconds());
        assertTrue(records.get(0).activeDuration());
        assertEquals(0, records.get(0).startTime().getHour());
        assertEquals(9 * 3600, records.get(0).startTime().getOffset().getTotalSeconds());
        assertFalse(records.get(1).activeDuration());
        assertEquals(5400, records.get(1).durationSeconds());
        assertEquals(date.plusDays(1), records.get(1).endTime().toLocalDate());
        assertEquals("", records.get(1).notes());
    }

    @Test void tokyoStartBoundaryIsInclusiveAndEndBoundaryExclusive() throws Exception {
        var root = mapper.readTree("{\"dataPoints\":["
            + point("2026-04-10T14:59:59Z", "2026-04-10T16:00:00Z", "") + ","
            + point("2026-04-10T08:00:00-07:00", "2026-04-10T09:00:00-07:00", "") + ","
            + point("2026-04-11T14:59:59Z", "2026-04-11T15:30:00Z", "") + ","
            + point("2026-04-11T15:00:00Z", "2026-04-11T16:00:00Z", "") + "]}");
        assertEquals(2, GoogleHealthApiService.parseExercises(root, date).size());
    }

    @Test void emptyAndInvalidDataAreDifferent() throws Exception {
        assertTrue(GoogleHealthApiService.parseExercises(mapper.readTree("{}"), date).isEmpty());
        assertTrue(GoogleHealthApiService.parseExercises(mapper.readTree("{\"dataPoints\":[]}"), date).isEmpty());
        var ex = assertThrows(IOException.class, () -> GoogleHealthApiService.parseExercises(
            mapper.readTree("{\"dataPoints\":[{\"exercise\":{\"notes\":\"private\"}}]}"), date));
        assertFalse(ex.toString().contains("private"));
        assertNull(ex.getCause());
    }

    @SuppressWarnings("unchecked")
    @Test void followsPagesEncodesTokenAndUsesReadScopeAllSources() throws Exception {
        var auth = mock(GoogleHealthAuthService.class);
        when(auth.getCredential()).thenReturn(new Credential.Builder(com.google.api.client.auth.oauth2.BearerToken.authorizationHeaderAccessMethod()).build().setAccessToken("test"));
        var http = mock(HttpClient.class);
        HttpResponse<String> first = mock(HttpResponse.class), second = mock(HttpResponse.class);
        when(first.statusCode()).thenReturn(200); when(second.statusCode()).thenReturn(200);
        when(first.body()).thenReturn("{\"nextPageToken\":\"a+/=\",\"dataPoints\":["
            + point("2026-04-11T14:00:00Z", "2026-04-11T15:00:00Z", "") + "]}");
        when(second.body()).thenReturn("{\"dataPoints\":["
            + point("2026-04-10T15:00:00Z", "2026-04-10T16:00:00Z", "") + "]}");
        when(http.send(any(HttpRequest.class), any(HttpResponse.BodyHandler.class))).thenReturn(first, second);
        var service = new GoogleHealthApiService(auth, http);
        var records = service.getExercises(date);
        assertEquals(2, records.size());
        assertEquals(0, records.get(0).startTime().getHour());
        var requests = org.mockito.ArgumentCaptor.forClass(HttpRequest.class);
        verify(http, times(2)).send(requests.capture(), any(HttpResponse.BodyHandler.class));
        String url = requests.getAllValues().get(1).uri().toString();
        assertTrue(url.contains("pageSize=25")); assertTrue(url.contains("pageToken=a%2B%2F%3D"));
        assertTrue(java.net.URLDecoder.decode(url, java.nio.charset.StandardCharsets.UTF_8).contains("exercise.interval.civil_start_time >= \"2026-04-09T00:00:00\""));
        assertTrue(url.contains("all-sources"));
        assertTrue(GoogleHealthAuthService.SCOPES.contains("https://www.googleapis.com/auth/googlehealth.activity_and_fitness.readonly"));
        when(http.send(any(HttpRequest.class), any(HttpResponse.BodyHandler.class))).thenReturn(first);
        when(first.statusCode()).thenReturn(403);
        var error = assertThrows(GoogleHealthApiService.HealthAccessException.class, () -> service.getExercises(date));
        assertEquals(403, error.getStatus());
        assertFalse(error.toString().contains("test"));
    }
}
