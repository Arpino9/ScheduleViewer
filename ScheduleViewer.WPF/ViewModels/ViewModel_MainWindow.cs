namespace ScheduleViewer.WPF.ViewModels;

/// <summary>
/// ViewModel - MainWindow
/// </summary>
public sealed class ViewModel_MainWindow : ViewModelBase<Model_WorkSchedule>
{
    public override event PropertyChangedEventHandler PropertyChanged;

    public ViewModel_MainWindow()
    {
        this.Model_Private.ViewModel = this;
        this.Model_Private.ViewModel = this;
    }

    /// <summary>
    /// Model - 勤務表
    /// </summary>
    protected Model_Schedule Model_Private => Model_Schedule.GetInstance();

    /// <summary>
    /// Model - 勤務表
    /// </summary>
    protected override Model_WorkSchedule Model => Model_WorkSchedule.GetInstance();

    protected override void BindEvents()
    {
        throw new NotImplementedException();
    }
}
