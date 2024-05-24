namespace ScheduleViewer.Domain.Entities;

/// <summary>
/// Entity - 本
/// </summary>
public sealed class BookEntity
{
    public BookEntity(
        string title,
        DateTime readDate,
        string author, 
        string publisher, 
        string releasedDate,
        string type,
        string isbn_10, 
        string isbn_13, 
        string caption, 
        string rating)
    {
        Title        = title;
        ReadDate     = readDate;
        Author       = author;
        Publisher    = publisher;
        ReleasedDate = releasedDate;
        Type         = type;
        ISBN_10      = isbn_10;
        ISBN_13      = isbn_13;
        Caption      = caption;
        Rating       = rating;
    }

    /// <summary> 著者 </summary>
    public string Author { get; }

    public DateTime ReadDate { get; }

    /// <summary> 出版社 </summary>
    public string Publisher { get; }

    /// <summary> 詳細情報 </summary>
    public string Title { get; }

    /// <summary> 発売日 </summary>
    public string ReleasedDate { get; }

    /// <summary> 本の種類 </summary>
    public string Type { get; }

    /// <summary> ISBN-10 </summary>
    public string ISBN_10 { get; }

    /// <summary> ISBN-13 </summary>
    public string ISBN_13 { get; }

    /// <summary> 本の概要 </summary>
    public string Caption { get; }

    /// <summary> 本の評価 </summary>
    public string Rating { get; }
}
