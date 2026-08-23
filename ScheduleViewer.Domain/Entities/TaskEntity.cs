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
        DateTime? completed,
        DateTime? dueDate)
    {
        TaskListName = taskListName;
        TaskName     = taskName;
        Details      = details;
        Completed    = completed;
        DueDate      = dueDate;
    }

    /// <summary> ToDoリスト名 </summary>
    public string TaskListName { get; }

    /// <summary> タスク名 </summary>
    public string TaskName { get; }

    /// <summary> 詳細 </summary>
    public string Details { get; }

    /// <summary> 完了日時。未完了の場合はnull。 </summary>
    public DateTime? Completed { get; }

    /// <summary> 期日。設定されていない場合はnull。 </summary>
    public DateTime? DueDate { get; }
}
