namespace ScheduleViewer.WPF.Models;

/// <summary>
/// Model - スケジュール詳細 (健康管理)
/// </summary>
public sealed class Model_ScheduleDetails_Health : ModelBase<ViewModel_ScheduleDetails_Health>, IViewer
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

        //TODO: 睡眠時間のみNG
        var activities = FitnessReader.ReadActivity(start, end);
        var steps      = FitnessReader.ReadSteps(start, end);
        var sleeping   = FitnessReader.ReadSleepTime(start, end);

        this.ViewModel.Step_Text.Value = steps.FirstOrDefault();
    }

    public void Clear_ViewForm()
    {
        this.ViewModel.Step_Text.Value = 0;
    }

    public void ListView_SelectionChanged()
    {
        throw new NotImplementedException();
    }
}
