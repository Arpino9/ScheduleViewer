namespace ScheduleViewer.Web.Models;

public sealed record CalendarSearchRecord(
    DateOnly Date,
    string Time,
    string Title,
    string Place,
    string Description,
    string Category,
    string TargetTab);
