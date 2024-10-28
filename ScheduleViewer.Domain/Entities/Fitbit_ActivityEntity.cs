namespace ScheduleViewer.Domain.Entities;

/// <summary>
/// Entity - Fitbit (アクティビティ)
/// </summary>
public sealed class Fitbit_ActivityEntity
{
    public Fitbit_ActivityEntity(
        double steps,
        double caloriesOut,
        double elevation,
        double distance)
    {
        this.Steps       = steps;
        this.CaloriesOut = caloriesOut;
        this.Elevation   = elevation;
        this.Distance    = distance;
    }

    /// <summary> 歩数 </summary>
    public double Steps { get; }

    /// <summary> 消費エネルギー </summary>
    public double CaloriesOut { get; }

    /// <summary> 階数 </summary>
    public double Elevation { get; }

    /// <summary> 距離 </summary>
    public double Distance { get; }
}
