package com.scheduleviewer.infrastructure.google.calendar;

import org.junit.jupiter.api.Test;

import java.time.LocalDateTime;
import java.util.Map;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNull;
import static org.junit.jupiter.api.Assertions.assertTrue;

class CalendarServiceTest {

    @Test
    void appendsPhotoSectionWithoutRemovingExistingDescription() {
        assertEquals(
                "概要\n\n【写真】\nhttps://photos.google.com/share/one",
                CalendarService.appendSectionValue(
                        "概要", "写真", "https://photos.google.com/share/one"));
    }

    @Test
    void insertsPhotoUrlBeforeFollowingSection() {
        assertEquals(
                "【写真】\nhttps://photos.google.com/share/one\nhttps://photos.google.com/share/two\n【概要】\n本文",
                CalendarService.appendSectionValue(
                        "【写真】\nhttps://photos.google.com/share/one\n【概要】\n本文",
                        "写真",
                        "https://photos.google.com/share/two"));
    }

    @Test
    void doesNotAddDuplicatePhotoUrl() {
        String description = "【写真】\nhttps://photos.google.com/share/one";

        assertEquals(
                description,
                CalendarService.appendSectionValue(
                        description, "写真", "https://photos.google.com/share/one"));
    }

    @Test
    void boxLinkMetadataPreservesUnrelatedPrivateProperties() throws Exception {
        var properties = CalendarService.addBoxLink(
                Map.of("another.application.key", "keep-me"),
                "https://app.box.com/s/example",
                "contract.pdf");

        assertEquals("keep-me", properties.get("another.application.key"));
        assertEquals(2, properties.size());
        assertTrue(properties.keySet().stream().anyMatch(key -> key.startsWith("scheduleviewer.box.")));
    }

    @Test
    void reattachingSameBoxUrlUpdatesMetadataInsteadOfAddingDuplicate() throws Exception {
        var first = CalendarService.addBoxLink(
                null, "https://app.box.com/s/example", "old-name.pdf");
        var second = CalendarService.addBoxLink(
                first, "https://app.box.com/s/example", "new-name.pdf");

        assertEquals(1, second.size());
        assertEquals(
                "new-name.pdf",
                CalendarService.readBoxAttachments(LocalDateTime.now(), second).getFirst().getTitle());
    }

    @Test
    void boxLinkMetadataIsReadAsAnAttachmentWithoutCalendarAttachmentMutation() throws Exception {
        var properties = CalendarService.addBoxLink(
                null, "https://app.box.com/s/example", "contract.pdf");

        var attachment = CalendarService.readBoxAttachments(
                LocalDateTime.of(2026, 8, 10, 10, 0), properties).getFirst();

        assertEquals("contract.pdf", attachment.getTitle());
        assertEquals("https://app.box.com/s/example", attachment.getUrl());
        assertEquals("application/vnd.scheduleviewer.box-link", attachment.getMimeType());
    }

    @Test
    void invalidBoxMetadataIsIgnored() {
        var attachments = CalendarService.readBoxAttachments(
                LocalDateTime.of(2026, 8, 10, 10, 0),
                Map.of("scheduleviewer.box.invalid", "not-json"));

        assertTrue(attachments.isEmpty());
    }

    @Test
    void boxMetadataPatchDoesNotContainDescriptionOrCalendarAttachments() {
        var patch = CalendarService.createBoxMetadataPatch(Map.of("scheduleviewer.box.example", "value"));

        assertNull(patch.getDescription());
        assertNull(patch.getAttachments());
        assertEquals(
                "value",
                patch.getExtendedProperties().getPrivate().get("scheduleviewer.box.example"));
    }
}
