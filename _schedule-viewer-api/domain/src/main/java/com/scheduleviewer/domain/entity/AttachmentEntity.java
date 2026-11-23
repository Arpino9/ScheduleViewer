package com.scheduleviewer.domain.entity;

import java.time.LocalDateTime;

/**
 * Entity - 添付ファイル
 */
public final class AttachmentEntity {

    private final LocalDateTime date;
    private final String title;
    private final String url;
    private final String mimeType;

    public AttachmentEntity(LocalDateTime date, String title, String url, String mimeType) {
        this.date     = date;
        this.title    = title;
        this.url      = url;
        this.mimeType = mimeType;
    }

    /** 日付 */
    public LocalDateTime getDate() { return date; }

    /** タイトル */
    public String getTitle() { return title; }

    /** URL */
    public String getUrl() { return url; }

    /** ファイルタイプ */
    public String getMimeType() { return mimeType; }
}
