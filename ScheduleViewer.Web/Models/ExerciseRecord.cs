namespace ScheduleViewer.Web.Models;

public sealed record ExerciseRecord(string ExerciseType, string DisplayName, DateTimeOffset StartTime,
    DateTimeOffset EndTime, double DurationSeconds, bool ActiveDuration, string Notes,
    string Name = "", bool CanEdit = false, double? CaloriesKcal = null);
