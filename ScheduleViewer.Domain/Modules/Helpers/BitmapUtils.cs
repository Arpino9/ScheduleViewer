namespace ScheduleViewer.Domain.Modules.Helpers;

/// <summary>
/// Utility - Bitmap
/// </summary>
public static class BitmapUtils
{
    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="imageUrl">画像Uri</param>
    /// <returns>bmp</returns>
    public static BitmapImage Initialize(this BitmapImage bitmap, string imageUrl)
    {
        bitmap.BeginInit();
        bitmap.UriSource = new Uri(imageUrl);
        bitmap.EndInit();

        return bitmap;
    }

    /// <summary>
    /// URLをBitmapに変換する
    /// </summary>
    /// <param name="url">URL</param>
    /// <returns>画像</returns>
    /// <remarks>
    /// 「BitmapCacheOption.OnLoad」にすることで、キャッシュの影響を最小化している。
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
