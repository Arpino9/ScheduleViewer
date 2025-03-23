namespace ScheduleViewer.WPF.ViewModels;

/// <summary>
/// ViewModel - スケジュール詳細
/// </summary>
public sealed class ViewModel_ScheduleDetails : ViewModelBase<Model_ScheduleDetails>
{
    public override event PropertyChangedEventHandler PropertyChanged;

    public ViewModel_ScheduleDetails()
    {
        this.Model.ViewModel = this;

        this.Model.Initialize();

        this.BindEvents();
    }

    protected override void BindEvents()
    {
        // 戻る
        Return_Command.Subscribe(_ => this.Model.Return());

        // 進む
        Proceed_Command.Subscribe(_ => this.Model.Proceed());
    }

    protected override Model_ScheduleDetails Model => Model_ScheduleDetails.GetInstance();

    /// <summary> タイトル </summary>
    public ReactiveProperty<string> Window_Title { get; } = new ReactiveProperty<string>("予定詳細");

    /// <summary> 戻る - Command </summary>
    public ReactiveCommand Return_Command { get; private set; } = new ReactiveCommand();

    /// <summary> 日付 </summary>
    public ReactiveProperty<DateOnly> Date { get; set; } = new ReactiveProperty<DateOnly>();

    /// <summary> 進む - Command </summary>
    public ReactiveCommand Proceed_Command { get; private set; } = new ReactiveCommand();
}
