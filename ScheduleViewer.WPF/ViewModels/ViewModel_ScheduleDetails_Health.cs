namespace ScheduleViewer.WPF.ViewModels;

/// <summary>
/// ViewModel - スケジュール詳細 (健康管理)
/// </summary>
public sealed class ViewModel_ScheduleDetails_Health : ViewModelBase<Model_ScheduleDetails_Health>
{
    public override event PropertyChangedEventHandler PropertyChanged;

    public ViewModel_ScheduleDetails_Health()
    {
        this.Model.ViewModel = this;

        this.Model.Initialize();
    }

    protected override void BindEvents()
    {
        throw new NotImplementedException();
    }

    /// <summary> Model - スケジュール詳細 (健康管理) </summary>
    protected override Model_ScheduleDetails_Health Model 
        => Model_ScheduleDetails_Health.GetInstance();

    /// <summary> 歩数 - Text </summary>
    public ReactiveProperty<int> Step_Text { get; set; } = new ReactiveProperty<int>();
}
