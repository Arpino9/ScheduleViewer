package com.scheduleviewer.domain.entity;

import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;

/**
 * Entity - Googleカレンダーのイベント
 */
public final class CalendarEventsEntity {

    public static final CalendarEventsEntity EMPTY =
            new CalendarEventsEntity("", LocalDateTime.MIN, LocalDateTime.MIN, "", "");

    private final boolean isAllDay;
    private String eventId;
    private final String title;
    private String displayTitle;
    private List<AttachmentEntity> attachments = new ArrayList<>();
    private final String place;
    private final LocalDateTime startDate;
    private final LocalDateTime progressingStartDate;
    private final LocalDateTime endDate;
    private final String description;

    /** 通常のイベント */
    public CalendarEventsEntity(String title, LocalDateTime startDate, LocalDateTime endDate) {
        this(title, startDate, endDate, "", "");
    }

    /** 終日イベント */
    public CalendarEventsEntity(String title, LocalDateTime startDate, LocalDateTime endDate, String description) {
        this.isAllDay             = true;
        this.title                = title;
        this.displayTitle         = title;
        this.startDate            = startDate;
        this.progressingStartDate = startDate;
        this.endDate              = endDate;
        this.place                = "";
        this.description          = description;
    }

    /** 通常のイベント (場所・説明付き) */
    public CalendarEventsEntity(
            String title,
            LocalDateTime startDate,
            LocalDateTime endDate,
            String place,
            String description) {
        this.isAllDay             = false;
        this.title                = title;
        this.displayTitle         = title;
        this.startDate            = startDate;
        this.progressingStartDate = startDate;
        this.endDate              = endDate;
        this.place                = place;
        this.description          = description;
    }

    /** 表示用タイトルを分けたい場合 */
    public CalendarEventsEntity(
            String title,
            String displayTitle,
            LocalDateTime displayStartDate,
            LocalDateTime startDate,
            LocalDateTime endDate,
            String place,
            String description) {
        this.isAllDay             = false;
        this.title                = title;
        this.displayTitle         = displayTitle;
        this.progressingStartDate = displayStartDate;
        this.startDate            = startDate;
        this.endDate              = endDate;
        this.place                = place;
        this.description          = description;
    }

    /** 終日か */
    public boolean isAllDay() { return isAllDay; }

    /** タイトル */
    public String getTitle() { return title; }

    /** 表示用タイトル */
    public String getDisplayTitle() { return displayTitle; }
    public void setDisplayTitle(String displayTitle) { this.displayTitle = displayTitle; }

    /** 場所 */
    public String getPlace() { return place; }

    /** 開始日時 */
    public LocalDateTime getStartDate() { return startDate; }

    /** 進行中表示用の開始日時 */
    public LocalDateTime getProgressingStartDate() { return progressingStartDate; }

    /** 終了日時 */
    public LocalDateTime getEndDate() { return endDate; }

    /** 説明 */
    public String getDescription() { return description; }

    /** Google Calendar イベントID */
    public String getEventId() { return eventId; }
    public void setEventId(String eventId) { this.eventId = eventId; }

    /** 添付ファイル */
    public List<AttachmentEntity> getAttachments() { return attachments; }
    public void setAttachments(List<AttachmentEntity> attachments) { this.attachments = attachments; }

    /**
     * 全日イベントか
     * <p>本・テレビ番組は全日イベント扱いしない</p>
     */
    public boolean isAllDayEvent() {
        if (isBook() || isProgram()) return false;
        return isAllDay;
    }

    /** 本か (説明に【出版社】を含む) */
    public boolean isBook() {
        return description != null && description.contains("【出版社】");
    }

    /** テレビ番組か (説明に【視聴先】を含む) */
    public boolean isProgram() {
        return description != null && description.contains("【視聴先】");
    }
}
