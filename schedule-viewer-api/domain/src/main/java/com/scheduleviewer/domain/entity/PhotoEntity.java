package com.scheduleviewer.domain.entity;

import java.time.LocalDateTime;

/**
 * Entity - 写真データ
 * <p>注: .NET版のBitmapImageはWPF固有のため除去。画像URLをクライアントが直接参照する。</p>
 */
public final class PhotoEntity {

    private final String id;
    private final LocalDateTime date;
    private final String fileName;
    private final String description;
    private final String url;
    private final String mimeType;
    private final long height;
    private final long width;

    public PhotoEntity(
            String id,
            LocalDateTime date,
            String fileName,
            String description,
            String url,
            String mimeType,
            long height,
            long width) {
        this.id          = id;
        this.date        = date;
        this.fileName    = fileName;
        this.description = description;
        this.url         = url;
        this.mimeType    = mimeType;
        this.height      = height;
        this.width       = width;
    }

    /** ID */
    public String getId() { return id; }

    /** 日付 */
    public LocalDateTime getDate() { return date; }

    /** ファイル名 */
    public String getFileName() { return fileName; }

    /** 説明 */
    public String getDescription() { return description; }

    /** URL */
    public String getUrl() { return url; }

    /** MIMEタイプ */
    public String getMimeType() { return mimeType; }

    /** 高さ */
    public long getHeight() { return height; }

    /** 幅 */
    public long getWidth() { return width; }
}
