namespace ScheduleViewer.Domain.Modules.Helpers;

/// <summary>
/// Utility - 色情報
/// </summary>
public static class ColorUtils
{
    //TODO: メソッド名は要検討、
    //MEMO: Labelにバインディングするため、値オブジェクトは不採用
    /// <summary>
    /// WPFの前景色、背景色に変換
    /// </summary>
    /// <param name="color">色情報</param>
    /// <returns>色定義</returns>
    public static SolidColorBrush ToWPFColor(Color color)
    {
        var brush = new SolidColorBrush();
        brush.Color = Color.FromArgb(color.A, color.R, color.G, color.B);

        return brush;
    }

    public static SolidColorBrush ToWPFColor(byte colorAlpha, byte colorRed, byte colorGreen, byte colorBlue)
    {
        var brush = new SolidColorBrush();

        brush.Color = Color.FromArgb(colorAlpha, colorRed, colorGreen, colorBlue);

        return brush;
    }

    public static SolidColorBrush ToWPFColor(string colorAlpha, string colorRed, string colorGreen, string colorBlue)
    {
        return ToWPFColor(byte.Parse(colorAlpha), byte.Parse(colorRed), byte.Parse(colorGreen), byte.Parse(colorBlue));
    }
}