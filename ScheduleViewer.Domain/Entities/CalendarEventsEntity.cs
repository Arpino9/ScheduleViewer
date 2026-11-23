namespace ScheduleViewer.Domain.Entities;

/// <summary>
/// Entity - Googleカレンダーのイベント
/// </summary>
public sealed class CalendarEventsEntity
{
    public static readonly CalendarEventsEntity Empty = new(
        eventId: string.Empty,
        title: string.Empty,
        startDate: DateTime.MinValue,
        endDate: DateTime.MinValue);

    /// <summary>
    /// Googleカレンダーのイベントを生成する。
    /// </summary>
    /// <param name="eventId">イベントID。</param>
    /// <param name="title">タイトル。</param>
    /// <param name="startDate">開始日時。</param>
    /// <param name="endDate">終了日時。</param>
    /// <param name="isAllDay">終日イベントかどうか。</param>
    /// <param name="displayTitle">表示用タイトル。省略時はタイトルを使用する。</param>
    /// <param name="progressingStartDate">進行中表示用の開始日時。省略時は開始日時を使用する。</param>
    /// <param name="place">場所。</param>
    /// <param name="description">説明。</param>
    /// <param name="achievementImageUrl">Steam実績などの画像URL。</param>
    /// <param name="attachments">イベントへ関連付けられた外部添付リンク。</param>
    public CalendarEventsEntity(
        string eventId,
        string title,
        DateTime startDate,
        DateTime endDate,
        bool isAllDay = false,
        string displayTitle = null,
        DateTime? progressingStartDate = null,
        string place = null,
        string description = null,
        string achievementImageUrl = null,
        IEnumerable<CalendarAttachmentEntity> attachments = null)
    {
        EventId = eventId ?? string.Empty;
        Title = title ?? string.Empty;
        DisplayTitle = string.IsNullOrEmpty(displayTitle) ? Title : displayTitle;
        Place = place ?? string.Empty;
        StartDate = startDate;
        ProgressingStartDate = progressingStartDate ?? startDate;
        EndDate = endDate;
        Description = description ?? string.Empty;
        AchievementImageUrl = achievementImageUrl ?? string.Empty;
        Attachments = attachments?.ToList() ?? [];
        IsAllDay = isAllDay;
    }

    /// <summary>イベントID。</summary>
    public string EventId { get; }

    /// <summary>終日イベントかどうか。</summary>
    public bool IsAllDay { get; }

    /// <summary>タイトル。</summary>
    public string Title { get; }

    /// <summary>表示用タイトル。</summary>
    public string DisplayTitle { get; }

    /// <summary>場所。</summary>
    public string Place { get; }

    /// <summary>開始日時。</summary>
    public DateTime StartDate { get; }

    /// <summary>進行中表示用の開始日時。</summary>
    public DateTime ProgressingStartDate { get; }

    /// <summary>終了日時。</summary>
    public DateTime EndDate { get; }

    /// <summary>説明。</summary>
    public string Description { get; }

    /// <summary>Steam実績など、予定カードへ表示する画像のURL。</summary>
    public string AchievementImageUrl { get; }

    /// <summary>イベントへ関連付けられた外部添付リンク。</summary>
    public IReadOnlyList<CalendarAttachmentEntity> Attachments { get; }

    /// <summary>本・テレビ番組を除いた全日イベントかどうか。</summary>
    public bool IsAllDayEvent => IsAllDay && !IsBook && !IsProgram;

    /// <summary>本のイベントかどうか。</summary>
    public bool IsBook => Description.Contains("【出版社】");

    /// <summary>テレビ番組のイベントかどうか。</summary>
    public bool IsProgram => Description.Contains("【視聴先】");
}
