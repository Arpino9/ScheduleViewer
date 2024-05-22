namespace ScheduleViewer.Infrastructure.JSON;

/// <summary>
/// JSON属性 - 休祝日マスタ用
/// </summary>
public record class JSONProperty_Holiday
{
    /// <summary> 休祝日の日付 </summary>
    [JsonProperty("date")]
    public DateTime Date { get; set; }

    /// <summary> 休祝日の名前 </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary> 会社名 </summary>
    [JsonProperty("companyName")]
    public string CompanyName { get; set; }

    /// <summary> 備考 </summary>
    [JsonProperty("remarks")]
    public string Remarks { get; set; }
}
