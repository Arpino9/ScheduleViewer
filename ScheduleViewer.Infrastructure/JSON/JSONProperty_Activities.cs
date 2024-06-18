namespace ScheduleViewer.Infrastructure.JSON;

/// <summary>
/// JSON属性 - Google Fitnessのアクティビティ
/// </summary>
public record class JSONProperty_Activities
{
    /// <summary> ID </summary>
    [JsonProperty("id")]
    public int ID { get; set; }

    /// <summary> 名称 </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary> 和名 </summary>
    [JsonProperty("japanese_name")]
    public string JapaneseName { get; set; }
}
