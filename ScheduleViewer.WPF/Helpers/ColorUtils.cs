namespace ScheduleViewer.WPF.Helpers;

/// <summary>
/// WPFで色を扱うためのユーティリティです。
/// </summary>
public static class ColorUtils
{
    /// <summary>
    /// UI非依存のARGB値をWPFのブラシへ変換します。
    /// </summary>
    /// <param name="color">変換するARGB値。</param>
    /// <returns>ARGB値から生成したブラシ。</returns>
    public static SolidColorBrush ToWpfBrush(this ArgbColorValue color)
    {
        return new SolidColorBrush(Color.FromArgb(
            color.Alpha,
            color.Red,
            color.Green,
            color.Blue));
    }
}
