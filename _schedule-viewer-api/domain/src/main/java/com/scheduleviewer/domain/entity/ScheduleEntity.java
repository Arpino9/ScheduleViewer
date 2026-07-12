package com.scheduleviewer.domain.entity;

import java.time.LocalDate;

/**
 * Entity - スケジュール
 * <p>注: .NET版のSolidColorBrush/BrushはWPF固有のため、CSS互換の16進カラーコード文字列に置き換え</p>
 */
public final class ScheduleEntity {

    private final LocalDate date;
    private final String dayText;
    private final String backgroundColor;
    private final String foregroundColor;
    private final String allDayEventText;
    private final String dailyEvent1Text;
    private final String dailyEvent2Text;
    private final String dailyEvent3Text;
    private final String dailyEvent4Text;
    private final String dailyEvent5Text;

    public ScheduleEntity(
            String foregroundColor,
            String backgroundColor,
            LocalDate date,
            String allDayEvent,
            String event1,
            String event2,
            String event3,
            String event4,
            String event5) {
        this.date             = date;
        this.dayText          = (date == null) ? "" : String.valueOf(date.getDayOfMonth());
        this.backgroundColor  = backgroundColor;
        this.foregroundColor  = foregroundColor;
        this.allDayEventText  = (allDayEvent == null || allDayEvent.isEmpty()) ? null : "★" + allDayEvent;
        this.dailyEvent1Text  = event1;
        this.dailyEvent2Text  = event2;
        this.dailyEvent3Text  = event3;
        this.dailyEvent4Text  = event4;
        this.dailyEvent5Text  = event5;
    }

    /** 日付 */
    public LocalDate getDate() { return date; }

    /** 日(表示用) */
    public String getDayText() { return dayText; }

    /** 背景色 (例: "#FFFFFF") */
    public String getBackgroundColor() { return backgroundColor; }

    /** 文字色 (例: "#000000") */
    public String getForegroundColor() { return foregroundColor; }

    /** 全日イベント */
    public String getAllDayEventText() { return allDayEventText; }

    /** イベント1 */
    public String getDailyEvent1Text() { return dailyEvent1Text; }

    /** イベント2 */
    public String getDailyEvent2Text() { return dailyEvent2Text; }

    /** イベント3 */
    public String getDailyEvent3Text() { return dailyEvent3Text; }

    /** イベント4 */
    public String getDailyEvent4Text() { return dailyEvent4Text; }

    /** イベント5 */
    public String getDailyEvent5Text() { return dailyEvent5Text; }
}
