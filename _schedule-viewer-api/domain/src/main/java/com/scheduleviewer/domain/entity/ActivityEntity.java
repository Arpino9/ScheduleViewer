package com.scheduleviewer.domain.entity;

import java.time.LocalDateTime;

/**
 * Entity - アクティビティ
 */
public final class ActivityEntity {

    private final int id;
    private final String name;
    private final LocalDateTime date;
    private final int value;
    private final int hour;

    public ActivityEntity(int id, String name, LocalDateTime date, int value, int hour) {
        this.id    = id;
        this.name  = name;
        this.date  = date;
        this.value = value;
        this.hour  = hour;
    }

    /** ID */
    public int getId() { return id; }

    /** 名称 */
    public String getName() { return name; }

    /** 日付 */
    public LocalDateTime getDate() { return date; }

    /** 値 */
    public int getValue() { return value; }

    /** 時間 */
    public int getHour() { return hour; }
}
