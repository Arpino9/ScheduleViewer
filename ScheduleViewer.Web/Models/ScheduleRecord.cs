namespace ScheduleViewer.Web.Models;

public sealed record ScheduleRecord(
    string EventId,
    DateOnly Date,
    string Time,
    string Duration,
    string Title,
    string Tag,
    string Kind,
    string Place,
    string Description,
    string AchievementImageUrl,
    bool IsAllDay,
    IReadOnlyList<AttachmentLinkRecord> Attachments);
