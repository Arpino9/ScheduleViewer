package com.scheduleviewer.domain.entity;

import java.time.OffsetDateTime;

/** A complete session, with timestamps normalized to Asia/Tokyo. */
public record ExerciseEntity(String exerciseType, String displayName, OffsetDateTime startTime,
                             OffsetDateTime endTime, double durationSeconds, boolean activeDuration,
                             String notes, String name, boolean canEdit) {
    public ExerciseEntity(String exerciseType, String displayName, OffsetDateTime startTime,
                          OffsetDateTime endTime, double durationSeconds, boolean activeDuration, String notes) {
        this(exerciseType, displayName, startTime, endTime, durationSeconds, activeDuration, notes, "", false);
    }
}
