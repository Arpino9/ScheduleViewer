namespace ScheduleViewer.Domain.Entities;

/// <summary>
/// Entity - Googleカレンダーのイベント
/// </summary>
public sealed class CalendarEventsEntity
{
    public static readonly CalendarEventsEntity Empty = new CalendarEventsEntity(string.Empty, string.Empty, DateTime.MinValue, DateTime.MinValue, false, false );

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="eventId">イベントID</param>
    /// <param name="title">タイトル</param>
    /// <param name="startDate">開始日時</param>
    /// <param name="endDate">終了日時</param>
    /// <param name="book">本情報の有無</param>
    /// <param name="program">アニメ情報の有無</param>
    public CalendarEventsEntity(
        string eventId,
        string title,
        DateTime startDate,
        DateTime endDate, bool book, bool program) : this(eventId, title, startDate, endDate, string.Empty, string.Empty, book, program)
    {

    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="eventId">イベントID</param>
    /// <param name="title">タイトル</param>
    /// <param name="startDate">開始日時</param>
    /// <param name="endDate">終了日時</param>
    /// <param name="description">詳細</param>
    /// <param name="book">本情報の有無</param>
    /// <param name="program">アニメ情報の有無</param>
    /// <remarks>
    /// 終日イベント
    /// </remarks>
    public CalendarEventsEntity(string eventId, string title, DateTime startDate, DateTime endDate, string description, bool book, bool program)
        : this(eventId, title, startDate, endDate, string.Empty, description, book, program)
    {
        this.IsAllDay = true;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="eventId">イベントID</param>
    /// <param name="title">タイトル</param>, bool book, bool program
    /// <param name="startDate">開始日時</param>
    /// <param name="endDate">終了日時</param>
    /// <param name="place">場所</param>
    /// <param name="description">説明</param>
    /// <param name="book">本情報の有無</param>
    /// <param name="program">アニメ情報の有無</param>
    /// <remarks>
    /// 表示用のタイトルと分けたい場合
    /// </remarks>
    public CalendarEventsEntity(string eventId, string title, string displayTitle, DateTime displayStartDate, 
                                DateTime startDate, DateTime endDate, string place, string description, bool book, bool program)
        : this(eventId, title, startDate, endDate, place, description, book, program)
    {
        this.ProgressingStartDate = displayStartDate;
        this.DisplayTitle     = displayTitle;
        this.Book = false;
        this.Program = false;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="eventId">イベントID</param>
    /// <param name="title">タイトル</param>
    /// <param name="startDate">開始日時</param>
    /// <param name="endDate">終了日時</param>
    /// <param name="place">場所</param>
    /// <param name="description">説明</param>
    /// <param name="book">本情報の有無</param>
    /// <param name="program">アニメ情報の有無</param>
    /// <remarks>
    /// 通常のイベント
    /// </remarks>
    public CalendarEventsEntity(string eventId, string title, DateTime startDate, DateTime endDate, 
                                string place, string description, bool book, bool program)
    {
        this.EventId      = eventId;
        this.IsAllDay     = false;
        this.Title        = title;
        this.DisplayTitle = title;
        this.StartDate    = startDate;
        this.EndDate      = endDate;
        this.Place        = place;
        this.Description  = description;
        this.AchievementImageUrl = null;
        this.Book         = book;
        this.Program      = program;
    }

    public string EventId { get; set; }

    /// <summary> 終日か </summary>
    public bool IsAllDay { get; }

    /// <summary> タイトル </summary>
    public string Title { get; }

    /// <summary> 表示用タイトル </summary>
    public string DisplayTitle { get; set; }

    /// <summary> 場所 </summary>
    public string Place { get; }

    /// <summary> 開始日時 </summary>
    public DateTime StartDate { get; }

    /// <summary> 進行中表示用の開始日時 </summary>
    public DateTime ProgressingStartDate { get; }

    /// <summary> 終了日時 </summary>
    public DateTime EndDate { get; }

    /// <summary> 説明 </summary>
    public string Description { get; }

    /// <summary> 本情報の有無 </summary>
    public bool Book { get; set; }

    /// <summary> アニメ情報の有無 </summary>
    public bool Program { get; set; }

    /// <summary> Steam実績など、予定カードへ表示する画像のURL </summary>
    public string AchievementImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// 全日イベントか
    /// </summary>
    public bool IsAllDayEvent
    {
        get
        {
            if (this.IsBook)
            {
                return false;
            }

            if (this.IsProgram)
            {
                return false;
            }

            return this.IsAllDay;
        }
    }

    /// <summary>
    /// 本か
    /// </summary>
    public bool IsBook
    {
        get
        {
            if (this.Description is null)
            {
                return false;
            }

            return (this.Description.Contains("【出版社】"));
        }
    }

    /// <summary>
    /// テレビ番組か
    /// </summary>
    public bool IsProgram
    {
        get
        {
            if (this.Description is null)
            {
                return false;
            }

            return (this.Description.Contains("【視聴先】"));
        }
    }
}