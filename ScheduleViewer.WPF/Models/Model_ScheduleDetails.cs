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

    internal async Task Initialize()
    {
        this.Model_ScheduleDetails_Plan.ViewModel_Header        = this.ViewModel;
        this.Model_ScheduleDetails_Photo.ViewModel_Header       = this.ViewModel;
        this.Model_ScheduleDetails_Book.ViewModel_Header        = this.ViewModel;
        this.Model_ScheduleDetails_Health.ViewModel_Header      = this.ViewModel;
        this.Model_ScheduleDetails_Task.ViewModel_Header        = this.ViewModel;
        this.Model_ScheduleDetails_Expenditure.ViewModel_Header = this.ViewModel;

        this.ViewModel.Date.Value = this.Date;
    }

    /// <summary>
    /// 戻るボタン
    /// </summary>
    /// <returns>void</returns>
    internal async Task Return()
    {
        using (new CursorWaiting())
        {
            this.Date = this.Date.AddDays(-1);

            if (this.Date.AddMonths(-1).Month != this.Date.Month)
            {
                var value = new DateValue(this.Date);

                await Task.WhenAll(
                    GoogleFacade.Fitness.ReadActivity(value.FirstDateOfMonth, value.LastDateOfMonth),
                    GoogleFacade.Fitness.ReadSteps(value.FirstDateOfMonth, value.LastDateOfMonth),
                    GoogleFacade.Fitness.ReadSleepTime(value.FirstDateOfMonth, value.LastDateOfMonth));

                System.Threading.Thread.Sleep(300);
            }

            await this.Reload();
        }
    }

    /// <summary>
    /// 進むボタン
    /// </summary>
    /// <returns>void</returns>
    internal async Task Proceed()
    {
        using(new CursorWaiting())
        {
            this.Date = this.Date.AddDays(1);

            if (this.Date.AddMonths(1).Month != this.Date.Month)
            {
                var value = new DateValue(this.Date);

                await Task.WhenAll(
                    GoogleFacade.Fitness.ReadActivity(value.FirstDateOfMonth, value.LastDateOfMonth),
                    GoogleFacade.Fitness.ReadSteps(value.FirstDateOfMonth, value.LastDateOfMonth),
                    GoogleFacade.Fitness.ReadSleepTime(value.FirstDateOfMonth, value.LastDateOfMonth));

                System.Threading.Thread.Sleep(300);
            }

            await this.Reload();
        }
    }

    private async Task Reload()
    {
        this.ViewModel.Date.Value = this.Date;

        this.Model_ScheduleDetails_Plan.Clear_ViewForm();
        this.Model_ScheduleDetails_Plan.Initialize();

        this.Model_ScheduleDetails_Photo.Clear_ViewForm();
        this.Model_ScheduleDetails_Photo.Initialize();

        this.Model_ScheduleDetails_Task.Clear_ViewForm();
        this.Model_ScheduleDetails_Task.Initialize();

        this.Model_ScheduleDetails_Health.Clear_ViewForm();
        await this.Model_ScheduleDetails_Health.Initialize();

        this.Model_ScheduleDetails_Book.Clear_ViewForm();
        this.Model_ScheduleDetails_Book.Initialize();

        this.Model_ScheduleDetails_Expenditure.Clear_ViewForm();
        this.Model_ScheduleDetails_Expenditure.Initialize();
    }

    /// <summary> 日付 </summary>
    public DateTime Date { get; set; } = new DateTime();

    /// <summary> ViewModel </summary>
    internal override ViewModel_ScheduleDetails ViewModel { get; set; }

    /// <summary> ViewModel - 予定一覧 </summary>
    internal Model_ScheduleDetails_Plan Model_ScheduleDetails_Plan { get; set; }
        = Model_ScheduleDetails_Plan.GetInstance();

    /// <summary> ViewModel - 本一覧 </summary>
    internal Model_ScheduleDetails_Book Model_ScheduleDetails_Book { get; set; }
        = Model_ScheduleDetails_Book.GetInstance();

    /// <summary> ViewModel - Fitness一覧 </summary>
    internal Model_ScheduleDetails_Fitbit Model_ScheduleDetails_Health { get; set; }
        = Model_ScheduleDetails_Fitbit.GetInstance();

    /// <summary> ViewModel - 写真一覧 </summary>
    internal Model_ScheduleDetails_Photo Model_ScheduleDetails_Photo { get; set; }
        = Model_ScheduleDetails_Photo.GetInstance();

    /// <summary> ViewModel - タスク一覧 </summary>
    internal Model_ScheduleDetails_Task Model_ScheduleDetails_Task { get; set; }
        = Model_ScheduleDetails_Task.GetInstance();

    /// <summary> ViewModel - 支出 </summary>
    internal Model_ScheduleDetails_Expenditure Model_ScheduleDetails_Expenditure { get; set; }
        = Model_ScheduleDetails_Expenditure.GetInstance();
}
