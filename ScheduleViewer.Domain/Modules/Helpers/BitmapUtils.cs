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
}
