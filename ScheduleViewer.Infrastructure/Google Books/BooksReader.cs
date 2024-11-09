using System.Reflection;

namespace ScheduleViewer.Infrastructure.Google_Books;

/// <summary>
/// Google Books 読込
/// </summary>
internal class BooksReader : GoogleServiceBase<BooksService>
{
    /// <summary> クラス名 </summary>
    private static string ClassName => MethodBase.GetCurrentMethod().DeclaringType.Name;

    /// <summary> 
    /// 初期化子
    /// </summary>
    protected override BooksService Initializer
    {
        get => base.Initialize_OAuth(initializer => new BooksService(initializer),
                                     new[] { BooksService.Scope.Books },
                                     "token_Books");
    }

    /// <summary>
    /// タイトル検索
    /// </summary>
    /// <param name="title">タイトル</param>
    internal void FindByTitle(string title)
        => this.FindByTitle(title, string.Empty, DateTime.Now);

    /// <summary>
    /// タイトル検索
    /// </summary>
    /// <param name="title">タイトル</param>
    /// <param name="thumbnail">サムネイル</param>
    /// <param name="readDate">読了日</param>
    /// <returns>void</returns>
    internal async Task FindByTitle(string title, string thumbnail, DateTime readDate)
    {
        var books = Initializer.Volumes.List(title).Execute();

        if (books.Items.IsEmpty())
        {
            // 該当なし
            return;
        }

        var book = books.Items.FirstOrDefault();

        var author       = book.VolumeInfo.Authors?.FirstOrDefault();
        var publisher    = book.VolumeInfo.Publisher?.FirstOrDefault().ToString();
        var releasedDate = book.VolumeInfo.PublishedDate?.ToString();
        var type         = book.VolumeInfo.Categories?.FirstOrDefault();
        var caption      = book.VolumeInfo.Description;
        var rating       = book.VolumeInfo.RatingsCount?.ToString();

        var isbn = await GetIsbnCode(book.Id);

        var a = new BookEntity(title, readDate, author, publisher, releasedDate, type, string.Empty, string.Empty, caption, thumbnail, rating);

        /*foreach (var book in books.Items)
        {
            Console.WriteLine($"タイトル: {book.VolumeInfo.Title}");
            Console.WriteLine($"著者: {book.VolumeInfo.Authors?.FirstOrDefault()}");
            Console.WriteLine($"説明: {book.VolumeInfo.Description}");
            Console.WriteLine("--------------------");
        }*/
    }

    private async Task<string> GetIsbnCode(string volumeId)
    {
        // volumes.get メソッドを使用して書籍の詳細を取得
        var detailRequest = Initializer.Volumes.Get(volumeId);
        var detailVolume = detailRequest.Execute();

        // ISBNを取得
        var isbn = detailVolume.VolumeInfo.IndustryIdentifiers?.FirstOrDefault(x => x.Type == "ISBN_13")?.Identifier;
        if (isbn != null)
        {
            LogUtils.Info(ClassName, $"ISBN: {isbn}");
        }

        return string.Empty;
    }
}
