namespace ScheduleViewer.Domain.Entities;

/// <summary>
/// カレンダーの添付ファイル
/// </summary>
public sealed class CalendarAttachmentEntity(DateTime date, string title, string url, string mimeType)
{
    /// <summary> 日付 </summary>
    public DateTime Date { get; } = date;

    /// <summary> タイトル </summary>
    public string Title { get; } = title;

    /// <summary> URL </summary>
    public string Url { get; } = url;

    /// <summary> MIME Type </summary>
    public string MimeType { get; } = mimeType;
}
