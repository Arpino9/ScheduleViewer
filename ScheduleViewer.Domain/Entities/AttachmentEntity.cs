namespace ScheduleViewer.Domain.Entities;

/// <summary>
/// Entity - 添付ファイル
/// </summary>
public sealed class AttachmentEntity
{
    public AttachmentEntity(
        DateTime date,
        string title,
        string url,
        string minetype)
    {
        this.Date     = date;
        this.Title    = title;
        this.Url      = url;
        this.Minetype = minetype;
    }

    public DateTime Date { get; }

    /// <summary> タイトル </summary>
    public string Title { get; }
    
    /// <summary> URL </summary>
    public string Url { get; }
    
    /// <summary> ファイルタイプ </summary>
    public string Minetype { get; }
}
