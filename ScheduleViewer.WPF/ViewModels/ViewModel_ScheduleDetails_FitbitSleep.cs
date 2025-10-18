namespace ScheduleViewer.WPF.ViewModels;

/// <summary>
/// ViewModel - スケジュール詳細 (健康管理)
/// </summary>
public sealed class ViewModel_ScheduleDetails_FitbitSleep : ViewModelBase<Model_ScheduleDetails_Fitbit>
{
    public override event PropertyChangedEventHandler PropertyChanged;

    public ViewModel_ScheduleDetails_FitbitSleep()
    {
        this.Model.ViewModel = this;

        this.Model.InitializeAsync();
    }

    protected override void BindEvents()
    {
        throw new NotImplementedException();
    }

    /// <summary> Model - スケジュール詳細 (健康管理) </summary>
    protected override Model_ScheduleDetails_Fitbit Model 
        => Model_ScheduleDetails_Fitbit.GetInstance();

    /// <summary> 睡眠時間 </summary>
    public ReactiveProperty<TimeSpan> Sleeping { get; set; } = new ReactiveProperty<TimeSpan>();

    /// <summary> 就寝時刻 </summary>
    public ReactiveProperty<DateTime> StartTime { get; set; } = new ReactiveProperty<DateTime>();

    /// <summary> 起床時刻 </summary>
    public ReactiveProperty<DateTime> EndTime { get; set; } = new ReactiveProperty<DateTime>();

    /// <summary> 覚醒状態 </summary>
    public ReactiveProperty<TimeSpan> Awake { get; set; } = new ReactiveProperty<TimeSpan>();
    
    /// <summary> 寝付けない </summary>
    public ReactiveProperty<TimeSpan> Restless { get; set; } = new ReactiveProperty<TimeSpan>();

    /// <summary> レム睡眠 </summary>
    public ReactiveProperty<TimeSpan> Rem { get; set; } = new ReactiveProperty<TimeSpan>();

    /// <summary> ノンレム睡眠 </summary>
    public ReactiveProperty<TimeSpan> Asleep { get; set; } = new ReactiveProperty<TimeSpan>();
}
