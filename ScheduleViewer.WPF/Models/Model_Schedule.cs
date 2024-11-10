using System.Windows.Forms;

namespace ScheduleViewer.WPF.Models;

/// <summary>
/// Model - スケジュール
/// </summary>
public sealed class Model_Schedule : ModelBase<ViewModel_MainWindow>
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

    /// <summary> ViewModel - MainWindow </summary>
    internal override ViewModel_MainWindow ViewModel { get; set; }

    /// <summary> ViewModel - スケジュール - ヘッダ </summary>
    internal ViewModel_Schedule_Header ViewModel_Header { get; set; }

    /// <summary> ViewModel - スケジュール - テーブル </summary>
    internal ViewModel_Schedule_Table ViewModel_Table { get; set; }

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

        this.Update_HeaderAsync();
    }

    /// <summary>
    /// 日付の更新
    /// </summary>
    /// <returns>void</returns>
    public void Update_HeaderAsync()
    {
        this.ViewModel_Header.Year_Text.Value  = this.TargetDate.Year;
        this.ViewModel_Header.Month_Text.Value = this.TargetDate.Month;
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
        var value = new DateValue(this.TargetDate);

        await Task.WhenAll(
            GoogleFacade.Calendar.InitializeAsync(),
            GoogleFacade.Tasks.InitializeAsync(),
            GoogleFacade.Photo.InitializeAsync(),
            GoogleFacade.Drive.InitializeAsync(),
            GoogleFacade.Fitbit.InitializeAsync(),
            GoogleFacade.Fitness.ReadActivity(value.FirstDateOfMonth, value.LastDateOfMonth),
            GoogleFacade.Fitness.ReadSteps(value.FirstDateOfMonth, value.LastDateOfMonth),
            GoogleFacade.Fitness.ReadSleepTime(value.FirstDateOfMonth, value.LastDateOfMonth));

        this.UpdateTable();
    }

    private void UpdateTable()
    {
        var value = new DateValue(this.TargetDate);

        var currentDate = value.FirstDateOfMonth;

        var nextMonth = new DateValue(this.TargetDate.AddMonths(1));

        while (true)
        {
            if (currentDate == nextMonth.FirstDateOfMonth)
            {
                // 翌月の月初
                break;
            }

            this.SetCalendarDay(currentDate);

            this.GetCalendarEvents(currentDate);

            currentDate = currentDate.AddDays(1);
        }
    }

    /// <summary>
    /// カレンダーの日付設定
    /// </summary>
    /// <param name="date">日付</param>
    /// <remarks>
    /// カレンダーの日付を初期化する。代入先がContentなので、忘れずにToStringすること。
    /// </remarks>
    private void SetCalendarDay(DateTime date)
    {
        var table = this.ViewModel_Table;

        var events = this.GetCalendarEvents(date);

        var txtDay = string.Empty;

        switch (DateUtils.GetWeekOfMonth(date))
        {
            case 1:
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Monday    : table.WeekNum1_Monday.Value    = events; break;
                    case DayOfWeek.Tuesday   : table.WeekNum1_Tuesday.Value   = events; break;
                    case DayOfWeek.Wednesday : table.WeekNum1_Wednesday.Value = events; break;
                    case DayOfWeek.Thursday  : table.WeekNum1_Thursday.Value  = events; break;
                    case DayOfWeek.Friday    : table.WeekNum1_Friday.Value    = events; break;
                    case DayOfWeek.Saturday  : table.WeekNum1_Saturday.Value  = events; break;
                    case DayOfWeek.Sunday    : table.WeekNum1_Sunday.Value    = events; break;
                }
                break;

            case 2:
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Monday    : table.WeekNum2_Monday.Value    = events; break;
                    case DayOfWeek.Tuesday   : table.WeekNum2_Tuesday.Value   = events; break;
                    case DayOfWeek.Wednesday : table.WeekNum2_Wednesday.Value = events; break;
                    case DayOfWeek.Thursday  : table.WeekNum2_Thursday.Value  = events; break;
                    case DayOfWeek.Friday    : table.WeekNum2_Friday.Value    = events; break;
                    case DayOfWeek.Saturday  : table.WeekNum2_Saturday.Value  = events; break;
                    case DayOfWeek.Sunday    : table.WeekNum2_Sunday.Value    = events; break;
                }
                break;

            case 3:
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Monday    : table.WeekNum3_Monday.Value    = events; break;
                    case DayOfWeek.Tuesday   : table.WeekNum3_Tuesday.Value   = events; break;
                    case DayOfWeek.Wednesday : table.WeekNum3_Wednesday.Value = events; break;
                    case DayOfWeek.Thursday  : table.WeekNum3_Thursday.Value  = events; break;
                    case DayOfWeek.Friday    : table.WeekNum3_Friday.Value    = events; break;
                    case DayOfWeek.Saturday  : table.WeekNum3_Saturday.Value  = events; break;
                    case DayOfWeek.Sunday    : table.WeekNum3_Sunday.Value    = events; break;
                }
                break;

            case 4:
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Monday    : table.WeekNum4_Monday.Value    = events; break;
                    case DayOfWeek.Tuesday   : table.WeekNum4_Tuesday.Value   = events; break;
                    case DayOfWeek.Wednesday : table.WeekNum4_Wednesday.Value = events; break;
                    case DayOfWeek.Thursday  : table.WeekNum4_Thursday.Value  = events; break;
                    case DayOfWeek.Friday    : table.WeekNum4_Friday.Value    = events; break;
                    case DayOfWeek.Saturday  : table.WeekNum4_Saturday.Value  = events; break;
                    case DayOfWeek.Sunday    : table.WeekNum4_Sunday.Value    = events; break;
                }
                break;

            case 5:
                switch (date.DayOfWeek)
                {
                    case DayOfWeek.Monday    : table.WeekNum5_Monday.Value    = events; break;
                    case DayOfWeek.Tuesday   : table.WeekNum5_Tuesday.Value   = events; break;
                    case DayOfWeek.Wednesday : table.WeekNum5_Wednesday.Value = events; break;
                    case DayOfWeek.Thursday  : table.WeekNum5_Thursday.Value  = events; break;
                    case DayOfWeek.Friday    : table.WeekNum5_Friday.Value    = events; break;
                    case DayOfWeek.Saturday  : table.WeekNum5_Saturday.Value  = events; break;
                    case DayOfWeek.Sunday    : table.WeekNum5_Sunday.Value    = events; break;
                }
                break;
        }
    }

    /// <summary>
    /// カレンダーのイベント設定
    /// </summary>
    /// <param name="date">日付</param>
    /// <remarks>
    /// 指定された日付のイベントをカレンダーに設定する。
    /// </remarks>
    private ScheduleEntity GetCalendarEvents(DateTime date)
    {
        var allEvents = GoogleFacade.Calendar.FindByDate(date);

        var allDayEvent = allEvents.Where(x => x.IsAllDay  == true && 
                                               x.IsBook    == false && 
                                               x.IsProgram == false).ToList().FirstOrDefault();
        
        var dailyEvents = allEvents.Where(x => x.IsAllDay == false).ToList();

        if (dailyEvents.IsEmpty())
        {
            return new ScheduleEntity(
                date, allDayEvent?.Title, 
                default, default, default, default, default);
        }

        if (dailyEvents.Count == 1)
        {
            return new ScheduleEntity(
                date, allDayEvent?.Title, 
                dailyEvents[0].Title, default, default, default, default);
        }

        if (dailyEvents.Count == 2)
        {
            return new ScheduleEntity(
                date, allDayEvent?.Title, 
                dailyEvents[0].Title, dailyEvents[1].Title, default, default, default);
        }

        if (dailyEvents.Count == 3)
        {
            return new ScheduleEntity(
                date, allDayEvent?.Title,
                dailyEvents[0].Title, dailyEvents[1].Title, dailyEvents[2].Title, default, default);
        }

        if (dailyEvents.Count == 4)
        {
            return new ScheduleEntity(
                date, allDayEvent?.Title,
                dailyEvents[0].Title, dailyEvents[1].Title, dailyEvents[2].Title,
                dailyEvents[3].Title, default);
        }

        return new ScheduleEntity(
                date, allDayEvent?.Title,
                dailyEvents[0].Title, dailyEvents[1].Title, dailyEvents[2].Title,
                dailyEvents[3].Title, dailyEvents[4].Title);
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

            this.Clear();
            this.UpdateTable();
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

            this.Clear();
            this.UpdateTable();
        }
    }

    /// <summary>
    /// クリア
    /// </summary>
    internal void Clear()
    {
        this.Clear_Content();
    }

    /// <summary>
    /// クリア - Content
    /// </summary>
    private void Clear_Content()
    {
        var empty = new ScheduleEntity(default, default, 
                                       default, default, default, default, default);

        // 第1週
        this.ViewModel_Table.WeekNum1_Monday.Value    = empty;
        this.ViewModel_Table.WeekNum1_Tuesday.Value   = empty;
        this.ViewModel_Table.WeekNum1_Wednesday.Value = empty;
        this.ViewModel_Table.WeekNum1_Thursday.Value  = empty;
        this.ViewModel_Table.WeekNum1_Friday.Value    = empty;
        this.ViewModel_Table.WeekNum1_Saturday.Value  = empty;
        this.ViewModel_Table.WeekNum1_Sunday.Value    = empty;

        // 第2週
        this.ViewModel_Table.WeekNum2_Monday.Value    = empty;
        this.ViewModel_Table.WeekNum2_Tuesday.Value   = empty;
        this.ViewModel_Table.WeekNum2_Wednesday.Value = empty;
        this.ViewModel_Table.WeekNum2_Thursday.Value  = empty;
        this.ViewModel_Table.WeekNum2_Friday.Value    = empty;
        this.ViewModel_Table.WeekNum2_Saturday.Value  = empty;
        this.ViewModel_Table.WeekNum2_Sunday.Value    = empty;

        // 第3週
        this.ViewModel_Table.WeekNum3_Monday.Value    = empty;
        this.ViewModel_Table.WeekNum3_Tuesday.Value   = empty;
        this.ViewModel_Table.WeekNum3_Wednesday.Value = empty;
        this.ViewModel_Table.WeekNum3_Thursday.Value  = empty;
        this.ViewModel_Table.WeekNum3_Friday.Value    = empty;
        this.ViewModel_Table.WeekNum3_Saturday.Value  = empty;
        this.ViewModel_Table.WeekNum3_Sunday.Value    = empty;

        // 第4週
        this.ViewModel_Table.WeekNum4_Monday.Value    = empty;
        this.ViewModel_Table.WeekNum4_Tuesday.Value   = empty;
        this.ViewModel_Table.WeekNum4_Wednesday.Value = empty;
        this.ViewModel_Table.WeekNum4_Thursday.Value  = empty;
        this.ViewModel_Table.WeekNum4_Friday.Value    = empty;
        this.ViewModel_Table.WeekNum4_Saturday.Value  = empty;
        this.ViewModel_Table.WeekNum4_Sunday.Value    = empty;

        // 第5週
        this.ViewModel_Table.WeekNum5_Monday.Value    = empty;
        this.ViewModel_Table.WeekNum5_Tuesday.Value   = empty;
        this.ViewModel_Table.WeekNum5_Wednesday.Value = empty;
        this.ViewModel_Table.WeekNum5_Thursday.Value  = empty;
        this.ViewModel_Table.WeekNum5_Friday.Value    = empty;
        this.ViewModel_Table.WeekNum5_Saturday.Value  = empty;
        this.ViewModel_Table.WeekNum5_Sunday.Value    = empty;
    }

    /// <summary>
    /// 予定詳細画面の表示
    /// </summary>
    /// <param name="txtDay">対象日</param>
    /// <remarks>
    /// ViewModelの初期描画(BindEventsメソッド)でも呼ばれるので注意する。
    /// </remarks>
    internal void ShowDetailWindow(string txtDay)
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
