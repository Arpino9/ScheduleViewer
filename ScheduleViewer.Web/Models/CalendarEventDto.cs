using System.Text.Json.Serialization;

namespace ScheduleViewer.Web.Models;

public sealed class CalendarEventDto
{
    public string EventId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string DisplayTitle { get; set; } = string.Empty;
    public string Place { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string AchievementImageUrl { get; set; } = string.Empty;
    public bool Book { get; set; }
    public bool Program { get; set; }
    public List<CalendarAttachmentDto> Attachments { get; set; } = [];

    [JsonPropertyName("allDay")]
    public bool IsAllDay { get; set; }
}

public sealed class TaskDto
{
    public string TaskListName { get; set; } = string.Empty;
    public string TaskName { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public DateTime? Completed { get; set; }
    public DateTime? DueDate { get; set; }
}

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

public sealed record MapLocationRecord(
    double Latitude,
    double Longitude);

public sealed record PhotoLinkRecord(
    string Title,
    string Url);

public sealed class CalendarAttachmentDto
{
    public DateTime Date { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
}

public sealed record AttachmentLinkRecord(
    string Title,
    string Url,
    string MimeType);

public sealed record CalendarSearchRecord(
    DateOnly Date,
    string Time,
    string Title,
    string Place,
    string Description,
    string Category,
    string TargetTab);

public sealed class ExpenditureDto
{
    public string Id { get; set; } = string.Empty;
    public string CanCalc { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public long Price { get; set; }
    public string FinancialInstitutions { get; set; } = string.Empty;
    public string CategoryLarge { get; set; } = string.Empty;
    public string CategoryMiddle { get; set; } = string.Empty;
    public string Memo { get; set; } = string.Empty;
    public string Change { get; set; } = string.Empty;
}

public sealed record ExpenditureRecord(
    string ItemName,
    long Price,
    string FinancialInstitutions,
    string CategoryLarge,
    string CategoryMiddle,
    string Memo,
    bool CanCalculate);

public sealed record TaskRecord(
    string ListName,
    string Title,
    string Details,
    DateTime? DueDate,
    bool Done);

public sealed record FitbitHealthRecord(
    double Steps,
    double CaloriesOut,
    double Elevation,
    double Distance,
    double RestingHeartRate,
    double Bmi,
    double Weight,
    DateTime? SleepStart,
    DateTime? SleepEnd,
    TimeSpan Sleeping,
    TimeSpan Awake,
    TimeSpan Restless,
    TimeSpan Rem,
    TimeSpan Asleep)
{
    public bool HasData => Steps > 0 || CaloriesOut > 0 || Elevation > 0 || Distance > 0 ||
                           RestingHeartRate > 0 || Weight > 0 || Sleeping > TimeSpan.Zero;
}

internal sealed class FitbitActivityDto
{
    public double Steps { get; set; }
    public double CaloriesOut { get; set; }
    public double Elevation { get; set; }
    public double Distance { get; set; }
}

internal sealed class FitbitHeartDto
{
    public double RestingHeartRate { get; set; }
}

internal sealed class FitbitWeightDto
{
    public double Bmi { get; set; }
    public double Weight { get; set; }
}

internal sealed class FitbitSleepDto
{
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public string Sleeping { get; set; } = "PT0S";
    public string Awake { get; set; } = "PT0S";
    public string Restless { get; set; } = "PT0S";
    public string Rem { get; set; } = "PT0S";
    public string Asleep { get; set; } = "PT0S";
}

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

public sealed record AnimeRecord(
    string CalendarTitle,
    string Title,
    string Part,
    string EpisodesCount,
    string SubTitle,
    string WatchedFrom,
    string Caption,
    string SeasonYear,
    string SeasonName,
    string Cast,
    string Thumbnail,
    string EpisodeThumbnail,
    string OfficialSiteUrl,
    string WikipediaUrl);

internal sealed class AnimeApiDto
{
    public string Title { get; set; } = string.Empty;
    public string SeasonName { get; set; } = string.Empty;
    public string SeasonYear { get; set; } = string.Empty;
    public string OfficialSiteUrl { get; set; } = string.Empty;
    public string WikipediaUrl { get; set; } = string.Empty;
    public string EpisodesCount { get; set; } = string.Empty;
    public string Cast { get; set; } = string.Empty;
    public string Thumbnail { get; set; } = string.Empty;
}

internal sealed class AnimeRegisterResponseDto
{
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
