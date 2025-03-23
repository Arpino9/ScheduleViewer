namespace ScheduleViewer.WPF.Models;

/// <summary>
/// Model - スケジュール詳細 (健康管理)
/// </summary>
public sealed class Model_ScheduleDetails_Fitbit : ModelBase<ViewModel_ScheduleDetails_FitbitSleep>
{
    #region Get Instance

    private static Model_ScheduleDetails_Fitbit model = null;

    internal static Model_ScheduleDetails_Fitbit GetInstance()
    {
        if (model == null)
        {
            model = new Model_ScheduleDetails_Fitbit();
        }

        return model;
    }

    #endregion

    /// <summary> ViewModel - スケジュール詳細 </summary>
    public ViewModel_ScheduleDetails ViewModel_Header { get; set; }

    /// <summary> ViewModel - スケジュール詳細 (健康管理) </summary>
    internal override ViewModel_ScheduleDetails_FitbitSleep ViewModel { get; set; }

    public async Task Initialize()
    {
        var start = this.ViewModel_Header.Date.Value.ToDateTime(TimeOnly.MinValue).ToOffset();
        var end   = this.ViewModel_Header.Date.Value.AddDays(1).ToDateTime(TimeOnly.MinValue).ToOffset();

        await this.Initialize_Sleep();

        var activity = GoogleFacade.Fitbit.ReadActivityAsync(this.ViewModel_Header.Date.Value);
        var heart    = GoogleFacade.Fitbit.ReadHeartAsync(this.ViewModel_Header.Date.Value);
        var weight   = GoogleFacade.Fitbit.ReadWeightAsync(this.ViewModel_Header.Date.Value);

        var steps      = GoogleFacade.Fitness.FindStepsByDate(this.ViewModel_Header.Date.Value);
        var activities = GoogleFacade.Fitness.FindActivitiesByDate(this.ViewModel_Header.Date.Value);
        var sleeping   = GoogleFacade.Fitness.FindSleepTimeByDate(this.ViewModel_Header.Date.Value);
    }

    /// <summary>
    /// Initialize - 睡眠時間
    /// </summary>
    private async Task Initialize_Sleep()
    {
        var sleep = await GoogleFacade.Fitbit.ReadSleepAsync(this.ViewModel_Header.Date.Value);

        if (sleep is null)
        {
            return;
        }

        this.ViewModel.Sleeping.Value  = sleep.Sleeping;
        this.ViewModel.StartTime.Value = sleep.StartTime;
        this.ViewModel.EndTime.Value   = sleep.EndTime;
        this.ViewModel.Rem.Value       = sleep.Rem;
        this.ViewModel.Restless.Value  = sleep.Restless;
        this.ViewModel.Awake.Value     = sleep.Awake;
        this.ViewModel.Asleep.Value    = sleep.Asleep;
    }

    public void Clear_ViewForm()
    {
        this.ViewModel.Sleeping.Value  = default(TimeSpan);
        this.ViewModel.StartTime.Value = default(DateTime);
        this.ViewModel.EndTime.Value   = default(DateTime);
        this.ViewModel.Rem.Value       = default(TimeSpan);
        this.ViewModel.Restless.Value  = default(TimeSpan);
        this.ViewModel.Awake.Value     = default(TimeSpan);
        this.ViewModel.Asleep.Value    = default(TimeSpan);
    }

    public void ListView_SelectionChanged()
    {
        throw new NotImplementedException();
    }
}
