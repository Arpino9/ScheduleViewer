namespace ScheduleViewer.Web.Models;

public sealed record TaskRecord(
    string ListName,
    string Title,
    string Details,
    DateTime? DueDate,
    bool Done);
