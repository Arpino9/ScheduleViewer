namespace ScheduleViewer.WPF.Models;

/// <summary>
/// Model - スケジュール詳細 (予定一覧)
/// </summary>
public sealed class Model_ScheduleDetails_Plan : ModelBase<ViewModel_ScheduleDetails_Plan>, IViewer
{
    public Model_ScheduleDetails_Plan()
    {
        
    }

    #region Get Instance

    private static Model_ScheduleDetails_Plan model = null;

    public static Model_ScheduleDetails_Plan GetInstance()
    {
        if (model == null)
        {
            model = new Model_ScheduleDetails_Plan();
        }

        return model;
    }

    #endregion

    /// <summary> ViewModel - スケジュール詳細 </summary>
    public ViewModel_ScheduleDetails ViewModel_Header { get; set; }

    /// <summary> ViewModel - スケジュール詳細 (予定一覧) </summary>
    internal override ViewModel_ScheduleDetails_Plan ViewModel { get; set; }

    /// <summary> ViewModel - イメージビューワー </summary>
    internal ViewModel_ImageViewer ViewModel_ImageViewer { get; set; }

    public void Initialize()
    {
        var events = GoogleFacade.Calendar.FindByDate(this.ViewModel_Header.Date.Value);

        this.ViewModel.Events_ItemSource.Clear();

        var schedules = events.Where(x => x.Place != null);

        this.Clear_ScheduleView();

        foreach (var schedule in schedules)
        {
            if (schedule.IsAllDayEvent)
            {
                this.ViewModel.Events_ItemSource.Add(schedule);
            }

            this.WriteSchedule(schedule);
        }

        this.ListView_SelectionChanged();
    }

    private void WriteSchedule(CalendarEventsEntity entity)
    {
        this.SetTitleInSchedule(entity);

        DateTime nextTime = entity.StartDate;

        while (true)
        {
            nextTime = nextTime.AddMinutes(30);

            if (entity.EndDate <= nextTime)
            {
                return;
            }

            if (entity.IsAllDay)
            {
                return;
            }

            var updatedEntity = new CalendarEventsEntity(entity.Title, "↓", entity.StartDate, nextTime, entity.EndDate, entity.Place, entity.Description);

            this.SetTitleInSchedule(updatedEntity);
        }
    }

    /// <summary>
    /// スケジュールビューのリセット
    /// </summary>
    private void Clear_ScheduleView()
    {
        this.ViewModel.Time_6_1_Entity.Value        = CalendarEventsEntity.Empty;
        this.ViewModel.Time_6_2_Entity.Value        = CalendarEventsEntity.Empty;
        this.ViewModel.Time_6_3_Entity.Value        = CalendarEventsEntity.Empty;
        this.ViewModel.Time_6_4_Entity.Value        = CalendarEventsEntity.Empty;
        this.ViewModel.Time_6_5_Entity.Value        = CalendarEventsEntity.Empty;
        this.ViewModel.Time_6_30min_1_Entity.Value  = CalendarEventsEntity.Empty;
        this.ViewModel.Time_6_30min_2_Entity.Value  = CalendarEventsEntity.Empty;
        this.ViewModel.Time_6_30min_3_Entity.Value  = CalendarEventsEntity.Empty;
        this.ViewModel.Time_6_30min_4_Entity.Value  = CalendarEventsEntity.Empty;
        this.ViewModel.Time_6_30min_5_Entity.Value  = CalendarEventsEntity.Empty;

        this.ViewModel.Time_7_1_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_7_2_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_7_3_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_7_4_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_7_5_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_7_30min_1_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_7_30min_2_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_7_30min_3_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_7_30min_4_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_7_30min_5_Entity.Value = CalendarEventsEntity.Empty;

        this.ViewModel.Time_8_1_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_8_2_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_8_3_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_8_4_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_8_5_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_8_30min_1_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_8_30min_2_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_8_30min_3_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_8_30min_4_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_8_30min_5_Entity.Value = CalendarEventsEntity.Empty;

        this.ViewModel.Time_9_1_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_9_2_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_9_3_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_9_4_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_9_5_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_9_30min_1_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_9_30min_2_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_9_30min_3_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_9_30min_4_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_9_30min_5_Entity.Value = CalendarEventsEntity.Empty;

        this.ViewModel.Time_10_1_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_10_2_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_10_3_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_10_4_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_10_5_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_10_30min_1_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_10_30min_2_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_10_30min_3_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_10_30min_4_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_10_30min_5_Entity.Value = CalendarEventsEntity.Empty;

        this.ViewModel.Time_11_1_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_11_2_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_11_3_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_11_4_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_11_5_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_11_30min_1_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_11_30min_2_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_11_30min_3_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_11_30min_4_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_11_30min_5_Entity.Value = CalendarEventsEntity.Empty;

        this.ViewModel.Time_12_1_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_12_2_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_12_3_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_12_4_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_12_5_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_12_30min_1_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_12_30min_2_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_12_30min_3_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_12_30min_4_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_12_30min_5_Entity.Value = CalendarEventsEntity.Empty;

        this.ViewModel.Time_13_1_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_13_2_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_13_3_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_13_4_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_13_5_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_13_30min_1_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_13_30min_2_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_13_30min_3_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_13_30min_4_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_13_30min_5_Entity.Value = CalendarEventsEntity.Empty;

        this.ViewModel.Time_14_1_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_14_2_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_14_3_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_14_4_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_14_5_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_14_30min_1_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_14_30min_2_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_14_30min_3_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_14_30min_4_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_14_30min_5_Entity.Value = CalendarEventsEntity.Empty;

        this.ViewModel.Time_15_1_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_15_2_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_15_3_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_15_4_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_15_5_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_15_30min_1_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_15_30min_2_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_15_30min_3_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_15_30min_4_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_15_30min_5_Entity.Value = CalendarEventsEntity.Empty;

        this.ViewModel.Time_16_1_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_16_2_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_16_3_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_16_4_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_16_5_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_16_30min_1_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_16_30min_2_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_16_30min_3_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_16_30min_4_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_16_30min_5_Entity.Value = CalendarEventsEntity.Empty;

        this.ViewModel.Time_17_1_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_17_2_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_17_3_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_17_4_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_17_5_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_17_30min_1_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_17_30min_2_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_17_30min_3_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_17_30min_4_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_17_30min_5_Entity.Value = CalendarEventsEntity.Empty;

        this.ViewModel.Time_18_1_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_18_2_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_18_3_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_18_4_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_18_5_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_18_30min_1_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_18_30min_2_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_18_30min_3_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_18_30min_4_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_18_30min_5_Entity.Value = CalendarEventsEntity.Empty;

        this.ViewModel.Time_19_1_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_19_2_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_19_3_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_19_4_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_19_5_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_19_30min_1_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_19_30min_2_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_19_30min_3_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_19_30min_4_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_19_30min_5_Entity.Value = CalendarEventsEntity.Empty;

        this.ViewModel.Time_20_1_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_20_2_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_20_3_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_20_4_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_20_5_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_20_30min_1_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_20_30min_2_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_20_30min_3_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_20_30min_4_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_20_30min_5_Entity.Value = CalendarEventsEntity.Empty;

        this.ViewModel.Time_21_1_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_21_2_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_21_3_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_21_4_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_21_5_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_21_30min_1_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_21_30min_2_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_21_30min_3_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_21_30min_4_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_21_30min_5_Entity.Value = CalendarEventsEntity.Empty;

        this.ViewModel.Time_22_1_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_22_2_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_22_3_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_22_4_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_22_5_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_22_30min_1_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_22_30min_2_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_22_30min_3_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_22_30min_4_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_22_30min_5_Entity.Value = CalendarEventsEntity.Empty;

        this.ViewModel.Time_23_1_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_23_2_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_23_3_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_23_4_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_23_5_Entity.Value       = CalendarEventsEntity.Empty;
        this.ViewModel.Time_23_30min_1_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_23_30min_2_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_23_30min_3_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_23_30min_4_Entity.Value = CalendarEventsEntity.Empty;
        this.ViewModel.Time_23_30min_5_Entity.Value = CalendarEventsEntity.Empty;
    }

    /// <summary>
    /// スケジュールビューの設定
    /// </summary>
    /// <param name="hour">時</param>
    /// <param name="minute">分</param>
    /// <param name="updatingEntity">タイトル</param>
    private void SetTitleInSchedule(CalendarEventsEntity updatingEntity)
    {
        bool IsWritten = false;

        var hour   = updatingEntity.StartDate.Hour;
        var minute = updatingEntity.StartDate.Minute;

        switch (hour)
        {
            case 6:
                if (minute < 30)
                {
                    this.ViewModel.Time_6_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_6_1_Entity.Value);
                    this.ViewModel.Time_6_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_6_2_Entity.Value);
                    this.ViewModel.Time_6_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_6_3_Entity.Value);
                    this.ViewModel.Time_6_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_6_4_Entity.Value);
                    this.ViewModel.Time_6_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_6_5_Entity.Value);
                }
                else
                {
                    this.ViewModel.Time_6_30min_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_6_30min_1_Entity.Value);
                    this.ViewModel.Time_6_30min_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_6_30min_2_Entity.Value);
                    this.ViewModel.Time_6_30min_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_6_30min_3_Entity.Value);
                    this.ViewModel.Time_6_30min_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_6_30min_4_Entity.Value);
                    this.ViewModel.Time_6_30min_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_6_30min_5_Entity.Value);
                }
                break;

            case 7:
                if (minute < 30)
                {
                    this.ViewModel.Time_7_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_7_1_Entity.Value);
                    this.ViewModel.Time_7_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_7_2_Entity.Value);
                    this.ViewModel.Time_7_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_7_3_Entity.Value);
                    this.ViewModel.Time_7_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_7_4_Entity.Value);
                    this.ViewModel.Time_7_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_7_5_Entity.Value);
                }
                else
                {
                    this.ViewModel.Time_7_30min_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_7_30min_1_Entity.Value);
                    this.ViewModel.Time_7_30min_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_7_30min_2_Entity.Value);
                    this.ViewModel.Time_7_30min_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_7_30min_3_Entity.Value);
                    this.ViewModel.Time_7_30min_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_7_30min_4_Entity.Value);
                    this.ViewModel.Time_7_30min_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_7_30min_5_Entity.Value);
                }
                break;

            case 8:
                if (minute < 30)
                {
                    this.ViewModel.Time_8_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_8_1_Entity.Value);
                    this.ViewModel.Time_8_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_8_2_Entity.Value);
                    this.ViewModel.Time_8_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_8_3_Entity.Value);
                    this.ViewModel.Time_8_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_8_4_Entity.Value);
                    this.ViewModel.Time_8_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_8_5_Entity.Value);
                }
                else
                {
                    this.ViewModel.Time_8_30min_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_8_30min_1_Entity.Value);
                    this.ViewModel.Time_8_30min_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_8_30min_2_Entity.Value);
                    this.ViewModel.Time_8_30min_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_8_30min_3_Entity.Value);
                    this.ViewModel.Time_8_30min_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_8_30min_4_Entity.Value);
                    this.ViewModel.Time_8_30min_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_8_30min_5_Entity.Value);
                }
                break;

            case 9:
                if (minute < 30)
                {
                    this.ViewModel.Time_9_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_9_1_Entity.Value);
                    this.ViewModel.Time_9_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_9_2_Entity.Value);
                    this.ViewModel.Time_9_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_9_3_Entity.Value);
                    this.ViewModel.Time_9_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_9_4_Entity.Value);
                    this.ViewModel.Time_9_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_9_5_Entity.Value);
                }
                else
                {
                    this.ViewModel.Time_9_30min_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_9_30min_1_Entity.Value);
                    this.ViewModel.Time_9_30min_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_9_30min_2_Entity.Value);
                    this.ViewModel.Time_9_30min_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_9_30min_3_Entity.Value);
                    this.ViewModel.Time_9_30min_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_9_30min_4_Entity.Value);
                    this.ViewModel.Time_9_30min_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_9_30min_5_Entity.Value);
                }
                break;

            case 10:
                if (minute < 30)
                {
                    this.ViewModel.Time_10_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_10_1_Entity.Value);
                    this.ViewModel.Time_10_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_10_2_Entity.Value);
                    this.ViewModel.Time_10_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_10_3_Entity.Value);
                    this.ViewModel.Time_10_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_10_4_Entity.Value);
                    this.ViewModel.Time_10_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_10_5_Entity.Value);
                }
                else
                {
                    this.ViewModel.Time_10_30min_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_10_30min_1_Entity.Value);
                    this.ViewModel.Time_10_30min_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_10_30min_2_Entity.Value);
                    this.ViewModel.Time_10_30min_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_10_30min_3_Entity.Value);
                    this.ViewModel.Time_10_30min_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_10_30min_4_Entity.Value);
                    this.ViewModel.Time_10_30min_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_10_30min_5_Entity.Value);
                }
                break;

            case 11:
                if (minute < 30)
                {
                    this.ViewModel.Time_11_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_11_1_Entity.Value);
                    this.ViewModel.Time_11_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_11_2_Entity.Value);
                    this.ViewModel.Time_11_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_11_3_Entity.Value);
                    this.ViewModel.Time_11_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_11_4_Entity.Value);
                    this.ViewModel.Time_11_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_11_5_Entity.Value);
                }
                else
                {
                    this.ViewModel.Time_11_30min_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_11_30min_1_Entity.Value);
                    this.ViewModel.Time_11_30min_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_11_30min_2_Entity.Value);
                    this.ViewModel.Time_11_30min_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_11_30min_3_Entity.Value);
                    this.ViewModel.Time_11_30min_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_11_30min_4_Entity.Value);
                    this.ViewModel.Time_11_30min_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_11_30min_5_Entity.Value);
                }
                break;

            case 12:
                if (minute < 30)
                {
                    this.ViewModel.Time_12_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_12_1_Entity.Value);
                    this.ViewModel.Time_12_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_12_2_Entity.Value);
                    this.ViewModel.Time_12_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_12_3_Entity.Value);
                    this.ViewModel.Time_12_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_12_4_Entity.Value);
                    this.ViewModel.Time_12_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_12_5_Entity.Value);
                }
                else
                {
                    this.ViewModel.Time_12_30min_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_12_30min_1_Entity.Value);
                    this.ViewModel.Time_12_30min_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_12_30min_2_Entity.Value);
                    this.ViewModel.Time_12_30min_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_12_30min_3_Entity.Value);
                    this.ViewModel.Time_12_30min_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_12_30min_4_Entity.Value);
                    this.ViewModel.Time_12_30min_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_12_30min_5_Entity.Value);
                }
                break;

            case 13:
                if (minute < 30)
                {
                    this.ViewModel.Time_13_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_13_1_Entity.Value);
                    this.ViewModel.Time_13_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_13_2_Entity.Value);
                    this.ViewModel.Time_13_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_13_3_Entity.Value);
                    this.ViewModel.Time_13_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_13_4_Entity.Value);
                    this.ViewModel.Time_13_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_13_5_Entity.Value);
                }
                else
                {
                    this.ViewModel.Time_13_30min_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_13_30min_1_Entity.Value);
                    this.ViewModel.Time_13_30min_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_13_30min_2_Entity.Value);
                    this.ViewModel.Time_13_30min_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_13_30min_3_Entity.Value);
                    this.ViewModel.Time_13_30min_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_13_30min_4_Entity.Value);
                    this.ViewModel.Time_13_30min_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_13_30min_5_Entity.Value);
                }
                break;

            case 14:
                if (minute < 30)
                {
                    this.ViewModel.Time_14_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_14_1_Entity.Value);
                    this.ViewModel.Time_14_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_14_2_Entity.Value);
                    this.ViewModel.Time_14_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_14_3_Entity.Value);
                    this.ViewModel.Time_14_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_14_4_Entity.Value);
                    this.ViewModel.Time_14_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_14_5_Entity.Value);
                }
                else
                {
                    this.ViewModel.Time_14_30min_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_14_30min_1_Entity.Value);
                    this.ViewModel.Time_14_30min_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_14_30min_2_Entity.Value);
                    this.ViewModel.Time_14_30min_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_14_30min_3_Entity.Value);
                    this.ViewModel.Time_14_30min_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_14_30min_4_Entity.Value);
                    this.ViewModel.Time_14_30min_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_14_30min_5_Entity.Value);
                }
                break;

            case 15:
                if (minute < 30)
                {
                    this.ViewModel.Time_15_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_15_1_Entity.Value);
                    this.ViewModel.Time_15_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_15_2_Entity.Value);
                    this.ViewModel.Time_15_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_15_3_Entity.Value);
                    this.ViewModel.Time_15_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_15_4_Entity.Value);
                    this.ViewModel.Time_15_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_15_5_Entity.Value);
                }
                else
                {
                    this.ViewModel.Time_15_30min_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_15_30min_1_Entity.Value);
                    this.ViewModel.Time_15_30min_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_15_30min_2_Entity.Value);
                    this.ViewModel.Time_15_30min_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_15_30min_3_Entity.Value);
                    this.ViewModel.Time_15_30min_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_15_30min_4_Entity.Value);
                    this.ViewModel.Time_15_30min_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_15_30min_5_Entity.Value);
                }
                break;

            case 16:
                if (minute < 30)
                {
                    this.ViewModel.Time_16_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_16_1_Entity.Value);
                    this.ViewModel.Time_16_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_16_2_Entity.Value);
                    this.ViewModel.Time_16_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_16_3_Entity.Value);
                    this.ViewModel.Time_16_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_16_4_Entity.Value);
                    this.ViewModel.Time_16_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_16_5_Entity.Value);
                }
                else
                {
                    this.ViewModel.Time_16_30min_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_16_30min_1_Entity.Value);
                    this.ViewModel.Time_16_30min_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_16_30min_2_Entity.Value);
                    this.ViewModel.Time_16_30min_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_16_30min_3_Entity.Value);
                    this.ViewModel.Time_16_30min_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_16_30min_4_Entity.Value);
                    this.ViewModel.Time_16_30min_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_16_30min_5_Entity.Value);
                }
                break;

            case 17:
                if (minute < 30)
                {
                    this.ViewModel.Time_17_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_17_1_Entity.Value);
                    this.ViewModel.Time_17_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_17_2_Entity.Value);
                    this.ViewModel.Time_17_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_17_3_Entity.Value);
                    this.ViewModel.Time_17_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_17_4_Entity.Value);
                    this.ViewModel.Time_17_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_17_5_Entity.Value);
                }
                else
                {
                    this.ViewModel.Time_17_30min_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_17_30min_1_Entity.Value);
                    this.ViewModel.Time_17_30min_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_17_30min_2_Entity.Value);
                    this.ViewModel.Time_17_30min_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_17_30min_3_Entity.Value);
                    this.ViewModel.Time_17_30min_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_17_30min_4_Entity.Value);
                    this.ViewModel.Time_17_30min_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_17_30min_5_Entity.Value);
                }
                break;

            case 18:
                if (minute < 30)
                {
                    this.ViewModel.Time_18_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_18_1_Entity.Value);
                    this.ViewModel.Time_18_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_18_2_Entity.Value);
                    this.ViewModel.Time_18_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_18_3_Entity.Value);
                    this.ViewModel.Time_18_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_18_4_Entity.Value);
                    this.ViewModel.Time_18_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_18_5_Entity.Value);
                }
                else
                {
                    this.ViewModel.Time_18_30min_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_18_30min_1_Entity.Value);
                    this.ViewModel.Time_18_30min_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_18_30min_2_Entity.Value);
                    this.ViewModel.Time_18_30min_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_18_30min_3_Entity.Value);
                    this.ViewModel.Time_18_30min_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_18_30min_4_Entity.Value);
                    this.ViewModel.Time_18_30min_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_18_30min_5_Entity.Value);
                }
                break;

            case 19:
                if (minute < 30)
                {
                    this.ViewModel.Time_19_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_19_1_Entity.Value);
                    this.ViewModel.Time_19_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_19_2_Entity.Value);
                    this.ViewModel.Time_19_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_19_3_Entity.Value);
                    this.ViewModel.Time_19_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_19_4_Entity.Value);
                    this.ViewModel.Time_19_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_19_5_Entity.Value);
                }
                else
                {
                    this.ViewModel.Time_19_30min_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_19_30min_1_Entity.Value);
                    this.ViewModel.Time_19_30min_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_19_30min_2_Entity.Value);
                    this.ViewModel.Time_19_30min_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_19_30min_3_Entity.Value);
                    this.ViewModel.Time_19_30min_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_19_30min_4_Entity.Value);
                    this.ViewModel.Time_19_30min_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_19_30min_5_Entity.Value);
                }
                break;

            case 20:
                if (minute < 30)
                {
                    this.ViewModel.Time_20_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_20_1_Entity.Value);
                    this.ViewModel.Time_20_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_20_2_Entity.Value);
                    this.ViewModel.Time_20_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_20_3_Entity.Value);
                    this.ViewModel.Time_20_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_20_4_Entity.Value);
                    this.ViewModel.Time_20_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_20_5_Entity.Value);
                }
                else
                {
                    this.ViewModel.Time_20_30min_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_20_30min_1_Entity.Value);
                    this.ViewModel.Time_20_30min_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_20_30min_2_Entity.Value);
                    this.ViewModel.Time_20_30min_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_20_30min_3_Entity.Value);
                    this.ViewModel.Time_20_30min_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_20_30min_4_Entity.Value);
                    this.ViewModel.Time_20_30min_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_20_30min_5_Entity.Value);
                }
                break;

            case 21:
                if (minute < 30)
                {
                    this.ViewModel.Time_21_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_21_1_Entity.Value);
                    this.ViewModel.Time_21_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_21_2_Entity.Value);
                    this.ViewModel.Time_21_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_21_3_Entity.Value);
                    this.ViewModel.Time_21_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_21_4_Entity.Value);
                    this.ViewModel.Time_21_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_21_5_Entity.Value);
                }
                else
                {
                    this.ViewModel.Time_21_30min_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_21_30min_1_Entity.Value);
                    this.ViewModel.Time_21_30min_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_21_30min_2_Entity.Value);
                    this.ViewModel.Time_21_30min_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_21_30min_3_Entity.Value);
                    this.ViewModel.Time_21_30min_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_21_30min_4_Entity.Value);
                    this.ViewModel.Time_21_30min_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_21_30min_5_Entity.Value);
                }
                break;

            case 22:
                if (minute < 30)
                {
                    this.ViewModel.Time_22_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_22_1_Entity.Value);
                    this.ViewModel.Time_22_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_22_2_Entity.Value);
                    this.ViewModel.Time_22_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_22_3_Entity.Value);
                    this.ViewModel.Time_22_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_22_4_Entity.Value);
                    this.ViewModel.Time_22_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_22_5_Entity.Value);
                }
                else
                {
                    this.ViewModel.Time_22_30min_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_22_30min_1_Entity.Value);
                    this.ViewModel.Time_22_30min_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_22_30min_2_Entity.Value);
                    this.ViewModel.Time_22_30min_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_22_30min_3_Entity.Value);
                    this.ViewModel.Time_22_30min_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_22_30min_4_Entity.Value);
                    this.ViewModel.Time_22_30min_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_22_30min_5_Entity.Value);
                }
                break;

            case 23:
                if (minute < 30)
                {
                    this.ViewModel.Time_23_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_23_1_Entity.Value);
                    this.ViewModel.Time_23_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_23_2_Entity.Value);
                    this.ViewModel.Time_23_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_23_3_Entity.Value);
                    this.ViewModel.Time_23_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_23_4_Entity.Value);
                    this.ViewModel.Time_23_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_23_5_Entity.Value);
                }
                else
                {
                    this.ViewModel.Time_23_30min_1_Entity.Value = WriteIfEmpty(this.ViewModel.Time_23_30min_1_Entity.Value);
                    this.ViewModel.Time_23_30min_2_Entity.Value = WriteIfEmpty(this.ViewModel.Time_23_30min_2_Entity.Value);
                    this.ViewModel.Time_23_30min_3_Entity.Value = WriteIfEmpty(this.ViewModel.Time_23_30min_3_Entity.Value);
                    this.ViewModel.Time_23_30min_4_Entity.Value = WriteIfEmpty(this.ViewModel.Time_23_30min_4_Entity.Value);
                    this.ViewModel.Time_23_30min_5_Entity.Value = WriteIfEmpty(this.ViewModel.Time_23_30min_5_Entity.Value);
                }
                break;
        }

        CalendarEventsEntity WriteIfEmpty(CalendarEventsEntity entity)
        {
            if (entity is null)
            {
                return entity;
            }

            if (string.IsNullOrEmpty(entity.Title) && !IsWritten)
            {
                IsWritten = true;
                return updatingEntity;
            }

            return entity;
        }
    }

    /// <summary>
    /// 予定一覧 - SelectionChanged
    /// </summary>
    public void ListView_SelectionChanged()
    {
        using(new CursorWaiting())
        {
            if (this.ViewModel.Events_ItemSource.IsEmpty())
            {
                // リストが空
                return;
            }

            if (this.ViewModel.Events_SelectedIndex.Value.IsUnSelected())
            {
                // 未選択
                return;
            }

            var entity = this.ViewModel.Events_ItemSource[this.ViewModel.Events_SelectedIndex.Value];

            // タイトル
            this.ViewModel.Title_Text.Value = entity.Title;
            // 開始時刻
            this.ViewModel.StartTime_Text.Value = entity.StartDate.ToString("HH:mm");
            // 終了時刻
            this.ViewModel.EndTime_Text.Value = entity.EndDate.ToString("HH:mm");
            // 場所
            this.ViewModel.Place_Text.Value = entity.Place;
            // 詳細
            this.ViewModel.Description_Text.Value = entity.Description;

            // 地図情報
            //this.ShowMapImage();

            // 写真
            var photo = JSONExtension.GetPhotoSource(this.ViewModel.Place_Text.Value);

            if (photo.Image != null)
            {
                this.ViewModel.Photo_Source.Value = photo.Image;
                this.ViewModel.Photo_Height.Value = photo.Height;
                this.ViewModel.Photo_Width.Value = photo.Width;
            }
        }
    }

    public void ShowDetails(CalendarEventsEntity entity)
    {
        // タイトル
        this.ViewModel.Title_Text.Value       = entity.Title;
        // 開始時刻
        if (entity.DisplayTitle == "↓")
        {
            this.ViewModel.StartTime_Text.Value = entity.ProgressingStartDate.ToString("HH:mm");
        }
        else
        {
            this.ViewModel.StartTime_Text.Value = entity.StartDate.ToString("HH:mm");
        }
       
        // 終了時刻
        this.ViewModel.EndTime_Text.Value     = entity.EndDate.ToString("HH:mm");
        // 場所
        this.ViewModel.Place_Text.Value       = entity.Place;
        // 詳細
        this.ViewModel.Description_Text.Value = entity.Description;
    }

    /// <summary>
    /// Clear - 閲覧項目
    /// </summary>
    public void Clear_ViewForm()
    {
        // 地図
        this.ViewModel.Map_Source.Value   = new BitmapImage();
        // 写真
        this.ViewModel.Photo_Source.Value = new BitmapImage();

        // タイトル
        this.ViewModel.Title_Text.Value       = string.Empty;
        // 開始時刻
        this.ViewModel.StartTime_Text.Value   = string.Empty;
        // 終了時刻
        this.ViewModel.EndTime_Text.Value     = string.Empty;
        // 場所
        this.ViewModel.Place_Text.Value       = string.Empty;
        // 詳細
        this.ViewModel.Description_Text.Value = string.Empty;
    }

    /// <summary>
    /// 地図のイメージを取得する
    /// </summary>
    private async void ShowMapImage()
    {
        //var imageUrl = GetImageurl();
        var imageUrl = GoogleFacade.Place.ReadLocation2(this.ViewModel.Place_Text.Value);

        if (string.IsNullOrEmpty(imageUrl.Result)) 
        {
            // 住所が未指定
            return; 
        }

        try
        {
            // Webリクエストを送信して地図画像を取得
            WebClient webClient = new WebClient();
            byte[] imageBytes   = await Task.Run(() => webClient.DownloadData(imageUrl.Result));

            // 地図画像をBitmapImageに変換
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(imageBytes);
            bitmapImage.EndInit();

            // Imageコントロールに地図画像を表示
            this.ViewModel.Map_Source.Value = bitmapImage;
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Error: {ex.Message}");
        }
    }

    /// <summary>
    /// 住所からGoogle Place APIのURLを取得する
    /// </summary>
    /// <returns>URL</returns>
    private string GetImageurl()
    {
        // 地図情報
        var location = GoogleFacade.Place.ReadLocation(this.ViewModel.Place_Text.Value);

        if (location == (double.MinValue, double.MinValue))
        {
            // 住所が未指定
            return string.Empty;
        }

        // Google Maps Static APIのURLを構築
        string latitude  = location.Latitude.Value.ToString(); // 緯度
        string longitude = location.Longitude.Value.ToString(); // 経度
        int zoom         = 15; // ズームレベル

        return $"https://maps.googleapis.com/maps/api/staticmap?center={latitude},{longitude}&zoom={zoom}&size=600x400&markers=color:red%7C{latitude},{longitude}&key={Shared.API_Key}";
    }

    /// <summary>
    /// イメージビューアーを開く
    /// </summary>
    /// <param name="title">タイトル</param>
    /// <param name="height">高さ</param>
    /// <param name="width">幅</param>
    /// <param name="image">画像</param>
    internal void OpenImageViewer(string title, double height, double width, ImageSource image)
    {
        this.ViewModel_ImageViewer = new ViewModel_ImageViewer();

        var viewer = new ImageViewer();

        this.ViewModel_ImageViewer.Window_Title.Value = title;
        this.ViewModel_ImageViewer.Image_Height.Value = height;
        this.ViewModel_ImageViewer.Image_Width.Value  = width;
        this.ViewModel_ImageViewer.Image_Source.Value = image;

        viewer.Show();
    }
}
