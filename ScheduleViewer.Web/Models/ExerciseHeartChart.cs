using System.Globalization;

namespace ScheduleViewer.Web.Models;

/// <summary>Uses actual measurement times; gaps over two minutes are not connected.</summary>
public sealed class ExerciseHeartChart
{
    public IReadOnlyList<string> Paths { get; }
    public IReadOnlyList<(double Bpm, string Y)> Ticks { get; }
    public IReadOnlyList<(string X, string Y)> Dots { get; }
    public ExerciseHeartChart(IReadOnlyList<ExerciseHeartPoint> samples, DateTimeOffset start, DateTimeOffset end)
    {
        var points = samples.Where(p => double.IsFinite(p.BeatsPerMinute) && p.BeatsPerMinute is >= 1 and <= 300
            && p.Time >= start && p.Time < end).OrderBy(p => p.Time).ToArray();
        double min = points.Length == 0 ? 40 : Math.Max(0, Math.Floor((points.Min(p => p.BeatsPerMinute) - 10) / 10) * 10);
        double max = points.Length == 0 ? 180 : Math.Ceiling((points.Max(p => p.BeatsPerMinute) + 10) / 10) * 10;
        var paths = new List<string>();
        var dots = new List<(string, string)>();
        var path = new System.Text.StringBuilder();
        DateTimeOffset? previous = null;
        double duration = Math.Max(1, (end - start).TotalSeconds);
        foreach (var point in points)
        {
            string x = Number(48 + 624 * (point.Time - start).TotalSeconds / duration);
            string y = Number(200 - 176 * (point.BeatsPerMinute - min) / (max - min));
            if (previous is null || (point.Time - previous.Value).TotalSeconds > 120)
            {
                if (path.Length > 0) paths.Add(path.ToString());
                path.Clear().Append('M').Append(x).Append(',').Append(y);
            }
            else path.Append(" L").Append(x).Append(',').Append(y);
            dots.Add((x, y));
            previous = point.Time;
        }
        if (path.Length > 0) paths.Add(path.ToString());
        Paths = paths;
        Dots = dots;
        Ticks = new[] { min, (min + max) / 2, max }.Select(bpm => (bpm, Number(200 - 176 * (bpm - min) / (max - min)))).ToArray();
    }
    public static string Number(double value) => value.ToString("0.##", CultureInfo.InvariantCulture);
}
