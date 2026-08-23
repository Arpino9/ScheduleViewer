namespace ScheduleViewer.Web.Models;

public sealed class TaskDto
{
    public string TaskListName { get; set; } = string.Empty;
    public string TaskName { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public DateTime? Completed { get; set; }
    public DateTime? DueDate { get; set; }
}
