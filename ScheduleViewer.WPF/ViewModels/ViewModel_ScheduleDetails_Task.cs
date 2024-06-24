namespace ScheduleViewer.WPF.ViewModels;

/// <summary>
/// Model - スケジュール詳細 (本一覧)
/// </summary>
public sealed class ViewModel_ScheduleDetails_Task : ViewModelBase<Model_ScheduleDetails_Task>
{
    public override event PropertyChangedEventHandler PropertyChanged;

    public ViewModel_ScheduleDetails_Task()
    {
        this.Model.ViewModel = this;
        this.Model.Initialize();

        this.BindEvents();
    }

    protected override void BindEvents()
    {
        Tasks_SelectionChanged.Subscribe(_ => this.Model.Clear_ViewForm());
        Tasks_SelectionChanged.Subscribe(_ => this.Model.ListView_SelectionChanged());
    }
    
    protected override Model_ScheduleDetails_Task Model => Model_ScheduleDetails_Task.GetInstance();

    /// <summary> タスク一覧 - ItemSource </summary>
    public ReactiveCollection<TaskEntity> Tasks_ItemSource { get; set; } = new ReactiveCollection<TaskEntity>();

    /// <summary> タスク一覧 - SelectedIndex </summary>
    public ReactiveProperty<int> Tasks_SelectedIndex { get; set; } = new ReactiveProperty<int>();

    /// <summary> 予定 - SelectionChanged </summary>
    public ReactiveCommand Tasks_SelectionChanged { get; private set; } = new ReactiveCommand();

    /// <summary> タスクリスト名 - Text </summary>
    public ReactiveProperty<string> TaskListName_Text { get; set; } = new ReactiveProperty<string>();

    /// <summary> タスク名 - Text </summary>
    public ReactiveProperty<string> TaskName_Text { get; set; } = new ReactiveProperty<string>();

    /// <summary> 詳細 - Text </summary>
    public ReactiveProperty<string> Details_Text { get; set; } = new ReactiveProperty<string>();
}
