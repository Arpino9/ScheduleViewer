using ScheduleViewer.WPF.Models;

namespace ScheduleViewer.WPF.ViewModels;

/// <summary>
/// ViewModel - スケジュール詳細 (予定一覧)
/// </summary>
public sealed class ViewModel_ScheduleDetails_Plan : ViewModelBase
{        
    public override event PropertyChangedEventHandler PropertyChanged;

    public ViewModel_ScheduleDetails_Plan()
    {
        this.Model.ViewModel = this;

        this.Model.Initialize();

        BindEvents();
    }

    public Model_ScheduleDetails_Plan Model = Model_ScheduleDetails_Plan.GetInstance();

    protected override void BindEvents()
    {
        Events_SelectionChanged.Subscribe(_ => this.Model.Plans_SelectionChanged());
    }

    public ReactiveCollection<CalendarEventsEntity> Events_ItemSource { get; set; } = new ReactiveCollection<CalendarEventsEntity>();

    /// <summary> 予定 - SelectionChanged </summary>
    public ReactiveCommand Events_SelectionChanged { get; private set; } = new ReactiveCommand();

    public ReactiveProperty<int> Events_SelectedIndex { get; set; } = new ReactiveProperty<int>();
    public ReactiveProperty<string> Title_Text { get; set; } = new ReactiveProperty<string>();
    public ReactiveProperty<DateTime> StartTime_Text { get; set; } = new ReactiveProperty<DateTime>();
    public ReactiveProperty<DateTime> EndTime_Text { get; set; } = new ReactiveProperty<DateTime>();
    public ReactiveProperty<string> Place_Text { get; set; } = new ReactiveProperty<string>();
    public ReactiveProperty<string> Description_Text { get; set; } = new ReactiveProperty<string>();
}
