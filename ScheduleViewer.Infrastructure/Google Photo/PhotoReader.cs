﻿namespace ScheduleViewer.Infrastructure.Google_Photo;

/// <summary>
/// Entity - 写真
/// </summary>
public class PhotoReader
{
    /// <summary> 初期化 </summary>
    public static PhotosLibraryService Initializer => GoogleService<PhotosLibraryService>.Initialize_OAuth(
                                               initializer => new PhotosLibraryService(initializer),
                                               new[] { PhotosLibraryService.Scope.PhotoslibraryReadonly },
                                               "token_Photos");

    /// <summary> 写真データ </summary>
    public static List<PhotoEntity> Photos { get; set; } = new List<PhotoEntity>();

    //TODO: Entityつくる
    /// <summary> アルバム </summary>
    public static List<Album> Albums { get; set; } = new List<Album>();

    /// <summary>
    /// 初期化
    /// </summary>
    public static async Task Initialize()
    {
        await Task.WhenAll(
            GetAllPhotos(),
            GetAllAlbums());
    }

    /// <summary>
    /// 全写真取得
    /// </summary>
    /// <remarks>
    /// 登録されている全ての写真を取得する
    /// </remarks>
    private async static Task GetAllPhotos()
    {
        string nextPageToken = null;
        do
        {
            var request = Initializer.MediaItems.Search(new SearchMediaItemsRequest
            {
                PageSize = 100,
                PageToken = nextPageToken
            });
            var response = await request.ExecuteAsync();

            if (response.MediaItems != null && response.MediaItems.Count > 0)
            {
                foreach (var item in response.MediaItems)
                {
                    var height = item.MediaMetadata.Height.Value;
                    var width  = item.MediaMetadata.Width.Value;

                    //var imageUrl = $"{item.BaseUrl}=w2048-h1024";
                    var imageUrl = $"{item.BaseUrl}=w{width}-h{height}";

                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imageUrl);
                    bitmap.EndInit();

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
            }
            else
            {
                Console.WriteLine("No more media items found.");
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
    private async static Task GetAllAlbums()
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
    public static List<PhotoEntity> FindByDate(DateTime date)
        => Photos.Where(x => x.Date.Year  == date.Year &&
                             x.Date.Month == date.Month &&
                             x.Date.Day   == date.Day).ToList();
}
