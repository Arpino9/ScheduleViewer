using ScheduleViewer.Infrastructure.Annict;

namespace ScheduleViewer.WPF.Models;

/// <summary>
/// Model - スケジュール詳細 (アニメ一覧)
/// </summary>
public sealed class Model_ScheduleDetails_Anime : ModelBase<ViewModel_ScheduleDetails_Anime>, IViewer
{
    /// <summary> ViewModel - スケジュール詳細 (アニメ) </summary>
    internal override ViewModel_ScheduleDetails_Anime ViewModel { get; set; }

    #region Get Instance

    private static Model_ScheduleDetails_Anime model = null;

    public static Model_ScheduleDetails_Anime GetInstance()
    {
        if (model == null)
        {
            model = new Model_ScheduleDetails_Anime();
        }

        return model;
    }

    #endregion

    [Obsolete("非同期版を使うこと")]
    public void Initialize()
    {
        new NotImplementedException();
    }

    /// <summary>
    /// 初期化(非同期版)
    /// </summary>
    /// <returns>void</returns>
    public async Task InitializeAsync()
    {
        var events = GoogleFacade.Calendar.FindByDate(this.ViewModel_Header.Date.Value);

        if (events.IsEmpty())
        {
            return;
        }

        this.Reload(await this.ConvertToAnimeEntitiesAsync(events));

        this.ListView_SelectionChanged();
    }

    /// <summary>
    /// リロード
    /// </summary>
    /// <param name="animes">本</param>
    private void Reload(IList<AnimeEntity> animes)
    {
        this.ViewModel.Animes_ItemSource.Clear();

        foreach (var anime in animes)
        {
            this.ViewModel.Animes_ItemSource.Add(anime);
        }
    }

    /// <summary>
    /// エンティティに変換
    /// </summary>
    /// <param name="events">イベント</param>
    /// <returns>エンティティ</returns>
    private async Task<List<AnimeEntity>> ConvertToAnimeEntitiesAsync(IReadOnlyList<CalendarEventsEntity> events)
    {
        var animes = events.Where(x => x.Description != null &&
                                       x.Description.Contains("【視聴先】"));

        var entities = new List<AnimeEntity>();

        foreach (var anime in animes)
        {
            var elements = anime.Description.Split('\n');

            var results = await AnnictReader.FetchAsync(anime.Title.Split(' ')[0]);

            foreach (var annic in results)
            {
                if (annic.Title == GetTitle(anime.Title))
                {
                    entities.Add(CreateEntity(annic, anime));
                }
            }
        }
        
        return entities;

        string GetTitle(string title)
        {
            if (title.Split(" ").Count() > 2)
            {
                return title.Split(' ')[0] + " " + title.Split(' ')[1];
            }

            return title.Split(' ')[0];
        }

        AnimeEntity CreateEntity(AnimeEntity anime, CalendarEventsEntity cEvent)
        {
            var thumbnail = string.IsNullOrEmpty(anime.Thumbnail) ? this.GetThumbnails(anime.Title) : anime.Thumbnail;

            return new AnimeEntity(
                anime.Title,
                anime.SeasonName,
                anime.SeasonYear,
                anime.OfficialSiteUrl,
                anime.WikipediaUrl,
                anime.EpisodesCount,
                anime.Cast,
                thumbnail,
                this.GetPart(cEvent.Title),
                this.GetSubTitle(cEvent),
                this.GetWatchedFrom(cEvent),
                this.GetCaption(cEvent));
        }
    }

    /// <summary>
    /// サムネイルを取得
    /// </summary>
    /// <param name="title">タイトル</param>
    /// <returns>サムネイル</returns>
    private string GetThumbnails(string title)
    {
        var thumbnails = GoogleFacade.SpreadSheet.Initialize_Thumbnails();

        foreach (var thumbnail in thumbnails)
        {
            if (thumbnail[0].ToString() == title)
            {
                return thumbnail[1].ToString();
            }
        }

        return string.Empty;
    }

    /// <summary>
    /// 話数を取得
    /// </summary>
    /// <param name="title">タイトル</param>
    /// <returns>話数</returns>
    private int GetPart(string title)
    {
        if (title.Split(" ").Count() > 2)
        {
            return Convert.ToInt32(Regex.Replace(title.Split(' ')[2], @"[^0-9]", ""));
        }

        return Convert.ToInt32(Regex.Replace(title.Split(' ')[1], @"[^0-9]", ""));
    }

    /// <summary>
    /// サブタイトルを取得
    /// </summary>
    /// <param name="book">本情報</param>
    /// <returns>著者</returns>
    private string GetSubTitle(CalendarEventsEntity book)
    {
        var elements = DivideByElements(book);

        return elements[this.FindIndex(elements, "【サブタイトル】") + 1];
    }

    /// <summary>
    /// 視聴先を取得
    /// </summary>
    /// <param name="book">本情報</param>
    /// <returns>著者</returns>
    private string GetWatchedFrom(CalendarEventsEntity book)
    {
        var elements = DivideByElements(book);

        return elements[this.FindIndex(elements, "【視聴先】") + 1];
    }

    /// <summary>
    /// 視聴先を取得
    /// </summary>
    /// <param name="book">本情報</param>
    /// <returns>著者</returns>
    private string GetCaption(CalendarEventsEntity book)
    {
        var elements = DivideByElements(book);

        return elements[this.FindIndex(elements, "【概要】") + 1];
    }

    /// <summary>
    /// 詳細情報を分割する
    /// </summary>
    /// <param name="anime">詳細情報</param>
    /// <returns>詳細情報</returns>
    private string[] DivideByElements(CalendarEventsEntity anime)
        => anime.Description.Split('\n');

    /// <summary>
    /// インデックスを取得する
    /// </summary>
    /// <param name="elements">本情報</param>
    /// <param name="name">検索するインデックスの名称</param>
    /// <returns></returns>
    private int FindIndex(string[] elements, string name)
        => elements.Select((t, i) => new { Text = t, Index = i })
                   .Where(x => x.Text.StartsWith(name)).First().Index;

    public void ListView_SelectionChanged()
    {
        if (this.ViewModel.Animes_ItemSource.IsEmpty())
        {
            // リストが空
            return;
        }

        if (this.ViewModel.Animes_SelectedIndex.Value.IsUnSelected())
        {
            // 未選択
            return;
        }

        var entity = this.ViewModel.Animes_ItemSource[this.ViewModel.Animes_SelectedIndex.Value];

        this.ViewModel.Title_Text.Value    = entity.Title;
        this.ViewModel.Part_Text.Value     = entity.Part.ToString() + " / " + entity.EpisodesCount?.ToString();
        this.ViewModel.Subtitle_Text.Value = entity.SubTitle;
        this.ViewModel.Caption_Text.Value  = entity.Caption;

        this.ViewModel.Casts_Text.Value        = entity.Cast;
        this.ViewModel.Season_Text.Value       = entity.SeasonYear + " " + entity.SeasonName;
        this.ViewModel.WatchedFrom_Text.Value  = entity.WatchedFrom;
        this.ViewModel.OfficialSite_Text.Value = entity.OfficialSiteUrl;
        this.ViewModel.WikipediaUrl_Text.Value = entity.WikipediaUrl;
        this.ViewModel.Thumbnail_Source.Value  = BitmapUtils.ConvertFromURL(entity.Thumbnail);
    }

    public void Clear_ViewForm()
    {
        // サムネイル
        this.ViewModel.Thumbnail_Source.Value = default;

        this.ViewModel.Title_Text.Value    = string.Empty;
        this.ViewModel.Part_Text.Value     = string.Empty;
        this.ViewModel.Subtitle_Text.Value = string.Empty;
        this.ViewModel.Caption_Text.Value  = string.Empty;

        this.ViewModel.Casts_Text.Value        = string.Empty;
        this.ViewModel.Season_Text.Value       = string.Empty;
        this.ViewModel.WatchedFrom.Value       = string.Empty;
        this.ViewModel.OfficialSite_Text.Value = string.Empty;
        this.ViewModel.WikipediaUrl_Text.Value = string.Empty;
    }

    /// <summary>
    /// タイトル別にソート
    /// </summary>
    internal void SortByTitle()
    {
        if (ListUtils.IsSortedAscending(this.ViewModel.Animes_ItemSource, x => x.Title))
        {
            // 昇順 → 降順
            this.Reload(this.ViewModel.Animes_ItemSource.OrderByDescending(x => x.Title).ToList());
        }
        else
        {
            // 降順 → 昇順
            this.Reload(this.ViewModel.Animes_ItemSource.OrderBy(x => x.Title).ToList());
        }
    }

    /// <summary>
    /// 話数別にソート
    /// </summary>
    internal void SortByPart()
    {
        if (ListUtils.IsSortedAscending(this.ViewModel.Animes_ItemSource, x => x.Part))
        {
            // 昇順 → 降順
            this.Reload(this.ViewModel.Animes_ItemSource.OrderByDescending(x => x.Part).ToList());
        }
        else
        {
            // 降順 → 昇順
            this.Reload(this.ViewModel.Animes_ItemSource.OrderBy(x => x.Part).ToList());
        }
    }

    /// <summary>
    /// サブタイトル別にソート
    /// </summary>
    internal void SortBySubTitle()
    {
        if (ListUtils.IsSortedAscending(this.ViewModel.Animes_ItemSource, x => x.SubTitle))
        {
            // 昇順 → 降順
            this.Reload(this.ViewModel.Animes_ItemSource.OrderByDescending(x => x.SubTitle).ToList());
        }
        else
            // 降順 → 昇順
        {
            this.Reload(this.ViewModel.Animes_ItemSource.OrderBy(x => x.SubTitle).ToList());
        }
    }

    /// <summary>
    /// 視聴先別にソート
    /// </summary>
    internal void SortByWatchedFrom()
    {
        if (ListUtils.IsSortedAscending(this.ViewModel.Animes_ItemSource, x => x.WatchedFrom))
        {
            // 昇順 → 降順
            this.Reload(this.ViewModel.Animes_ItemSource.OrderByDescending(x => x.WatchedFrom).ToList());
        }
        else
        // 降順 → 昇順
        {
            this.Reload(this.ViewModel.Animes_ItemSource.OrderBy(x => x.WatchedFrom).ToList());
        }
    }

    /// <summary> ViewModel - スケジュール詳細 </summary>
    public ViewModel_ScheduleDetails ViewModel_Header { get; set; }
}
