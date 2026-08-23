namespace ScheduleViewer.Web.Models;

public sealed class CalendarAttachmentDto
{
    public DateTime Date { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
}
