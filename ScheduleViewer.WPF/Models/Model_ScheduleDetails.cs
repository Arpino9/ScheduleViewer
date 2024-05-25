namespace ScheduleViewer.WPF.Models;

/// <summary>
/// Model - スケジュール詳細
/// </summary>
public sealed class Model_ScheduleDetails : ModelBase<ViewModel_ScheduleDetails>
{
    #region Get Instance

    private static Model_ScheduleDetails model = null;

    public static Model_ScheduleDetails GetInstance()
    {
        if (model == null)
        {
            model = new Model_ScheduleDetails();
        }

        return model;
    }

    #endregion

    internal override void Initialize()
    {
        this.Model_ScheduleDetails_Plan.ViewModel_Header = this.ViewModel;
        this.Model_ScheduleDetails_Book.ViewModel_Header = this.ViewModel;

        this.ViewModel.Date.Value = this.Date;
    }

    internal void Return()
    {
        this.Date = this.Date.AddDays(-1);

        this.ViewModel.Date.Value = this.Date;
        this.Model_ScheduleDetails_Plan.Initialize();
        this.Model_ScheduleDetails_Book.Initialize();
    }

    internal void Proceed()
    {
        this.Date = this.Date.AddDays(1);

        this.ViewModel.Date.Value = this.Date;
        this.Model_ScheduleDetails_Plan.Initialize();
        this.Model_ScheduleDetails_Book.Initialize();
    }

    /// <summary> 日付 </summary>
    public DateTime Date { get; set; } = new DateTime();

    /// <summary> ViewModel </summary>
    internal override ViewModel_ScheduleDetails ViewModel { get; set; }

    internal Model_ScheduleDetails_Plan Model_ScheduleDetails_Plan { get; set; }
        = Model_ScheduleDetails_Plan.GetInstance();

    internal Model_ScheduleDetails_Book Model_ScheduleDetails_Book { get; set; }
        = Model_ScheduleDetails_Book.GetInstance();
}
