namespace ScheduleViewer.Domain.ValueObjects;

/// <summary>
/// UIフレームワークに依存しないARGB形式の色を表します。
/// </summary>
/// <param name="Alpha">不透明度。</param>
/// <param name="Red">赤成分。</param>
/// <param name="Green">緑成分。</param>
/// <param name="Blue">青成分。</param>
public sealed record ArgbColorValue(byte Alpha, byte Red, byte Green, byte Blue);
