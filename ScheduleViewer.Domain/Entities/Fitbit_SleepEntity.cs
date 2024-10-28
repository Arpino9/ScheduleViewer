namespace ScheduleViewer.Domain.Entities;

/// <summary>
/// Entity - Fitbit (睡眠データ)
/// </summary>
public sealed class Fitbit_SleepEntity
{
    public Fitbit_SleepEntity(
        DateTime startTime,
        DateTime endTime,
        TimeSpan asleep,
        TimeSpan awake,
        TimeSpan restless)
    {
        this.StartTime = startTime;
        this.EndTime   = endTime;
        this.Sleeping  = endTime - startTime;
        this.Asleep    = asleep;
        this.Awake     = awake;
        this.Restless  = restless;
    }

    /// <summary> 就寝時刻 </summary>
    public DateTime StartTime { get; }

    /// <summary> 起床時刻 </summary>
    public DateTime EndTime { get; }

    /// <summary> 睡眠時間 </summary>
    public TimeSpan Sleeping { get; }

    /// <summary> 睡眠中 </summary>
    public TimeSpan Asleep { get; }

    /// <summary> 覚醒状態 </summary>
    public TimeSpan Awake { get; }
    
    /// <summary> 寝付けない </summary>
    public TimeSpan Restless { get; }

    public override string ToString()
    {
        var minutes = this.Sleeping.TotalHours % 60;

        return (this.Sleeping.TotalHours == 0) ? 
            "データなし" : 
            $"{this.Sleeping.TotalHours.ToString()}時間{minutes.ToString()}分";
    }
}
