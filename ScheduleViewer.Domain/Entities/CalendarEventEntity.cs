namespace ScheduleViewer.Domain.Entities;

/// <summary>
/// Entity - Googleカレンダーのイベント
/// </summary>
public class CalendarEventEntity
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="title">タイトル</param>
    /// <param name="startDate">開始日時</param>
    /// <param name="endDate">終了日時</param>
    public CalendarEventEntity(
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
    /// <param name="place">場所</param>
    /// <param name="description">説明</param>
    public CalendarEventEntity(
        string title,
        DateTime startDate,
        DateTime endDate,
        string place,
        string description)
    {
        Title = title;
        StartDate = startDate;
        EndDate = endDate;
        Place = place;
        Description = description;
        TimeSpan = endDate - startDate;
    }

    /// <summary> タイトル </summary>
    public string Title;

    /// <summary> 開始日時 </summary>
    public DateTime StartDate;

    /// <summary> 終了日時 </summary>
    public DateTime EndDate;

    /// <summary> 場所 </summary>
    public string Place;

    /// <summary> 説明 </summary>
    public string Description;

    /// <summary> 所要時間 </summary>
    public TimeSpan TimeSpan { get; set; }
}