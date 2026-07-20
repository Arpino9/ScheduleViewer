using System.Text.Json;

namespace ScheduleViewer.Infrastructure.Nominatim;

internal class NominatimReader
{
    internal async Task<string> GetMapImageFromAddressAsync(string address)
    {
        var n = this.GetTownAreaAsync("東京都渋谷区神南1-19-11");
        
        using var http = new HttpClient(); 

        // 1. 住所 → 緯度経度（Nominatim）
        var geoUrl = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(address)}&format=json";
        http.DefaultRequestHeaders.UserAgent.ParseAdd("YourAppNameHere"); // Nominatimに必要
        var geoJson = await http.GetStringAsync(geoUrl);
        var geoResults = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(geoJson);
        var lat = geoResults[0].GetProperty("lat").GetString();
        var lon = geoResults[0].GetProperty("lon").GetString();

        // 2. 緯度経度 → 画像（MapQuest）
        var position = LatLonToTile(double.Parse(lat), double.Parse(lon), 14);

        var url = $"https://tile.openstreetmap.org/14/{position.x}/{position.y}.png";

        return url;
    }

    public async Task<string> GetTownAreaAsync(string address)
    {
        using var http = new HttpClient();
        http.DefaultRequestHeaders.UserAgent.ParseAdd("YourAppNameHere");

        string url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(address)}&format=json&addressdetails=1&limit=1";
        string json = await http.GetStringAsync(url);

        var doc = JsonDocument.Parse(json);
        var addr = doc.RootElement[0].GetProperty("address");

        string prefecture = addr.GetProperty("state").GetString();
        string city = addr.TryGetProperty("city", out var c) ? c.GetString() :
                      addr.TryGetProperty("town", out var t) ? t.GetString() :
                      addr.TryGetProperty("village", out var v) ? v.GetString() : null;
        string suburb = addr.TryGetProperty("suburb", out var s) ? s.GetString() : null;

        return $"{prefecture}{city}{suburb}";
    }

    // 緯度・経度 → タイル座標（x, y）
    public (int x, int y) LatLonToTile(double lat, double lon, int zoom)
    {
        int x = (int)Math.Floor((lon + 180.0) / 360.0 * Math.Pow(2, zoom));
        int y = (int)Math.Floor((1.0 - Math.Log(Math.Tan(lat * Math.PI / 180.0) + 1.0 / Math.Cos(lat * Math.PI / 180.0)) / Math.PI) / 2.0 * Math.Pow(2, zoom));
        return (x, y);
    }
}
