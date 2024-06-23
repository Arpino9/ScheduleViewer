namespace ScheduleViewer.Domain.Entities;

/// <summary>
/// Entity - 写真データ
/// </summary>
public sealed class PhotoEntity
{
    public PhotoEntity(
        string id, 
        DateTime date, 
        string fileName,
        string description,
        BitmapImage image,
        string url, 
        string mimeType, 
        long height,
        long width)
    {
        ID          = id;
        Date        = date;
        FileName    = fileName;
        Description = description;
        Image       = image;
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

    /// <summary> 画像 </summary>
    public BitmapImage Image { get; }

    /// <summary> URL </summary>
    public string URL { get; }

    /// <summary> MIMEタイプ </summary>
    public string MimeType { get; }
    
    /// <summary> 高さ </summary>
    public long Height { get; }
 
    /// <summary> 幅 </summary>
    public long Width { get; }
}
