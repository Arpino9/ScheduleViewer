namespace ScheduleViewer.Infrastructure.JSON;

/// <summary>
/// Google Placesから取得した写真データを表します。
/// </summary>
/// <param name="ImageBytes">画像のバイト列。</param>
/// <param name="Height">画像の高さ。</param>
/// <param name="Width">画像の幅。</param>
public sealed record PlacePhotoData(byte[] ImageBytes, double Height, double Width)
{
    /// <summary>
    /// 写真が取得できなかった場合の空データを取得します。
    /// </summary>
    public static PlacePhotoData Empty { get; } = new(Array.Empty<byte>(), 0, 0);
}
