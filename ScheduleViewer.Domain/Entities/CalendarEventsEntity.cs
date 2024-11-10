namespace ScheduleViewer.Domain.Entities;

/// <summary>
/// Entity - Googleカレンダーのイベント
/// </summary>
public sealed class CalendarEventsEntity
{
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
    /// 通常のイベント
    /// </remarks>
    public CalendarEventsEntity(string title, DateTime startDate, DateTime endDate, string place, string description)
    {
        this.IsAllDay    = false;
        this.Title       = title;
        this.StartDate   = startDate;
        this.EndDate     = endDate;
        this.Place       = place;
        this.Description = description;
    }

    /// <summary> 終日か </summary>
    public bool IsAllDay { get; set; }

    /// <summary> タイトル </summary>
    public string Title { get; set; }

    /// <summary> 場所 </summary>
    public string Place { get; set; }

    /// <summary> 開始日時 </summary>
    public DateTime StartDate { get; set; }

    /// <summary> 終了日時 </summary>
    public DateTime EndDate { get; set; }

    /// <summary> 説明 </summary>
    public string Description { get; set; }

    /// <summary>
    /// 本か
    /// </summary>
    public bool IsBook
    {
        get => (Description.Contains("【出版社】"));
    }

    /// <summary>
    /// テレビ番組か
    /// </summary>
    public bool IsProgram
    {
        get => (Description.Contains("【視聴先】"));
    }
}