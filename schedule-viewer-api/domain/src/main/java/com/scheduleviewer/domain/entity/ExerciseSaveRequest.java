package com.scheduleviewer.domain.entity;

import java.time.LocalDateTime;

/** Form timestamps are civil times in Asia/Tokyo, never the server timezone. */
public record ExerciseSaveRequest(String name, String exerciseType, LocalDateTime startTime,
                                  LocalDateTime endTime, String notes) { }
