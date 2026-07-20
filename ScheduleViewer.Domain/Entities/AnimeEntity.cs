namespace ScheduleViewer.Domain.Entities;

/// <summary>
/// Entity - アニメ情報
/// </summary>
/// <param name="registeredAnnict">Annictに登録されているか</param>
/// <param name="title">タイトル</param>
/// <param name="seasonName">制作シーズン</param>
/// <param name="seasonYear">制作年</param>
/// <param name="officialSiteUrl">公式サイト</param>
/// <param name="wikipediaUrl">Wikipedia URL</param>
/// <param name="episodesCount">エピソード数</param>
/// <param name="cast">キャスト</param>
/// <param name="thumbnail">サムネイル</param>
/// <param name="part">パート数</param>
/// <param name="subTitle">サブタイトル</param>
/// <param name="watchedFrom">視聴先</param>
/// <param name="caption">概要(話数別)</param>
/// <remarks>
/// こちらはViewModelと紐づくEntityなので、コントロールの増減に応じて調整可能
/// </remarks>
public sealed class AnimeEntity(
    bool registeredAnnict,
    string title, 
    string seasonName, 
    string seasonYear,
    string officialSiteUrl, 
    string wikipediaUrl, 
    string episodesCount,
    string cast,
    string thumbnail,
    string part,
    string subTitle,
    string watchedFrom,
    string caption)
{
    /// <summary> Annictに登録されているか </summary>
    public bool RegisteredAnnict { get; } = registeredAnnict;

    /// <summary> タイトル </summary>
    public string? Title { get; } = title;

    /// <summary> 制作シーズン </summary>
    public string? SeasonName { get; } = seasonName;

    /// <summary> 制作年 </summary>
    public string SeasonYear { get; } = seasonYear;

    /// <summary> 公式サイト </summary>
    public string? OfficialSiteUrl { get; } = officialSiteUrl;

    /// <summary> Wikipedia URL </summary>
    public string? WikipediaUrl { get; } = wikipediaUrl;

    /// <summary> エピソード数 </summary>
    public string EpisodesCount { get; } = episodesCount;

    /// <summary> キャスト </summary>
    public string? Cast { get; } = cast;

    /// <summary> サムネイル </summary>
    public string? Thumbnail { get; } = thumbnail;

    /// <summary> パート数 </summary>
    public string Part { get; } = part;

    /// <summary> サブタイトル </summary>
    public string SubTitle { get; } = subTitle;

    /// <summary> 視聴先 </summary>
    public string WatchedFrom { get; } = watchedFrom;

    /// <summary> 概要(話数別) </summary>
    public string Caption { get; } = caption;

    /// <summary>
    /// シーズン(表示用)
    /// </summary>
    /// <returns>シーズン</returns>
    public string DisplaySeason()
    {
        if (RegisteredAnnict)
        {
            return $"{this.SeasonYear}年 {this.SeasonName}";
        }
        else
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// パート(表示用)
    /// </summary>
    /// <returns>パート</returns>
    public string DisplayPart()
    {
        if (RegisteredAnnict)
        {
            return $"{this.Part} / {this.EpisodesCount}";

        }
        else
        {
            return this.Part;
        }
    }
}
