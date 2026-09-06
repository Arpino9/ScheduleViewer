namespace ScheduleViewer.WPF.Helpers;

/// <summary>
/// Bitmap画像を扱うためのユーティリティです。
/// </summary>
public static class BitmapUtils
{
    /// <summary>
    /// 指定した画像URLでBitmap画像を初期化します。
    /// </summary>
    /// <param name="bitmap">初期化対象のBitmap画像。</param>
    /// <param name="imageUrl">画像のURL。</param>
    /// <returns>初期化したBitmap画像。</returns>
    public static BitmapImage Initialize(this BitmapImage bitmap, string imageUrl)
    {
        bitmap.BeginInit();
        bitmap.UriSource = new Uri(imageUrl);
        bitmap.EndInit();

        return bitmap;
    }

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
}
