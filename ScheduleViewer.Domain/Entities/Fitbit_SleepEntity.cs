namespace ScheduleViewer.Domain.Entities;

/// <summary>
/// Entity - Fitbit (睡眠データ)
/// </summary>
public sealed class Fitbit_SleepEntity
{
    public Fitbit_SleepEntity(
        DateTime startTime,
        DateTime endTime,
        TimeSpan awake,
        TimeSpan restless,
        TimeSpan rem,
        TimeSpan asleep)
    {
        this.StartTime = startTime;
        this.EndTime   = endTime;
        this.Sleeping  = (endTime - startTime) - awake;

        this.Asleep    = asleep;
        this.Awake     = awake;
        this.Rem       = rem;
        this.Restless  = restless;
    }

    /// <summary> 就寝時刻 </summary>
    public DateTime StartTime { get; }

    /// <summary> 起床時刻 </summary>
    public DateTime EndTime { get; }

    /// <summary> 睡眠時間 </summary>
    public TimeSpan Sleeping { get; }

    /// <summary> 覚醒状態 </summary>
    public TimeSpan Awake { get; }
    
    /// <summary> 寝付けない </summary>
    public TimeSpan Restless { get; }

    /// <summary> レム睡眠 </summary>
    public TimeSpan Rem { get; }

    /// <summary> 睡眠中 </summary>
    public TimeSpan Asleep { get; }

    public override string ToString()
    {
        var minutes = this.Sleeping.TotalHours % 60;

        return (this.Sleeping.TotalHours == 0) ? 
            "データなし" : 
            $"{this.Sleeping.TotalHours.ToString()}時間{minutes.ToString()}分";
    }
}
