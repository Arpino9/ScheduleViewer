package com.scheduleviewer.api.controller;

import com.scheduleviewer.infrastructure.google.health.GoogleFitImportService;
import com.scheduleviewer.infrastructure.google.health.GoogleHealthApiService;
import com.scheduleviewer.infrastructure.google.health.GoogleHealthAuthService;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;
import org.springframework.mock.web.MockMultipartFile;
import org.springframework.test.web.servlet.MockMvc;

import java.io.InputStream;
import java.util.List;

import static org.mockito.ArgumentMatchers.any;
import static org.mockito.ArgumentMatchers.eq;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.get;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.multipart;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.post;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.jsonPath;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.status;
import static org.springframework.test.web.servlet.setup.MockMvcBuilders.standaloneSetup;

@ExtendWith(MockitoExtension.class)
class FitbitControllerTest {

    @Mock
    private GoogleHealthApiService apiService;

    @Mock
    private GoogleHealthAuthService authService;

    @Mock
    private GoogleFitImportService importService;

    private MockMvc mockMvc;

    @BeforeEach
    void setUp() {
        mockMvc = standaloneSetup(new FitbitController(apiService, authService, importService)).build();
    }

    @Test
    void exerciseDetailUsesSelectedResourceAndSafeErrors() throws Exception {
        String name = "users/123/dataTypes/exercise/dataPoints/456";
        var start = java.time.OffsetDateTime.parse("2026-10-18T12:45:00+09:00");
        var record = new com.scheduleviewer.domain.entity.ExerciseEntity("STRENGTH_TRAINING", "筋力トレーニング",
                start, start.plusMinutes(13), 780, true, "30kg\n10回");
        when(apiService.getExerciseDetail(name)).thenReturn(new com.scheduleviewer.domain.entity.ExerciseDetailEntity(
                record, 128d, List.of(new com.scheduleviewer.domain.entity.ExerciseDetailEntity.Zone("moderate", 780d)), List.of(), "empty"));
        mockMvc.perform(get("/api/fitbit/exercises/detail").param("name", name))
                .andExpect(status().isOk()).andExpect(jsonPath("$.averageHeartRate").value(128))
                .andExpect(jsonPath("$.zones[0].seconds").value(780))
                .andExpect(jsonPath("$.heartRateStatus").value("empty"));
        org.mockito.Mockito.doThrow(new GoogleHealthApiService.HealthAccessException(403)).when(apiService).getExerciseDetail(name);
        mockMvc.perform(get("/api/fitbit/exercises/detail").param("name", name)).andExpect(status().isForbidden());
        org.mockito.Mockito.doThrow(new java.io.IOException("private health notes")).when(apiService).getExerciseDetail(name);
        mockMvc.perform(get("/api/fitbit/exercises/detail").param("name", name))
                .andExpect(status().isBadGateway()).andExpect(jsonPath("$.message").value("Unable to retrieve exercise detail."));
    }

    @Test
    void exerciseDeletionReturnsPendingAndSafeFailures() throws Exception {
        String name = "users/123/dataTypes/exercise/dataPoints/456";
        String json = "{\"name\":\"" + name + "\"}";
        when(apiService.deleteExercise(name)).thenReturn(new GoogleHealthApiService.ExerciseSaveResult("pending"));
        mockMvc.perform(post("/api/fitbit/exercises/delete").contentType("application/json").content(json))
                .andExpect(status().isOk()).andExpect(jsonPath("$.status").value("pending"));
        for (int code : new int[] {400, 401, 403, 404}) {
            org.mockito.Mockito.doThrow(new GoogleHealthApiService.ExerciseWriteException(code)).when(apiService).deleteExercise(name);
            mockMvc.perform(post("/api/fitbit/exercises/delete").contentType("application/json").content(json))
                    .andExpect(status().is(code));
        }
        org.mockito.Mockito.doThrow(new java.io.IOException("private notes and token")).when(apiService).deleteExercise(name);
        mockMvc.perform(post("/api/fitbit/exercises/delete").contentType("application/json").content(json))
                .andExpect(status().isBadGateway())
                .andExpect(jsonPath("$.message").value("削除結果を確認できません。一覧を再取得して確認してください。"));
    }

    @Test
    void exerciseWriteRoutesUsePostAndPatchAndKeepFailuresDistinct() throws Exception {
        var input = new com.scheduleviewer.domain.entity.ExerciseSaveRequest(null, "STRENGTH_TRAINING",
                java.time.LocalDateTime.of(2026, 4, 11, 23, 30), java.time.LocalDateTime.of(2026, 4, 12, 0, 30), "40kg\n10回");
        when(apiService.saveExercise(input, false)).thenReturn(new GoogleHealthApiService.ExerciseSaveResult("saved"));
        String json = """
            {"exerciseType":"STRENGTH_TRAINING","startTime":"2026-04-11T23:30:00",
             "endTime":"2026-04-12T00:30:00","notes":"40kg\\n10回"}
            """;
        mockMvc.perform(post("/api/fitbit/exercises").contentType("application/json").content(json))
                .andExpect(status().isOk()).andExpect(jsonPath("$.status").value("saved"));
        when(apiService.saveExercise(input, true)).thenReturn(new GoogleHealthApiService.ExerciseSaveResult("pending"));
        mockMvc.perform(org.springframework.test.web.servlet.request.MockMvcRequestBuilders.patch("/api/fitbit/exercises")
                .contentType("application/json").content(json))
                .andExpect(status().isOk()).andExpect(jsonPath("$.status").value("pending"));
        for (int code : new int[] {400, 401, 403, 404, 409, 502}) {
            org.mockito.Mockito.doThrow(new GoogleHealthApiService.ExerciseWriteException(code)).when(apiService).saveExercise(input, false);
            mockMvc.perform(post("/api/fitbit/exercises").contentType("application/json").content(json)).andExpect(status().is(code));
        }
        mockMvc.perform(post("/api/fitbit/exercises").contentType("application/json").content("{bad json private Notes"))
                .andExpect(status().isBadRequest());
        mockMvc.perform(get("/api/fitbit/exercises/types"))
                .andExpect(status().isOk()).andExpect(jsonPath("$[?(@.value == 'STRENGTH_TRAINING')].label").value("筋力トレーニング"));
    }

    @Test
    void exercisesSerializeNotesAndOffsetWithoutChangingDailyActivity() throws Exception {
        var date = java.time.LocalDate.of(2026, 4, 11);
        var start = java.time.OffsetDateTime.parse("2026-04-11T23:30:00+09:00");
        when(apiService.getExercises(date)).thenReturn(List.of(new com.scheduleviewer.domain.entity.ExerciseEntity(
                "STRENGTH_TRAINING", "筋力トレーニング", start, start.plusHours(1), 1800, true, "40kg\n<script>x</script>")));
        mockMvc.perform(get("/api/fitbit/exercises").param("date", date.toString()))
                .andExpect(status().isOk())
                .andExpect(jsonPath("$[0].notes").value("40kg\n<script>x</script>"))
                .andExpect(jsonPath("$[0].activeDuration").value(true))
                .andExpect(jsonPath("$[0].durationSeconds").value(1800));
    }

    @Test
    void exercisesDistinguishEmptyUnauthorizedForbiddenAndFailure() throws Exception {
        var date = java.time.LocalDate.of(2026, 4, 11);
        when(apiService.getExercises(date)).thenReturn(List.of());
        mockMvc.perform(get("/api/fitbit/exercises").param("date", date.toString()))
                .andExpect(status().isOk()).andExpect(jsonPath("$").isEmpty());
        for (int code : new int[] {401, 403}) {
            org.mockito.Mockito.doThrow(new GoogleHealthApiService.HealthAccessException(code)).when(apiService).getExercises(date);
            mockMvc.perform(get("/api/fitbit/exercises").param("date", date.toString())).andExpect(status().is(code));
        }
        org.mockito.Mockito.doThrow(new java.io.IOException("private notes")).when(apiService).getExercises(date);
        mockMvc.perform(get("/api/fitbit/exercises").param("date", date.toString()))
                .andExpect(status().isBadGateway()).andExpect(jsonPath("$.message").value("Unable to retrieve exercises."));
        mockMvc.perform(get("/api/fitbit/exercises").param("date", "bad-date")).andExpect(status().isBadRequest());
    }

    @Test
    void importStatusUsesSeparateWriteCredential() throws Exception {
        when(authService.hasImportToken()).thenReturn(true);

        mockMvc.perform(get("/api/fitbit/import/status"))
                .andExpect(status().isOk())
                .andExpect(jsonPath("$.authorized").value(true));
    }

    @Test
    void forceImportAuthorizationStartsReauthorization() throws Exception {
        when(authService.reauthorizeImport()).thenReturn("https://accounts.google.com/o/oauth2/auth");

        mockMvc.perform(post("/api/fitbit/import/auth").param("force", "true"))
                .andExpect(status().isOk())
                .andExpect(jsonPath("$.status").value("pending"))
                .andExpect(jsonPath("$.url").value("https://accounts.google.com/o/oauth2/auth"));

        verify(authService).reauthorizeImport();
    }

    @Test
    void importAcceptsJsonAndReturnsPerFileResult() throws Exception {
        var file = new MockMultipartFile("files", "weight.json", "application/json", "{}".getBytes());
        when(importService.importJson(eq("weight.json"), any(InputStream.class)))
                .thenReturn(new GoogleFitImportService.ImportResult("weight.json", 1, 1, 0, List.of()));

        mockMvc.perform(multipart("/api/fitbit/import/google-fit").file(file))
                .andExpect(status().isOk())
                .andExpect(jsonPath("$.files[0].fileName").value("weight.json"))
                .andExpect(jsonPath("$.files[0].imported").value(1));
    }

    @Test
    void importRejectsNonJsonFiles() throws Exception {
        var file = new MockMultipartFile("files", "notes.txt", "text/plain", "text".getBytes());

        mockMvc.perform(multipart("/api/fitbit/import/google-fit").file(file))
                .andExpect(status().isBadRequest())
                .andExpect(jsonPath("$.message").value("JSON ファイルのみ選択できます。"));
    }
}
