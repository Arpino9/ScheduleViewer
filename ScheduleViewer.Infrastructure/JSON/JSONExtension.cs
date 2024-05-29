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
            //TODO; 外部ファイルに入れる
            string apiKey = "AIzaSyB6P0iD888ZjeV8wMFPY9PWTTPXS1TkiqI";

            // Google Places APIのURLを構築
            string apiUrl = $"https://maps.googleapis.com/maps/api/place/findplacefromtext/json?input={Uri.EscapeDataString(address)}&inputtype=textquery&fields=place_id&key={apiKey}";

            // Webリクエストを送信してAPIからJSONデータを取得
            WebClient webClient = new WebClient();
            string jsonResponse = webClient.DownloadString(apiUrl);

            // JSONデータからPlace IDを抽出
            JObject jsonObject = JObject.Parse(jsonResponse);
            string placeId = jsonObject["candidates"][0]["place_id"].ToString();

            return placeId;
        }
        catch (Exception ex)
        {
            //throw new FileReaderException("Google Places APIの読み込みに失敗しました。", ex);
            return null;
        }
    }
}