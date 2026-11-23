namespace ScheduleViewer.WPF.Helpers;

/// <summary>
/// Bitmap画像を扱うためのユーティリティです。
/// </summary>
public static class BitmapUtils
{
    /// <summary>
    /// URLをBitmap画像に変換します。
    /// </summary>
    /// <param name="url">画像のURL。</param>
    /// <returns>変換したBitmap画像。URLが空の場合は空のBitmap画像。</returns>
    /// <remarks>
    /// <see cref="BitmapCacheOption.OnLoad"/>を使用し、キャッシュの影響を最小化します。
    /// </remarks>
    public static BitmapImage ConvertFromURL(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return new BitmapImage();
        }

        var bitmap = new BitmapImage();

        bitmap.BeginInit();
        bitmap.UriSource = new Uri(url, UriKind.Absolute);
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.EndInit();

        return bitmap;
    }

    /// <summary>
    /// 画像のバイト列をBitmap画像に変換します。
    /// </summary>
    /// <param name="imageBytes">画像のバイト列。</param>
    /// <returns>変換したBitmap画像。バイト列が空の場合は空のBitmap画像。</returns>
    public static BitmapImage ConvertFromBytes(byte[] imageBytes)
    {
        if (imageBytes is null || imageBytes.Length == 0)
        {
            return new BitmapImage();
        }

        using var stream = new MemoryStream(imageBytes);
        var bitmap = new BitmapImage();

        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.StreamSource = stream;
        bitmap.EndInit();

        return bitmap;
    }
}
