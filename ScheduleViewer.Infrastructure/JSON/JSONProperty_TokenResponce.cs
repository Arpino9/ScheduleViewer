namespace ScheduleViewer.Infrastructure.JSON;

/// <summary>
/// トークンレスポンスを受け取るためのクラス
/// </summary>
public record class JSONProperty_TokenResponce
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }

    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }

    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonProperty("token_type")]
    public string TokenType { get; set; }
}
