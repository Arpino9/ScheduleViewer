namespace ScheduleViewer.Domain.Entities;

/// <summary>
/// Entity - Googleカレンダーのイベント
/// </summary>
public sealed class CalendarEventsEntity
{
    public static readonly CalendarEventsEntity Empty = new CalendarEventsEntity(string.Empty, DateTime.MinValue, DateTime.MinValue, string.Empty, string.Empty);

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="title">タイトル</param>
    /// <param name="startDate">開始日時</param>
    /// <param name="endDate">終了日時</param>
    public CalendarEventsEntity(
        string title,
        DateTime startDate,
        DateTime endDate) : this(title, startDate, endDate, string.Empty, string.Empty)
    {

    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="title">タイトル</param>
    /// <param name="startDate">開始日時</param>
    /// <param name="endDate">終了日時</param>
    /// <param name="description">詳細</param>
    /// <remarks>
    /// 終日イベント
    /// </remarks>
    public CalendarEventsEntity(string title, DateTime startDate, DateTime endDate, string description)
        : this(title, startDate, endDate, string.Empty, description)
    {
        this.IsAllDay = true;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="title">タイトル</param>
    /// <param name="startDate">開始日時</param>
    /// <param name="endDate">終了日時</param>
    /// <param name="place">場所</param>
    /// <param name="description">説明</param>
    /// <remarks>
    /// 表示用のタイトルと分けたい場合
    /// </remarks>
    public CalendarEventsEntity(string title, string displayTitle, DateTime displayStartDate, DateTime startDate, DateTime endDate, string place, string description)
        : this(title, startDate, endDate, place, description)
    {
        this.ProgressingStartDate = displayStartDate;
        this.DisplayTitle     = displayTitle;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="title">タイトル</param>
    /// <param name="startDate">開始日時</param>
    /// <param name="endDate">終了日時</param>
    /// <param name="place">場所</param>
    /// <param name="description">説明</param>
    /// <remarks>
    /// 通常のイベント
    /// </remarks>
    public CalendarEventsEntity(string title, DateTime startDate, DateTime endDate, string place, string description)
    {
        this.IsAllDay     = false;
        this.Title        = title;
        this.DisplayTitle = title;
        this.StartDate    = startDate;
        this.EndDate      = endDate;
        this.Place        = place;
        this.Description  = description;
    }

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