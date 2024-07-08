namespace ScheduleViewer.WPF.Models;

/// <summary>
/// Model - スケジュール詳細 (健康管理)
/// </summary>
public sealed class Model_ScheduleDetails_Health : ModelBase<ViewModel_ScheduleDetails_Health>
{
    #region Get Instance

    private static Model_ScheduleDetails_Health model = null;

    internal static Model_ScheduleDetails_Health GetInstance()
    {
        if (model == null)
        {
            model = new Model_ScheduleDetails_Health();
        }

        return model;
    }

    #endregion

    /// <summary> ViewModel - スケジュール詳細 </summary>
    public ViewModel_ScheduleDetails ViewModel_Header { get; set; }

    /// <summary> ViewModel - スケジュール詳細 (健康管理) </summary>
    internal override ViewModel_ScheduleDetails_Health ViewModel { get; set; }

    public void Initialize()
    {
        var start = this.ViewModel_Header.Date.Value.ToOffset();
        var end   = this.ViewModel_Header.Date.Value.AddDays(1).ToOffset();

        var steps      = GoogleFacade.Fitness.FindStepsByDate(this.ViewModel_Header.Date.Value);
        var activities = GoogleFacade.Fitness.FindActivitiesByDate(this.ViewModel_Header.Date.Value);
        var sleeping   = GoogleFacade.Fitness.FindSleepTimeByDate(this.ViewModel_Header.Date.Value);

        if (activities.Any() && activities.Where(x => x.Name == "Strength training").Any())
        {
            var minutes = activities.Where(x => x.Name == "Strength training").FirstOrDefault().Value / 60000;
            TimeSpan time = new TimeSpan(0, minutes, 0);

            this.ViewModel.StrengthTrainingHour.Value = time;
        }

        this.ViewModel.Step_Text.Value = steps.FirstOrDefault();

        //FitnessWriter.WriteActivity(start, end);
    }

    public void Clear_ViewForm()
    {
        this.ViewModel.Step_Text.Value = 0;

        this.ViewModel.StrengthTrainingHour.Value = new TimeSpan(0, 0, 0);
    }

    public void ListView_SelectionChanged()
    {
        throw new NotImplementedException();
    }
}
