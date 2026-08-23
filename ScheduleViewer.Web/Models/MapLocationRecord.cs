namespace ScheduleViewer.Web.Models;

/// <summary>地図表示に使用する緯度・経度。</summary>
/// <param name="Latitude">緯度。</param>
/// <param name="Longitude">経度。</param>
public sealed record MapLocationRecord(
    double Latitude,
    double Longitude);
