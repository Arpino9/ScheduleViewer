namespace ScheduleViewer.Infrastructure.Google_MapsPlace;

/// <summary>
/// Google Maps Places 読込
/// </summary>
internal sealed class PlaceReader : GoogleServiceBase<MapsPlacesService>
{
    /// <summary> 
    /// 初期化子 
    /// </summary>
    protected override MapsPlacesService Initializer
    {
        get => base.Initialize_OAuth(initializer => new MapsPlacesService(initializer),
                                     new[] { MapsPlacesService.Scope.CloudPlatform },
                                     "token_Place");
    }   

    /// <summary>
    /// 住所から地点情報(緯度、経度)を取得する
    /// </summary>
    /// <param name="address">住所</param>
    /// <returns>地点情報(緯度、経度)</returns>
    internal (double? Latitude, double? Longitude) ReadPlaceLocation(string address)
    {
        var place_id = JSONExtension.GetPlaceId(address);

        if (string.IsNullOrEmpty(place_id))
        {
            return (double.MinValue, double.MinValue);
        }

        MapsPlacesService request = Initializer;

        // FieldMaskを設定
        var fieldMaskHeaderValue = "displayName,id,location,photos"; // 必要なフィールドをカンマ区切りで指定
        request.HttpClient.DefaultRequestHeaders.Add("X-Goog-FieldMask", fieldMaskHeaderValue);

        var placeDetailsRequest = request.Places.Get($"places/{place_id}");
        placeDetailsRequest.LanguageCode = "ja";
        var placeDetails = placeDetailsRequest.Execute();

        return (placeDetails.Location.Latitude, placeDetails.Location.Longitude);        
    }
}
