package com.scheduleviewer.infrastructure.google.drive;

import com.scheduleviewer.infrastructure.config.AppProperties;
import org.junit.jupiter.api.Test;
import java.time.LocalDate;
import java.time.format.DateTimeParseException;
import static org.junit.jupiter.api.Assertions.*;

class DriveServiceTest {
    @Test
    void readsExportDatesWithoutMovingThemToToday() {
        var expected = LocalDate.of(2026, 10, 23).atStartOfDay();
        assertEquals(expected, DriveService.parseDate("2026-10-23"));
        assertEquals(expected, DriveService.parseDate("2026/10/23"));
        assertEquals(expected, DriveService.parseDate("2026-10-23T00:00:00"));
        assertEquals(LocalDate.of(2026, 1, 3).atStartOfDay(), DriveService.parseDate("2026/1/3"));
    }

    @Test
    void rejectsInvalidDatesRatherThanUsingToday() {
        assertThrows(DateTimeParseException.class, () -> DriveService.parseDate("invalid"));
        assertThrows(DateTimeParseException.class, () -> DriveService.parseDate("2026/2/30"));
    }

    @Test
    void missingFolderIsAnErrorRatherThanAnEmptyDay() {
        var service = new DriveService(null, new AppProperties());
        assertThrows(IllegalStateException.class, () -> service.getExpenditure(LocalDate.of(2026, 10, 23)));
        assertThrows(IllegalStateException.class, service::getAll);
    }
}
