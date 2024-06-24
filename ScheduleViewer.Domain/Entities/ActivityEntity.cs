namespace ScheduleViewer.Domain.Entities;

/// <summary>
/// Entity - アクティビティ
/// </summary>
public sealed class ActivityEntity
{
    public ActivityEntity(int id, string name, DateTime date, int value, int hour)
    {
        this.Id    = id;
        this.Name  = name;
        this.Date  = date;
        this.Value = value;
        this.Hour  = hour;
    }

    /// <summary> ID </summary>
    public int Id { get; }

    public DateTime Date { get; }

    /// <summary> 名称 </summary>
    public string Name { get; }

    /// <summary> 値 </summary>
    public int Value { get; }

    /// <summary> 時間 </summary>
    public int Hour { get; }
}
