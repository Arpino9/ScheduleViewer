namespace ScheduleViewer.Domain.Entities;

/// <summary>
/// Entity - 写真データ
/// </summary>
public sealed class PhotoEntity
{
    /// <summary>
    /// 写真のメタデータを初期化します。
    /// </summary>
    /// <param name="id">写真ID。</param>
    /// <param name="date">撮影日時。</param>
    /// <param name="fileName">ファイル名。</param>
    /// <param name="description">説明。</param>
    /// <param name="imageUrl">表示用画像のURL。</param>
    /// <param name="url">写真ページのURL。</param>
    /// <param name="mimeType">MIMEタイプ。</param>
    /// <param name="height">画像の高さ。</param>
    /// <param name="width">画像の幅。</param>
    public PhotoEntity(
        string id, 
        DateTime date, 
        string fileName,
        string description,
        string imageUrl,
        string url, 
        string mimeType, 
        long height,
        long width)
    {
        ID          = id;
        Date        = date;
        FileName    = fileName;
        Description = description;
        ImageUrl    = imageUrl;
        URL         = url;
        MimeType    = mimeType;
        Height      = height;
        Width       = width;
    }

    /// <summary> ID </summary>
    public string ID { get; }

    /// <summary> 日付 </summary>
    public DateTime Date { get; }
    
    /// <summary> ファイル名 </summary>
    public string FileName { get; }
    
    /// <summary> 説明 </summary>
    public string Description { get; }

    /// <summary> 表示用画像のURL </summary>
    public string ImageUrl { get; }

    /// <summary> URL </summary>
    public string URL { get; }

    /// <summary> MIMEタイプ </summary>
    public string MimeType { get; }
    
    /// <summary> 高さ </summary>
    public long Height { get; }
 
    /// <summary> 幅 </summary>
    public long Width { get; }
}
