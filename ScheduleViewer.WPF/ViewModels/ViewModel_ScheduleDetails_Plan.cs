namespace ScheduleViewer.WPF.ViewModels;

/// <summary>
/// ViewModel - スケジュール詳細 (予定一覧)
/// </summary>
public sealed class ViewModel_ScheduleDetails_Plan : ViewModelBase<Model_ScheduleDetails_Plan>
{        
    public override event PropertyChangedEventHandler PropertyChanged;

    public ViewModel_ScheduleDetails_Plan()
    {
        this.Model.ViewModel = this;

        this.Model.Initialize();

        BindEvents();
    }

    /// <summary> ViewModel - スケジュール詳細 (本一覧) </summary>
    public ViewModel_ScheduleDetails ViewModel_Header { get; set; }

    protected override Model_ScheduleDetails_Plan Model { get; } 
        = Model_ScheduleDetails_Plan.GetInstance();

    protected override void BindEvents()
    {
        Events_SelectionChanged.Subscribe(_ => this.Model.Clear_ViewForm());
        Events_SelectionChanged.Subscribe(_ => this.Model.ListView_SelectionChanged());
    }

    public ReactiveCollection<CalendarEventsEntity> Events_ItemSource { get; set; } = new ReactiveCollection<CalendarEventsEntity>();

    /// <summary> 予定 - SelectionChanged </summary>
    public ReactiveCommand Events_SelectionChanged { get; private set; } = new ReactiveCommand();

    /// <summary> 予定 - SelectedIndex </summary>
    public ReactiveProperty<int> Events_SelectedIndex { get; set; } = new ReactiveProperty<int>();

    /// <summary> タイトル - Text </summary>
    public ReactiveProperty<string> Title_Text { get; set; } = new ReactiveProperty<string>();

    /// <summary> 開始時刻 - Text </summary>
    public ReactiveProperty<string> StartTime_Text { get; set; } = new ReactiveProperty<string>();

    /// <summary> 終了時刻 - Text </summary>
    public ReactiveProperty<string> EndTime_Text { get; set; } = new ReactiveProperty<string>();

    /// <summary> 場所 - Text </summary>
    public ReactiveProperty<string> Place_Text { get; set; } = new ReactiveProperty<string>();
    
    /// <summary> 詳細 - Text </summary>
    public ReactiveProperty<string> Description_Text { get; set; } = new ReactiveProperty<string>();
}
