namespace ScheduleViewer.Domain.ValueObjects;

/// <summary>
/// Value Object - 勤務日
/// </summary>
public sealed record class WorkingDateValue
{
    /// <summary> 不明 </summary>
    public static readonly WorkingDateValue Unknown = new WorkingDateValue(DateTime.MinValue);

    /// <summary> 就業中 </summary>
    public static readonly WorkingDateValue Working = new WorkingDateValue(DateTime.MaxValue);

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="value">勤務日</param>
    public WorkingDateValue(DateTime value)
    {
        this.Value = value;
    }

    /// <summary> 値 </summary>
    public readonly DateTime Value;

    /// <summary> 不明か </summary>
    public bool IsUnknown
        => (this.Value.ToString("yyyy/MM/dd") == WorkingDateValue.Unknown.Value.ToString("yyyy/MM/dd"));

    /// <summary> 就業中か </summary>
    public bool IsWorking
        => (this.Value.ToString("yyyy/MM/dd") == WorkingDateValue.Working.Value.ToString("yyyy/MM/dd"));

    public override string ToString()
        => (this.IsWorking ? "就業中" : this.Value.ToString("yyyy/MM/dd"));
}
