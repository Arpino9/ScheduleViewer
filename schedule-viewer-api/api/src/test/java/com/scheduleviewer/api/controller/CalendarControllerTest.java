package com.scheduleviewer.api.controller;

import com.scheduleviewer.infrastructure.google.calendar.CalendarService;
import com.scheduleviewer.infrastructure.google.spreadsheet.SpreadsheetService;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;
import org.springframework.http.MediaType;
import org.springframework.test.web.servlet.MockMvc;

import java.time.LocalDate;

import static org.mockito.Mockito.verify;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.post;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.status;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.jsonPath;
import static org.springframework.test.web.servlet.setup.MockMvcBuilders.standaloneSetup;

@ExtendWith(MockitoExtension.class)
class CalendarControllerTest {

    @Mock
    private CalendarService calendarService;

    @Mock
    private SpreadsheetService spreadsheetService;

    private MockMvc mockMvc;

    @BeforeEach
    void setUp() {
        mockMvc = standaloneSetup(new CalendarController(calendarService, spreadsheetService)).build();
    }

    @Test
    void photoEndpointDelegatesEventIdAndUrl() throws Exception {
        mockMvc.perform(post("/api/calendar/events/{eventId}/photo", "event-123")
                        .contentType(MediaType.APPLICATION_JSON)
                        .content("{\"photoUrl\":\"https://photos.google.com/share/example\"}"))
                .andExpect(status().isNoContent());

        verify(calendarService).addPhotoUrl(
                "event-123", "https://photos.google.com/share/example");
    }

    @Test
    void boxAttachmentEndpointDelegatesSharedLinkMetadata() throws Exception {
        mockMvc.perform(post("/api/calendar/events/{eventId}/attachments", "event-123")
                        .contentType(MediaType.APPLICATION_JSON)
                        .content("{\"fileUrl\":\"https://app.box.com/s/example\",\"fileTitle\":\"contract.pdf\"}"))
                .andExpect(status().isNoContent());

        verify(calendarService).attachBoxFile(
                "event-123", "https://app.box.com/s/example", "contract.pdf");
    }

    @Test
    void calendarEventEndpointCreatesTimedEvent() throws Exception {
        mockMvc.perform(post("/api/calendar/events")
                        .contentType(MediaType.APPLICATION_JSON)
                        .content("""
                                {"title":"歯医者","date":"2026-08-16","allDay":false,
                                 "startTime":"11:30","endTime":"12:30","location":"東京","description":"定期検診"}
                                """))
                .andExpect(status().isCreated())
                .andExpect(jsonPath("$.status").value("ok"));

        verify(calendarService).createEvent(
                "歯医者", LocalDate.of(2026, 8, 16), false,
                java.time.LocalTime.of(11, 30), java.time.LocalTime.of(12, 30), "東京", "定期検診");
    }

    @Test
    void calendarEventEndpointRejectsInvalidTimeRange() throws Exception {
        mockMvc.perform(post("/api/calendar/events")
                        .contentType(MediaType.APPLICATION_JSON)
                        .content("""
                                {"title":"歯医者","date":"2026-08-16","allDay":false,
                                 "startTime":"12:30","endTime":"11:30"}
                                """))
                .andExpect(status().isBadRequest());
    }
}
