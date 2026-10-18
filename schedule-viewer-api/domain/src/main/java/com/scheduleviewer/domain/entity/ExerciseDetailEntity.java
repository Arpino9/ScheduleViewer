package com.scheduleviewer.domain.entity;

import java.time.OffsetDateTime;
import java.util.List;

public record ExerciseDetailEntity(ExerciseEntity exercise, Double averageHeartRate,
                                   List<Zone> zones, List<HeartPoint> heartRate, String heartRateStatus) {
    public record Zone(String key, Double seconds) { }
    public record HeartPoint(OffsetDateTime time, double beatsPerMinute) { }
}
