namespace ScheduleViewer.Web.Models;

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
