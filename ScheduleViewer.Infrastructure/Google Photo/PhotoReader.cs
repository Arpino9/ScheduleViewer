namespace ScheduleViewer.Infrastructure.Google_Photo;

/// <summary>
/// Entity - 写真
/// </summary>
internal class PhotoReader : GoogleServiceBase<PhotosLibraryService>
{
    /// <summary> 
    /// 初期化子 
    /// </summary>
    protected override PhotosLibraryService Initializer
    {
        get => base.Initialize_OAuth(initializer => new PhotosLibraryService(initializer),
                                     new[] { PhotosLibraryService.Scope.PhotoslibraryReadonly },
                                     "token_Photos");
    }
    
    /// <summary> 写真データ </summary>
    internal List<PhotoEntity> Photos { get; set; } = new List<PhotoEntity>();

    //TODO: Entityつくる
    /// <summary> アルバム </summary>
    internal List<Album> Albums { get; set; } = new List<Album>();

    /// <summary>
    /// 初期化
    /// </summary>
    internal async Task InitializeAsync()
    {
        await Task.WhenAll(
            GetAllPhotosAsync(),
            GetAllAlbumsAsync());
    }

    /// <summary>
    /// 全写真取得
    /// </summary>
    /// <remarks>
    /// 登録されている全ての写真を取得する
    /// </remarks>
    private async Task GetAllPhotosAsync()
    {
        string nextPageToken = null;

        do
        {
            var request = Initializer.MediaItems.Search(new SearchMediaItemsRequest
            {
                PageSize  = 100,
                PageToken = nextPageToken
            });

            var response = await request.ExecuteAsync();

            if (response.MediaItems is null)
            {
                // 写真が不正
                return;
            }

            if (response.MediaItems.IsEmpty())
            {
                // 写真が不正
                return;
            }

            foreach (var item in response.MediaItems)
            {
                // サイズ
                var height = item.MediaMetadata.Height.Value;
                var width  = item.MediaMetadata.Width.Value;

                // 画像
                var imageUrl = $"{item.BaseUrl}=w{width}-h{height}";
                var bitmap   = new BitmapImage().Initialize(imageUrl);

                Photos.Add(new PhotoEntity(item.Id,
                                          (DateTime)item.MediaMetadata.CreationTime,
                                          item.Filename,
                                          item.Description,
                                          bitmap,
                                          item.ProductUrl,
                                          item.MimeType,
                                          height,
                                          width));
            }

            nextPageToken = response.NextPageToken;
        } while (!string.IsNullOrEmpty(nextPageToken));
    }

    /// <summary>
    /// 全アルバム取得
    /// </summary>
    /// <remarks>
    /// 登録されている全てのアルバムを取得する
    /// </remarks>
    private async Task GetAllAlbumsAsync()
    {
        var albums = await Initializer.Albums.List().ExecuteAsync();

        if (albums.Albums is null)
        {
            // アルバムなし
            return;
        }

        if (albums.Albums.IsEmpty())
        {
            // アルバムに写真の登録なし
            return;
        }

        foreach (var album in albums.Albums)
        {
            Albums.Add(album);
            Console.WriteLine($"{album.Title} ({album.Id})");
        }
    }

    /// <summary>
    /// 日付で検索
    /// </summary>
    /// <param name="date">日付</param>
    /// <returns>写真データ</returns>
    /// <remarks>
    /// 写真が登録されていれば、日付と一致する写真を取り出す。
    /// </remarks>
    internal List<PhotoEntity> FindPhotosByDate(DateTime date)
        => Photos.Any() ?
           Photos.Where(x => x.Date.Year  == date.Year &&
                             x.Date.Month == date.Month &&
                             x.Date.Day   == date.Day).ToList() :
           new List<PhotoEntity>();
}
