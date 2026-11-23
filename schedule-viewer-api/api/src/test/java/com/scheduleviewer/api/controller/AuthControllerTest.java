package com.scheduleviewer.api.controller;

import com.scheduleviewer.infrastructure.google.GoogleAuthService;
import com.scheduleviewer.infrastructure.google.calendar.CalendarService;
import com.scheduleviewer.infrastructure.google.drive.DriveService;
import com.scheduleviewer.infrastructure.google.health.GoogleHealthAuthService;
import com.scheduleviewer.infrastructure.google.photo.PhotoService;
import com.scheduleviewer.infrastructure.google.spreadsheet.SpreadsheetService;
import com.scheduleviewer.infrastructure.google.tasks.TasksService;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;
import org.springframework.test.web.servlet.MockMvc;

import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.when;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.post;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.jsonPath;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.status;
import static org.springframework.test.web.servlet.setup.MockMvcBuilders.standaloneSetup;

@ExtendWith(MockitoExtension.class)
class AuthControllerTest {

    @Mock private GoogleAuthService authService;
    @Mock private CalendarService calendarService;
    @Mock private TasksService tasksService;
    @Mock private DriveService driveService;
    @Mock private PhotoService photoService;
    @Mock private SpreadsheetService spreadsheetService;
    @Mock private GoogleHealthAuthService healthAuthService;

    private MockMvc mockMvc;

    @BeforeEach
    void setUp() {
        mockMvc = standaloneSetup(new AuthController(
                authService,
                calendarService,
                tasksService,
                driveService,
                photoService,
                spreadsheetService,
                healthAuthService)).build();
    }

    @Test
    void forceAuthorizationIsForwardedToCalendarService() throws Exception {
        when(calendarService.getAuthUrl(true))
                .thenReturn("https://accounts.google.com/o/oauth2/auth?prompt=consent");

        mockMvc.perform(post("/api/auth/google/calendar").param("force", "true"))
                .andExpect(status().isOk())
                .andExpect(jsonPath("$.status").value("pending"))
                .andExpect(jsonPath("$.url")
                        .value("https://accounts.google.com/o/oauth2/auth?prompt=consent"));

        verify(calendarService).getAuthUrl(true);
    }
}
