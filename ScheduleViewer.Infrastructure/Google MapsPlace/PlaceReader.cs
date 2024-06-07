namespace ScheduleViewer.Infrastructure.Google_MapsPlace;

/// <summary>
/// Google Maps Places 読込
/// </summary>
public sealed class PlaceReader
{
    /// <summary> 初期化 </summary>
    public static MapsPlacesService Initializer => GoogleService<MapsPlacesService>.Initialize(
                                                   initializer => new MapsPlacesService(initializer),
                                                   MapsPlacesService.Scope.CloudPlatform,
                                                   "token_Place");

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

        MapsPlacesService request = PlaceReader.Initializer;

        // FieldMaskを設定
        var fieldMaskHeaderValue = "displayName,id,location,photos"; // 必要なフィールドをカンマ区切りで指定
        request.HttpClient.DefaultRequestHeaders.Add("X-Goog-FieldMask", fieldMaskHeaderValue);

        var placeDetailsRequest = request.Places.Get($"places/{place_id}");
        placeDetailsRequest.LanguageCode = "ja";
        var placeDetails = placeDetailsRequest.Execute();

        return (placeDetails.Location.Latitude, placeDetails.Location.Longitude);        
    }
}
