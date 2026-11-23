namespace ScheduleViewer.Web.Models;

public sealed record ExerciseDetailRecord(ExerciseRecord Exercise, double? AverageHeartRate,
    IReadOnlyList<ExerciseZone> Zones, IReadOnlyList<ExerciseHeartPoint> HeartRate, string HeartRateStatus);
public sealed record ExerciseZone(string Key, double? Seconds);
public sealed record ExerciseHeartPoint(DateTimeOffset Time, double BeatsPerMinute);
