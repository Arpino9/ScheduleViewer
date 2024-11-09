namespace ScheduleViewer.WPF.Models;

public sealed class Model_Schedule
{

    #region Get Instance

    private static Model_Schedule model = null;

    public static Model_Schedule GetInstance()
    {
        if (model == null)
        {
            model = new Model_Schedule();
        }

        return model;
    }

    #endregion

    internal ViewModel_MainWindow ViewModel { get; set; }

    /// <summary> ViewModel - 勤務表 </summary>
    internal ViewModel_Schedule_Table ViewModel_Table { get; set; }

    /// <summary> ViewModel - 勤務表 </summary>
    internal ViewModel_Schedule_Header ViewModel_Header { get; set; }

    /// <summary>
    /// Model - スケジュール詳細
    /// </summary>
    private Model_ScheduleDetails Model_ScheduleDetails { get; set; }
        = Model_ScheduleDetails.GetInstance();

    /// <summary> 対象日 </summary>
    public DateTime TargetDate { get; set; }

    /// <summary>
    /// 初期化
    /// </summary>
    public async Task Initialize_HeaderAsync()
    {
        this.TargetDate = DateTime.Now;

        await Update_HeaderAsync();
    }

    /// <summary>
    /// 日付の更新
    /// </summary>
    /// <returns></returns>
    public async Task Update_HeaderAsync()
    {
        var value = new DateValue(this.TargetDate);

        this.ViewModel_Header.Year_Text.Value = this.TargetDate.Year;
        this.ViewModel_Header.Month_Text.Value = this.TargetDate.Month;

        await Task.WhenAll(
            GoogleFacade.Calendar.InitializeAsync(),
            GoogleFacade.Tasks.InitializeAsync(),
            GoogleFacade.Photo.InitializeAsync(),
            GoogleFacade.Drive.InitializeAsync(),
            GoogleFacade.Fitbit.InitializeAsync(),
            GoogleFacade.Fitness.ReadActivity(value.FirstDateOfMonth, value.LastDateOfMonth),
            GoogleFacade.Fitness.ReadSteps(value.FirstDateOfMonth, value.LastDateOfMonth),
            GoogleFacade.Fitness.ReadSleepTime(value.FirstDateOfMonth, value.LastDateOfMonth));
    }

    /// <summary>
    /// テーブルの初期化
    /// </summary>
    /// <returns>void</returns>
    /// <remarks>
    /// 該当月の日付を初期化する。
    /// </remarks>
    public async Task Initialize_TableAsync()
    {
        var currentDate = new DateValue(this.TargetDate).FirstDateOfMonth;

        var nextMonth = new DateValue(this.TargetDate.AddMonths(1));

        while (true)
        {
            if (currentDate == nextMonth.FirstDateOfMonth)
            {
                break;
            }

            this.SetCalendarDay(currentDate);

            currentDate = currentDate.AddDays(1);
        }
    }

    /// <summary>
    /// カレンダーの日付設定
    /// </summary>
    /// <param name="date">日付</param>
    /// <remarks>
    /// カレンダーの日付を初期化する。
    /// </remarks>
    private void SetCalendarDay(DateTime date)
    {
        var table = this.ViewModel_Table;

        switch (DateUtils.GetWeekOfMonth(date))
        {
            case 1:
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Monday    : table.WeekNum1_Monday_Content.Value    = date.Day.ToString(); break;
                    case DayOfWeek.Tuesday   : table.WeekNum1_Tuesday_Content.Value   = date.Day.ToString(); break;
                    case DayOfWeek.Wednesday : table.WeekNum1_Wednesday_Content.Value = date.Day.ToString(); break;
                    case DayOfWeek.Thursday  : table.WeekNum1_Thursday_Content.Value  = date.Day.ToString(); break;
                    case DayOfWeek.Friday    : table.WeekNum1_Friday_Content.Value    = date.Day.ToString(); break;
                    case DayOfWeek.Saturday  : table.WeekNum1_Saturday_Content.Value  = date.Day.ToString(); break;
                    case DayOfWeek.Sunday    : table.WeekNum1_Sunday_Content.Value    = date.Day.ToString(); break;
                }
                break;

            case 2:
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Monday    : table.WeekNum2_Monday_Content.Value    = date.Day.ToString(); break;
                    case DayOfWeek.Tuesday   : table.WeekNum2_Tuesday_Content.Value   = date.Day.ToString(); break;
                    case DayOfWeek.Wednesday : table.WeekNum2_Wednesday_Content.Value = date.Day.ToString(); break;
                    case DayOfWeek.Thursday  : table.WeekNum2_Thursday_Content.Value  = date.Day.ToString(); break;
                    case DayOfWeek.Friday    : table.WeekNum2_Friday_Content.Value    = date.Day.ToString(); break;
                    case DayOfWeek.Saturday  : table.WeekNum2_Saturday_Content.Value  = date.Day.ToString(); break;
                    case DayOfWeek.Sunday    : table.WeekNum2_Sunday_Content.Value    = date.Day.ToString(); break;
                }
                break;

            case 3:
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Monday    : table.WeekNum3_Monday_Content.Value    = date.Day.ToString(); break;
                    case DayOfWeek.Tuesday   : table.WeekNum3_Tuesday_Content.Value   = date.Day.ToString(); break;
                    case DayOfWeek.Wednesday : table.WeekNum3_Wednesday_Content.Value = date.Day.ToString(); break;
                    case DayOfWeek.Thursday  : table.WeekNum3_Thursday_Content.Value  = date.Day.ToString(); break;
                    case DayOfWeek.Friday    : table.WeekNum3_Friday_Content.Value    = date.Day.ToString(); break;
                    case DayOfWeek.Saturday  : table.WeekNum3_Saturday_Content.Value  = date.Day.ToString(); break;
                    case DayOfWeek.Sunday    : table.WeekNum3_Sunday_Content.Value    = date.Day.ToString(); break;
                }
                break;

            case 4:
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Monday    : table.WeekNum4_Monday_Content.Value    = date.Day.ToString(); break;
                    case DayOfWeek.Tuesday   : table.WeekNum4_Tuesday_Content.Value   = date.Day.ToString(); break;
                    case DayOfWeek.Wednesday : table.WeekNum4_Wednesday_Content.Value = date.Day.ToString(); break;
                    case DayOfWeek.Thursday  : table.WeekNum4_Thursday_Content.Value  = date.Day.ToString(); break;
                    case DayOfWeek.Friday    : table.WeekNum4_Friday_Content.Value    = date.Day.ToString(); break;
                    case DayOfWeek.Saturday  : table.WeekNum4_Saturday_Content.Value  = date.Day.ToString(); break;
                    case DayOfWeek.Sunday    : table.WeekNum4_Sunday_Content.Value    = date.Day.ToString(); break;
                }
                break;

            case 5:
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Monday    : table.WeekNum5_Monday_Content.Value    = date.Day.ToString(); break;
                    case DayOfWeek.Tuesday   : table.WeekNum5_Tuesday_Content.Value   = date.Day.ToString(); break;
                    case DayOfWeek.Wednesday : table.WeekNum5_Wednesday_Content.Value = date.Day.ToString(); break;
                    case DayOfWeek.Thursday  : table.WeekNum5_Thursday_Content.Value  = date.Day.ToString(); break;
                    case DayOfWeek.Friday    : table.WeekNum5_Friday_Content.Value    = date.Day.ToString(); break;
                    case DayOfWeek.Saturday  : table.WeekNum5_Saturday_Content.Value  = date.Day.ToString(); break;
                    case DayOfWeek.Sunday    : table.WeekNum5_Sunday_Content.Value    = date.Day.ToString(); break;
                }
                break;
        }
    }

    /// <summary>
    /// 戻る
    /// </summary>
    internal async Task ReturnAsync()
    {
        using (new CursorWaiting())
        {
            this.TargetDate = this.TargetDate.AddMonths(-1);

            this.ViewModel_Header.Year_Text.Value  = this.TargetDate.Year;
            this.ViewModel_Header.Month_Text.Value = this.TargetDate.Month;

            await this.Initialize_TableAsync();
        }
    }

    /// <summary>
    /// 進む
    /// </summary>
    internal async Task ProceedAsync()
    {
        using (new CursorWaiting())
        {
            this.TargetDate = this.TargetDate.AddMonths(1);

            this.ViewModel_Header.Year_Text.Value  = this.TargetDate.Year;
            this.ViewModel_Header.Month_Text.Value = this.TargetDate.Month;

            await this.Initialize_TableAsync();
        }
    }

    /// <summary>
    /// 予定詳細画面の表示
    /// </summary>
    /// <param name="txtDay">対象日</param>
    /// <param name="dayOfWeek">対象曜日</param>
    internal void ShowDetailWindow(string txtDay, DayOfWeek dayOfWeek)
    {
        if (this.ViewModel_Table.CursorWaiting.IsWaiting == true)
        {
            // 初期化中
            return;
        }

        int.TryParse(txtDay, out var day);

        if (day == 0)
        {
            // 対象日が空欄
            return;
        }

        var date = new DateTime(this.TargetDate.Year, this.TargetDate.Month, day);

        this.Model_ScheduleDetails.Date = date;
        var details = new ScheduleDetails();
        details.Show();
    }
}
