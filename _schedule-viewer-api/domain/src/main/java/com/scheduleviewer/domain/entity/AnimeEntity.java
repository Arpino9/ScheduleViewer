package com.scheduleviewer.domain.entity;

/**
 * Entity - アニメ情報
 */
public final class AnimeEntity {

    private final boolean registeredAnnict;
    private final String title;
    private final String seasonName;
    private final String seasonYear;
    private final String officialSiteUrl;
    private final String wikipediaUrl;
    private final String episodesCount;
    private final String cast;
    private final String thumbnail;
    private final String part;
    private final String subTitle;
    private final String watchedFrom;
    private final String caption;

    public AnimeEntity(
            boolean registeredAnnict,
            String title,
            String seasonName,
            String seasonYear,
            String officialSiteUrl,
            String wikipediaUrl,
            String episodesCount,
            String cast,
            String thumbnail,
            String part,
            String subTitle,
            String watchedFrom,
            String caption) {
        this.registeredAnnict = registeredAnnict;
        this.title            = title;
        this.seasonName       = seasonName;
        this.seasonYear       = seasonYear;
        this.officialSiteUrl  = officialSiteUrl;
        this.wikipediaUrl     = wikipediaUrl;
        this.episodesCount    = episodesCount;
        this.cast             = cast;
        this.thumbnail        = thumbnail;
        this.part             = part;
        this.subTitle         = subTitle;
        this.watchedFrom      = watchedFrom;
        this.caption          = caption;
    }

    /** Annictに登録されているか */
    public boolean isRegisteredAnnict() { return registeredAnnict; }

    /** タイトル */
    public String getTitle() { return title; }

    /** 制作シーズン */
    public String getSeasonName() { return seasonName; }

    /** 制作年 */
    public String getSeasonYear() { return seasonYear; }

    /** 公式サイト */
    public String getOfficialSiteUrl() { return officialSiteUrl; }

    /** Wikipedia URL */
    public String getWikipediaUrl() { return wikipediaUrl; }

    /** エピソード数 */
    public String getEpisodesCount() { return episodesCount; }

    /** キャスト */
    public String getCast() { return cast; }

    /** サムネイル */
    public String getThumbnail() { return thumbnail; }

    /** パート数 */
    public String getPart() { return part; }

    /** サブタイトル */
    public String getSubTitle() { return subTitle; }

    /** 視聴先 */
    public String getWatchedFrom() { return watchedFrom; }

    /** 概要 */
    public String getCaption() { return caption; }

    /** シーズン(表示用) */
    public String displaySeason() {
        return registeredAnnict ? seasonYear + "年 " + seasonName : "";
    }

    /** パート(表示用) */
    public String displayPart() {
        return registeredAnnict ? part + " / " + episodesCount : part;
    }
}
