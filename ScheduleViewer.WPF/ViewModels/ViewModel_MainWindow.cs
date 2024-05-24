namespace ScheduleViewer.WPF.ViewModels;

public sealed class ViewModel_MainWindow : ViewModelBase
{
    public override event PropertyChangedEventHandler PropertyChanged;

    public ViewModel_MainWindow()
    {
        this.Model.ViewModel = this;
    }

    protected override void BindEvents()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Model - 勤務表
    /// </summary>
    private Model_WorkSchedule Model = Model_WorkSchedule.GetInstance();
}
