namespace ScheduleViewer.WPF.Models;

/// <summary>
/// Model - スケジュール詳細 (タスク一覧)
/// </summary>
public sealed class Model_ScheduleDetails_Task : ModelBase<ViewModel_ScheduleDetails_Task>, IViewer
{
    #region Get Instance

    private static Model_ScheduleDetails_Task model = null;

    public static Model_ScheduleDetails_Task GetInstance()
    {
        if (model == null)
        {
            model = new Model_ScheduleDetails_Task();
        }

        return model;
    }

    #endregion

    /// <summary> ViewModel - スケジュール詳細 </summary>
    public ViewModel_ScheduleDetails ViewModel_Header { get; set; }

    /// <summary> ViewModel </summary>
    internal override ViewModel_ScheduleDetails_Task ViewModel { get; set; }

    internal override void Initialize()
    {
        var tasks = TaskReader.FindByDate(this.ViewModel_Header.Date.Value);

        if (tasks.IsEmpty())
        {
            return;
        }

        this.ViewModel.Tasks_ItemSource.Clear();
        
        foreach(var task in tasks)
        {
            this.ViewModel.Tasks_ItemSource.Add(task);
        }
        
        this.ListView_SelectionChanged();
    }

    public void ListView_SelectionChanged()
    {
        if (this.ViewModel.Tasks_ItemSource.IsEmpty())
        {
            // リストが空
            return;
        }

        if (this.ViewModel.Tasks_SelectedIndex.Value.IsUnSelected())
        {
            // 未選択
            return;
        }

        var entity = this.ViewModel.Tasks_ItemSource[this.ViewModel.Tasks_SelectedIndex.Value];

        // タスクリスト名
        this.ViewModel.TaskListName_Text.Value = entity.TaskListName;
        // タスク名
        this.ViewModel.TaskName_Text.Value     = entity.TaskName;
        // 詳細
        this.ViewModel.Details_Text.Value      = entity.Details;
    }

    public void Clear_ViewForm()
    {
        // タスクリスト名
        this.ViewModel.TaskListName_Text.Value = string.Empty;
        // タスク名
        this.ViewModel.TaskName_Text.Value     = string.Empty;
        // 詳細
        this.ViewModel.Details_Text.Value      = string.Empty;
    }
}
