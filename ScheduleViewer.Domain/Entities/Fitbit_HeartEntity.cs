namespace ScheduleViewer.Domain.Entities;

/// <summary>
/// Entity - Fitbit (心拍数)
/// </summary>
public sealed class Fitbit_HeartEntity
{
    public Fitbit_HeartEntity(double restingHeartRate)
    {
        this.RestingHeartRate = restingHeartRate;
    }

    /// <summary> 安静時 </summary>
    public double RestingHeartRate { get; }
}
