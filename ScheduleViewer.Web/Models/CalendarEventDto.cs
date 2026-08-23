using System.Text.Json.Serialization;

namespace ScheduleViewer.Web.Models;

public sealed class CalendarEventDto
{
    /// <summary>Google CalendarのイベントID。</summary>
    public string EventId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string DisplayTitle { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Description { get; set; } = string.Empty;
    /// <summary>Steam実績など、予定カードへ表示する画像のURL。</summary>
    public string AchievementImageUrl { get; set; } = string.Empty;
    public bool Book { get; set; }
    public bool Program { get; set; }
    /// <summary>イベントへ関連付けられた外部添付リンク。</summary>
    public List<CalendarAttachmentDto> Attachments { get; set; } = [];

    /// <summary>開始・終了時刻を持たない全日イベントかどうか。</summary>
    [JsonPropertyName("allDay")]
    public bool IsAllDay { get; set; }
}
