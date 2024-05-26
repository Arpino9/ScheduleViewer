namespace ScheduleViewer.Domain.Entities;

/// <summary>
/// Entity - ToDoリスト
/// </summary>
public sealed class TaskEntity
{
    public TaskEntity(
        string taskListName,
        string taskName,
        string details,
        DateTime completed,
        DateTime dueDate)
    {
        TaskListName = taskListName;
        TaskName     = taskName;
        Details      = details;
        Completed    = completed;
        DueDate      = dueDate;
    }

    /// <summary> ToDoリスト名 </summary>
    public string TaskListName { get; set; }

    /// <summary> タスク名 </summary>
    public string TaskName { get; set; }

    /// <summary> 詳細 </summary>
    public string Details { get; set; }

    /// <summary> 完了有無 </summary>
    public DateTime Completed { get; set; }

    /// <summary> 完了日付 </summary>
    public DateTime DueDate { get; set; }
}
