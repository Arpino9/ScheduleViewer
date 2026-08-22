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
