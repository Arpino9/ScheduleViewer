namespace ScheduleViewer.WPF.Models;

public sealed class Model_ScheduleDetails_Book : ModelBase, IViewer
{
    #region Get Instance

    private static Model_ScheduleDetails_Book model = null;

    public static Model_ScheduleDetails_Book GetInstance()
    {
        if (model == null)
        {
            model = new Model_ScheduleDetails_Book();
        }

        return model;
    }

    internal DateTime Date { get; set; }

    internal override void Initialize()
    {
        var events = CalendarReader.FindByDate(Date);

        if (events.IsEmpty())
        {
            return;
        }

        var books = ConvertToBookEntities(events);

        this.ViewModel.Books_ItemSource = books.ToReactiveCollection();
    }

    /// <summary> 有効な都道府県名 </summary>
    private Regex ValidBookType = new Regex(@".*コミック|.*文庫|.*単行本|.*新書|.*大型本|.*電子書籍|.*ペーパーバック");

    /// <summary>
    /// エンティティに変換
    /// </summary>
    /// <param name="events">イベント</param>
    /// <returns>エンティティ</returns>
    private List<BookEntity> ConvertToBookEntities(IReadOnlyList<CalendarEventsEntity> events)
    {
        var books = events.Where(x => x.Description != null &&
                                      x.Description.Contains("【出版社】"));

        var entities = new List<BookEntity>();

        foreach(var book in books) 
        {
            var elements = book.Description.Split('\n');

            var title     = book.Title;
            var readDate  = book.StartDate;
            var author    = this.GetAuthor(book);
            var publisher = this.GetPublisher(book);
            var details   = this.GetDetails(book);
            var caption   = this.GetCaption(book);

            entities.Add(new BookEntity(title, readDate, author, publisher,  
                                        details.ReleasedDate, details.Type, 
                                        details.Isbn_10, details.Isbn_13, 
                                        caption, details.Rating));
        }

        return entities;
    }

    /// <summary>
    /// インデックスを取得する
    /// </summary>
    /// <param name="elements">本情報</param>
    /// <param name="name">検索するインデックスの名称</param>
    /// <returns></returns>
    private int FindIndex(string[] elements, string name)
        => elements.Select((t, i) => new { Text = t, Index = i })
                   .Where(x => x.Text.StartsWith(name)).First().Index;

    /// <summary>
    /// 本情報を分割する
    /// </summary>
    /// <param name="book">本情報</param>
    /// <returns>本情報</returns>
    private string[] DivideByElements(CalendarEventsEntity book)
        => book.Description.Split('\n');

    /// <summary>
    /// 著者を取得
    /// </summary>
    /// <param name="book">本情報</param>
    /// <returns>著者</returns>
    private string GetAuthor(CalendarEventsEntity book)
    {
        var elements = DivideByElements(book);

        return elements[FindIndex(elements, "【著者】") + 1];
    }

    /// <summary>
    /// 出版社を取得
    /// </summary>
    /// <param name="book">本情報</param>
    /// <returns>出版社</returns>
    private string GetPublisher(CalendarEventsEntity book)
    {
        var elements = DivideByElements(book);

        return elements[FindIndex(elements, "【出版社】") + 1];
    }

    /// <summary>
    /// 詳細情報を取得
    /// </summary>
    /// <param name="book">本情報</param>
    /// <returns>詳細情報</returns>
    private (string ReleasedDate, string Type, string Isbn_10, string Isbn_13, string Rating) GetDetails(CalendarEventsEntity book)
    {
        var elements = DivideByElements(book);

        var detailsIndex = FindIndex(elements, "【出版社】");
        var captionIndex = FindIndex(elements, "【本の概要】");

        var type         = string.Empty;
        var releasedDate = string.Empty;
        var isbn_10      = string.Empty;
        var isbn_13      = string.Empty;

        for (var i = 1; (detailsIndex + i) < captionIndex; i++)
        {
            if (elements[detailsIndex + i].Contains("発売日"))
            {
                releasedDate = Substring();
            }
            else if (ValidBookType.IsMatch(elements[detailsIndex + i]))
            {
                type = Substring();
            }
            else if (elements[detailsIndex + i].Contains("ISBN-10"))
            {
                isbn_10 = Substring();
            }
            else if (elements[detailsIndex + i].Contains("ISBN-13"))
            {
                isbn_13 = Substring();
            }

            // 項目名を除外する
            string Substring()
                => elements[detailsIndex + i].Substring(elements[detailsIndex + i]
                                             .IndexOf(" : ") + 3);
        }

        var rating = elements[this.FindIndex(elements, "【本の評価】") + 1];

        return (releasedDate, type, isbn_10, isbn_13, rating);
    }

    /// <summary>
    /// 概要を取得
    /// </summary>
    /// <param name="book">本情報</param>
    /// <returns>概要</returns>
    private string GetCaption(CalendarEventsEntity book)
    {
        var elements = DivideByElements(book);

        var captionIndex    = FindIndex(elements, "【本の概要】");
        var evaluationIndex = FindIndex(elements, "【本の評価】");

        var caption = string.Empty;

        for (var i = 1; (captionIndex + i) < evaluationIndex; i++)
        {
            caption += elements[captionIndex + i] + "\n";
        }

        return caption;
    }

    /// <summary>
    /// リストビュー - SelectionChanged
    /// </summary>
    public void ListView_SelectionChanged()
    {
        if (this.ViewModel.Books_SelectedIndex.Value.IsUnSelected())
        {
            return;
        }

        var entity = this.ViewModel.Books_ItemSource[this.ViewModel.Books_SelectedIndex.Value];

        // タイトル
        this.ViewModel.Title_Text.Value        = entity.Title;
        // 日付
        this.ViewModel.ReadDate_Text.Value     = entity.ReadDate;
        // 著者 / 作者
        this.ViewModel.Author_Text.Value       = entity.Author;
        // 出版社
        this.ViewModel.Publisher_Text.Value    = entity.Publisher;
        // 発売日
        this.ViewModel.ReleasedDate_Text.Value = entity.ReleasedDate;
        // 本の種類
        this.ViewModel.Type_Text.Value         = entity.Type;
        // ISBN-10
        this.ViewModel.ISBN_10_Text.Value      = entity.ISBN_10;
        // ISBN-13
        this.ViewModel.ISBN_13_Text.Value      = entity.ISBN_13;
        // 概要
        this.ViewModel.Caption_Text.Value      = entity.Caption;
        // 評価
        this.ViewModel.Rating_Text.Value       = entity.Rating;
    }

    #endregion

    /// <summary> ViewModel - スケジュール詳細 (本一覧) </summary>
    public ViewModel_ScheduleDetails_Book ViewModel { get; set; }
}
