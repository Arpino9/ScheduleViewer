namespace ScheduleViewer.Web.Models;

public sealed record BookRecord(
    string Title,
    DateOnly ReadDate,
    string Author,
    string Publisher,
    string ReleasedDate,
    string Type,
    string Isbn10,
    string Isbn13,
    string Caption,
    string Thumbnail,
    string Rating);
