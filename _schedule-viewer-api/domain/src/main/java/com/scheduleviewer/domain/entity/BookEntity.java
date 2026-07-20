package com.scheduleviewer.domain.entity;

import java.time.LocalDate;

/**
 * Entity - 本
 */
public final class BookEntity {

    public static final BookEntity EMPTY = new BookEntity("", LocalDate.now(), "", "", "", "", "", "", "", "", "");

    private final String title;
    private final LocalDate readDate;
    private final String author;
    private final String publisher;
    private final String releasedDate;
    private final String type;
    private final String isbn10;
    private final String isbn13;
    private final String caption;
    private final String thumbnail;
    private final String rating;

    public BookEntity(
            String title,
            LocalDate readDate,
            String author,
            String publisher,
            String releasedDate,
            String type,
            String isbn10,
            String isbn13,
            String caption,
            String thumbnail,
            String rating) {
        this.title        = title;
        this.readDate     = readDate;
        this.author       = author;
        this.publisher    = publisher;
        this.releasedDate = releasedDate;
        this.type         = type;
        this.isbn10       = isbn10;
        this.isbn13       = isbn13;
        this.caption      = caption;
        this.thumbnail    = thumbnail;
        this.rating       = rating;
    }

    /** タイトル */
    public String getTitle() { return title; }

    /** 読了日 */
    public LocalDate getReadDate() { return readDate; }

    /** 著者 */
    public String getAuthor() { return author; }

    /** 出版社 */
    public String getPublisher() { return publisher; }

    /** 発売日 */
    public String getReleasedDate() { return releasedDate; }

    /** 本の種類 */
    public String getType() { return type; }

    /** ISBN-10 */
    public String getIsbn10() { return isbn10; }

    /** ISBN-13 */
    public String getIsbn13() { return isbn13; }

    /** 概要 */
    public String getCaption() { return caption; }

    /** サムネイル */
    public String getThumbnail() { return thumbnail; }

    /** 評価 */
    public String getRating() { return rating; }
}
