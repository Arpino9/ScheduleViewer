namespace ScheduleViewer.Domain.Entities;

/// <summary>
/// Entity - Fitbit (体重)
/// </summary>
public sealed class Fitbit_WeightEntity
{
    public Fitbit_WeightEntity(
        double bmi,
        double weight)
    {
        this.BMI    = bmi;
        this.Weight = weight;
    }

    /// <summary> BMI </summary>
    public double BMI { get; }

    /// <summary> 体重 </summary>
    public double Weight { get; }
}
