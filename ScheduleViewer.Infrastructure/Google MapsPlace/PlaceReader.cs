namespace ScheduleViewer.Infrastructure.Google_MapsPlace;

/// <summary>
/// Google Maps Places 読込
/// </summary>
public sealed class PlaceReader
{
    /// <summary>
    /// 住所から地点情報(緯度、経度)を取得する
    /// </summary>
    /// <param name="address">住所</param>
    /// <returns>地点情報(緯度、経度)</returns>
    public static (double? Latitude, double? Longitude) ReadPlaceLocation(string address)
    {
        var place_id = JSONExtension.GetPlaceId(address);

        if (string.IsNullOrEmpty(place_id))
        {
            return (double.MinValue, double.MinValue);
        }

        MapsPlacesService request = PlaceReader.Initialize();

        // FieldMaskを設定
        var fieldMaskHeaderValue = "displayName,id,location,photos"; // 必要なフィールドをカンマ区切りで指定
        request.HttpClient.DefaultRequestHeaders.Add("X-Goog-FieldMask", fieldMaskHeaderValue);

        var placeDetailsRequest = request.Places.Get($"places/{place_id}");
        placeDetailsRequest.LanguageCode = "ja";
        var placeDetails = placeDetailsRequest.Execute();

        return (placeDetails.Location.Latitude, placeDetails.Location.Longitude);        
    }

    /// <summary>
    /// 初期化
    /// </summary>
    /// <returns>Tasks Service</returns>
    private static MapsPlacesService Initialize()
    {
        using (var stream = new FileStream(@"C:\Users\OKAJIMA\source\repos\SalaryManager\SalaryManager.Infrastructure\Google Calendar\\client_secret_732519433057-69j4ur5vdpca55vfscem296gesd5j16o.apps.googleusercontent.com.json", FileMode.Open, FileAccess.Read))
        {
            var secrets = GoogleClientSecrets.Load(stream).Secrets;
            var scope = MapsPlacesService.Scope.CloudPlatform;
            var dataStore = new FileDataStore("token_Place", true);

            // tokenを保存するディレクトリ
            string credPath = "token_Place";
            Task<UserCredential> credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                secrets,
                new[] { scope },
                "user", CancellationToken.None, new FileDataStore(credPath, true));  // 第二引数をtrueにすると、カレントディレクトリからの相対パスに保存される

            return new MapsPlacesService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential.Result,
                ApplicationName = "myApi"
            });
        }
    }
}
