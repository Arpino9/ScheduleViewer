namespace ScheduleViewer.Web.Models;

internal sealed class FitbitSleepDto
{
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public string Sleeping { get; set; } = "PT0S";
    public string Awake { get; set; } = "PT0S";
    public string Restless { get; set; } = "PT0S";
    public string Rem { get; set; } = "PT0S";
    public string Asleep { get; set; } = "PT0S";
}
