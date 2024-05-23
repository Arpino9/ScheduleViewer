using ScheduleViewer.Infrastructure.Google_Calendar;

namespace ScheduleViewer.WPF.Models;

/// <summary>
/// Model - スケジュール詳細 (予定一覧)
/// </summary>
public sealed class Model_ScheduleDetails_Plan
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

    /// <summary> ViewModel - スケジュール詳細 (予定一覧) </summary>
    public ViewModel_ScheduleDetails_Plan ViewModel { get; set; }

    internal DateTime Date { get; set; }

    internal void Initialize()
    {
        var events = CalendarReader.FindByDate(Date);

        this.ViewModel.Events_ItemSource.Clear();

        this.ViewModel.Events_ItemSource = events.ToReactiveCollection();

        this.ViewModel.Events_SelectedIndex.Value = 0;
    }

    /// <summary>
    /// 経歴 - SelectionChanged
    /// </summary>
    public void Plans_SelectionChanged()
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
