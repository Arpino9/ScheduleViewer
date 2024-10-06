namespace ScheduleViewer.Infrastructure.JSON;

/// <summary>
/// JSON Writer
/// </summary>
public static class JSONExtension
{
    /// <summary>
    /// Jsonファイルへの出力
    /// </summary>
    /// <typeparam name="T">データ型</typeparam>
    /// <param name="obj">オブジェクト</param>
    /// <param name="path">ファイルパス</param>
    public static void SerializeToFile<T>(this T obj, string path)
    {
        try
        {
            using (var sw = new StreamWriter(path, false, System.Text.Encoding.UTF8))
            {
                // JSON データにシリアライズ
                var jsonData = JsonConvert.SerializeObject(obj, Formatting.Indented);

                // JSON データをファイルに書き込み
                sw.Write(jsonData);
            }
        }
        catch (Exception ex)
        {
            throw new FileWriterException("JSONの書き込みに失敗しました。", ex);
        }
    }

    /// <summary>
    /// Jsonファイルの読み込み
    /// </summary>
    /// <param name="path">ファイルパス</param>
    /// <returns>Jsonデータ</returns>
    /// <exception cref="FileReaderException">読み込み失敗</exception>
    public static T DeserializeSettings<T>(string path)
    {
        try
        {
            using (var sr = new StreamReader(path, Encoding.UTF8))
            {
                // 変数 jsonReadData にファイルの内容を代入 
                var jsonReadData = sr.ReadToEnd();

                // デシリアライズして person にセット
                return JsonConvert.DeserializeObject<T>(jsonReadData);
            }
        }
        catch (Exception ex)
        {
            throw new FileReaderException("JSONの読み込みに失敗しました。", ex);
        }
    }

    /// <summary>
    /// Google Places APIのPlace ID取得
    /// </summary>
    /// <param name="address">住所</param>
    /// <returns></returns>
    /// <exception cref="FileReaderException">読み込み失敗</exception>
    /// <remarks>
    /// 任意の住所を読み込み、Place IDを返す。
    /// Google Places APIのfindplacefromtextエンドポイントを使用して、指定された場所のPlace IDを取得している。
    /// </remarks>
    public static string GetPlaceId(string address)
    {
        if (address == string.Empty)
        {
            return string.Empty;
        }

        try
        {
            // Google Places APIのURLを構築
            string apiUrl = $"https://maps.googleapis.com/maps/api/place/findplacefromtext/json?input={Uri.EscapeDataString(address)}&inputtype=textquery&fields=place_id&key={Shared.API_Key}";

            // Webリクエストを送信してAPIからJSONデータを取得
            WebClient webClient = new WebClient();
            string jsonResponse = webClient.DownloadString(apiUrl);

            // JSONデータからPlace IDを抽出
            JObject jsonObject = JObject.Parse(jsonResponse);

            if (jsonObject["candidates"].IsEmpty())
            {
                // 地図情報が未登録 or 要課金
                return string.Empty;
            }

            string placeId = jsonObject["candidates"][0]["place_id"].ToString();

            return placeId;
        }
        catch (Exception ex)
        {
            //throw new FileReaderException("Google Places APIの読み込みに失敗しました。", ex);
            return string.Empty;
        }
    }

    /// <summary>
    /// 写真のイメージデータを取得
    /// </summary>
    /// <param name="address">住所</param>
    /// <returns>イメージデータ(イメージ, 高さ, 幅)</returns>
    public static (BitmapImage Image, double Height, double Width) GetPhotoSource(string address)
    {
        var placeDetails = GetPlaceDetails(address);

        return ShowPhotos(placeDetails);
    }

    /// <summary>
    /// 住所から写真データを取得
    /// </summary>
    /// <param name="address">住所</param>
    /// <returns>写真データ</returns>
    private static JObject GetPlaceDetails(string address)
    {
        var placeId = GetPlaceId(address);

        try
        {
            // Google Places APIのURLを構築
            string apiUrl = $"https://maps.googleapis.com/maps/api/place/details/json?place_id={placeId}&fields=photos&key={Shared.API_Key}";

            // Webリクエストを送信してAPIからJSONデータを取得
            WebClient webClient = new WebClient();
            string jsonResponse = webClient.DownloadString(apiUrl);

            // JSONデータを解析してJObjectを返す
            JObject jsonObject = JObject.Parse(jsonResponse);
            return jsonObject;
        }
        catch (Exception ex)
        {
            //MessageBox.Show($"Error: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 写真を表示
    /// </summary>
    /// <param name="placeDetails">写真データ</param>
    /// <returns>イメージデータ(イメージ, 高さ, 幅)</returns>
    private static (BitmapImage Image, double Height, double Width) ShowPhotos(JObject placeDetails)
    {
        BitmapImage bitmapImage = new BitmapImage();
        var height = default(double);
        var width  = default(double);

        if (placeDetails["result"] is null)
        {
            return (null, 0, 0);
        }

        try
        {
            // photos情報を取得
            JArray photosArray = (JArray)placeDetails["result"]["photos"];
            if (photosArray != null && photosArray.Count > 0)
            {
                // 最初の写真を取得
                JObject firstPhoto = (JObject)photosArray[0];
                string photoReference = firstPhoto["photo_reference"].ToString();
                height = double.Parse(firstPhoto["height"].ToString());
                width  = double.Parse(firstPhoto["width"].ToString());

                // Google Places APIのURLを構築して写真を取得
                string imageUrl = $"https://maps.googleapis.com/maps/api/place/photo?maxwidth=400&photoreference={photoReference}&key={Shared.API_Key}";

                // Webリクエストを送信して写真を取得
                WebClient webClient = new WebClient();
                byte[] imageBytes = webClient.DownloadData(imageUrl);

                // 画像をBitmapImageに変換
                
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(imageBytes);
                bitmapImage.EndInit();                
            }

            // Imageコントロールに画像を表示
            return (bitmapImage, height, width);
        }
        catch (Exception ex)
        {
            //MessageBox.Show($"Error: {ex.Message}");
            return (null, 0, 0);
        }
    }
}