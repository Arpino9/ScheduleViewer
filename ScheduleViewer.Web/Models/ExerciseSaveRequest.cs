namespace ScheduleViewer.Web.Models;

/// <summary>日時はブラウザの時間帯によらず日本時間として扱います。</summary>
public sealed record ExerciseSaveRequest(string? Name, string ExerciseType, DateTime StartTime,
    DateTime EndTime, string Notes);
public sealed record ExerciseTypeOption(string Value, string Label);
public sealed record ExerciseSaveResult(string Status);
