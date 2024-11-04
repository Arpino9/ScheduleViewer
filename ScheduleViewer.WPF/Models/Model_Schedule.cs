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

    /// <summary> 対象日 </summary>
    public DateTime TargetDate { get; set; }

    /// <summary>
    /// 初期化
    /// </summary>
    public async Task Initialize_HeaderAsync()
    {
        this.TargetDate = DateTime.Now;

        var value = new DateValue(this.TargetDate);

        this.ViewModel_Header.Year_Text.Value  = this.TargetDate.Year;
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

    public async Task Initialize_TableAsync()
    {
        this.SetCalendarDay(DayOfWeek.Monday);
        this.SetCalendarDay(DayOfWeek.Tuesday);
        this.SetCalendarDay(DayOfWeek.Wednesday);
        this.SetCalendarDay(DayOfWeek.Thursday);
        this.SetCalendarDay(DayOfWeek.Friday);
        this.SetCalendarDay(DayOfWeek.Saturday);
        this.SetCalendarDay(DayOfWeek.Sunday);
    }

    /// <summary>
    /// カレンダーの日付設定
    /// </summary>
    /// <param name="dayOfWeek"></param>
    private void SetCalendarDay(DayOfWeek dayOfWeek)
    {
        var table = this.ViewModel_Table;

        var dates = DateUtils.FindWantDayOfWeek(this.TargetDate.Year,
                                                this.TargetDate.Month,
                                                dayOfWeek);

        foreach (var date in dates)
        {
            switch (DateUtils.GetWeekOfMonth(date))
            {
                case 1:
                    switch (dayOfWeek)
                    {
                        case DayOfWeek.Monday    : table.Monday_WeekNum1.Value    = date.Day.ToString(); break;
                        case DayOfWeek.Tuesday   : table.Tuesday_WeekNum1.Value   = date.Day.ToString(); break;
                        case DayOfWeek.Wednesday : table.Wednesday_WeekNum1.Value = date.Day.ToString(); break;
                        case DayOfWeek.Thursday  : table.Thursday_WeekNum1.Value  = date.Day.ToString(); break;
                        case DayOfWeek.Friday    : table.Friday_WeekNum1.Value    = date.Day.ToString(); break;
                        case DayOfWeek.Saturday  : table.Saturday_WeekNum1.Value  = date.Day.ToString(); break;
                        case DayOfWeek.Sunday    : table.Sunday_WeekNum1.Value    = date.Day.ToString(); break;
                    }
                    break;

                case 2:
                    switch (dayOfWeek)
                    {
                        case DayOfWeek.Monday    : table.Monday_WeekNum2.Value    = date.Day.ToString(); break;
                        case DayOfWeek.Tuesday   : table.Tuesday_WeekNum2.Value   = date.Day.ToString(); break;
                        case DayOfWeek.Wednesday : table.Wednesday_WeekNum2.Value = date.Day.ToString(); break;
                        case DayOfWeek.Thursday  : table.Thursday_WeekNum2.Value  = date.Day.ToString(); break;
                        case DayOfWeek.Friday    : table.Friday_WeekNum2.Value    = date.Day.ToString(); break;
                        case DayOfWeek.Saturday  : table.Saturday_WeekNum2.Value  = date.Day.ToString(); break;
                        case DayOfWeek.Sunday    : table.Sunday_WeekNum2.Value    = date.Day.ToString(); break;
                    }
                    break;

                case 3:
                    switch (dayOfWeek)
                    {
                        case DayOfWeek.Monday    : table.Monday_WeekNum3.Value    = date.Day.ToString(); break;
                        case DayOfWeek.Tuesday   : table.Tuesday_WeekNum3.Value   = date.Day.ToString(); break;
                        case DayOfWeek.Wednesday : table.Wednesday_WeekNum3.Value = date.Day.ToString(); break;
                        case DayOfWeek.Thursday  : table.Thursday_WeekNum3.Value  = date.Day.ToString(); break;
                        case DayOfWeek.Friday    : table.Friday_WeekNum3.Value    = date.Day.ToString(); break;
                        case DayOfWeek.Saturday  : table.Saturday_WeekNum3.Value  = date.Day.ToString(); break;
                        case DayOfWeek.Sunday    : table.Sunday_WeekNum3.Value    = date.Day.ToString(); break;
                    }
                    break;

                case 4:
                    switch (dayOfWeek)
                    {
                        case DayOfWeek.Monday    : table.Monday_WeekNum4.Value    = date.Day.ToString(); break;
                        case DayOfWeek.Tuesday   : table.Tuesday_WeekNum4.Value   = date.Day.ToString(); break;
                        case DayOfWeek.Wednesday : table.Wednesday_WeekNum4.Value = date.Day.ToString(); break;
                        case DayOfWeek.Thursday  : table.Thursday_WeekNum4.Value  = date.Day.ToString(); break;
                        case DayOfWeek.Friday    : table.Friday_WeekNum4.Value    = date.Day.ToString(); break;
                        case DayOfWeek.Saturday  : table.Saturday_WeekNum4.Value  = date.Day.ToString(); break;
                        case DayOfWeek.Sunday    : table.Sunday_WeekNum4.Value    = date.Day.ToString(); break;
                    }
                    break;

                case 5:
                    switch (dayOfWeek)
                    {
                        case DayOfWeek.Monday    : table.Monday_WeekNum5.Value    = date.Day.ToString(); break;
                        case DayOfWeek.Tuesday   : table.Tuesday_WeekNum5.Value   = date.Day.ToString(); break;
                        case DayOfWeek.Wednesday : table.Wednesday_WeekNum5.Value = date.Day.ToString(); break;
                        case DayOfWeek.Thursday  : table.Thursday_WeekNum5.Value  = date.Day.ToString(); break;
                        case DayOfWeek.Friday    : table.Friday_WeekNum5.Value    = date.Day.ToString(); break;
                        case DayOfWeek.Saturday  : table.Saturday_WeekNum5.Value  = date.Day.ToString(); break;
                        case DayOfWeek.Sunday    : table.Sunday_WeekNum5.Value    = date.Day.ToString(); break;
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// 戻る
    /// </summary>
    internal async Task Return()
    {
        using (new CursorWaiting())
        {
            this.TargetDate = this.TargetDate.AddMonths(-1);

            this.ViewModel_Header.Year_Text.Value = this.TargetDate.Year;
            this.ViewModel_Header.Month_Text.Value = this.TargetDate.Month;
        }
    }

    /// <summary>
    /// 進む
    /// </summary>
    internal async Task Proceed()
    {
        using (new CursorWaiting())
        {
            this.TargetDate = this.TargetDate.AddMonths(1);

            this.ViewModel_Header.Year_Text.Value = this.TargetDate.Year;
            this.ViewModel_Header.Month_Text.Value = this.TargetDate.Month;
        }
    }
}
