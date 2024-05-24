namespace ScheduleViewer.WPF.Models;

/// <summary>
/// Model - スケジュール詳細 (予定一覧)
/// </summary>
public sealed class Model_ScheduleDetails_Plan : ModelBase, IViewer
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
    public ViewModel_ScheduleDetails_Plan ViewModel { get; set; }

    internal override void Initialize()
    {
        var events = CalendarReader.FindByDate(this.ViewModel_Header.Date.Value);

        this.ViewModel.Events_ItemSource.Clear();

        var schedule = events.Where(x => x.Place != string.Empty);
        this.ViewModel.Events_ItemSource = schedule.ToReactiveCollection(this.ViewModel.Events_ItemSource);

        this.ViewModel.Events_SelectedIndex.Value = 0;
    }

    /// <summary>
    /// 予定一覧 - SelectionChanged
    /// </summary>
    public void ListView_SelectionChanged()
    {
        if (this.ViewModel.Events_SelectedIndex.Value.IsUnSelected())
        {
            return;
        }

        var entity = this.ViewModel.Events_ItemSource[this.ViewModel.Events_SelectedIndex.Value];

        // タイトル
        this.ViewModel.Title_Text.Value       = entity.Title;
        // 開始時刻
        this.ViewModel.StartTime_Text.Value   = entity.StartDate;
        // 終了時刻
        this.ViewModel.EndTime_Text.Value     = entity.EndDate;
        // 場所
        this.ViewModel.Place_Text.Value       = entity.Place;
        // 詳細
        this.ViewModel.Description_Text.Value = entity.Description;
    }
}
