
namespace ScheduleViewer.WPF.ViewModels;

public sealed class ViewModel_Schedule_Table : ViewModelBase<Model_Schedule>
{
    protected override Model_Schedule Model => Model_Schedule.GetInstance();

    public override event PropertyChangedEventHandler PropertyChanged;

    public ViewModel_Schedule_Table()
    {
        this.Model.ViewModel_Table = this;

        using (var _ = new CursorWaiting())
        {
            this.Model.Initialize_TableAsync();
        }
    }

    protected override void BindEvents()
    {
        throw new NotImplementedException();
    }

    #region 月曜日

    /// <summary> 月曜日 - 第1週 </summary>
    public ReactiveProperty<string> Monday_WeekNum1 { get; set; } = new ReactiveProperty<string>();

    /// <summary> 月曜日 - 第2週 </summary>
    public ReactiveProperty<string> Monday_WeekNum2 { get; set; } = new ReactiveProperty<string>();

    /// <summary> 月曜日 - 第3週 </summary>
    public ReactiveProperty<string> Monday_WeekNum3 { get; set; } = new ReactiveProperty<string>();
    
    /// <summary> 月曜日 - 第4週 </summary>
    public ReactiveProperty<string> Monday_WeekNum4 { get; set; } = new ReactiveProperty<string>();
    
    /// <summary> 月曜日 - 第5週 </summary>
    public ReactiveProperty<string> Monday_WeekNum5 { get; set; } = new ReactiveProperty<string>();

    #endregion

    #region 火曜日

    /// <summary> 火曜日 - 第1週 </summary>
    public ReactiveProperty<string> Tuesday_WeekNum1 { get; set; } = new ReactiveProperty<string>();

    /// <summary> 火曜日 - 第2週 </summary>
    public ReactiveProperty<string> Tuesday_WeekNum2 { get; set; } = new ReactiveProperty<string>();

    /// <summary> 火曜日 - 第3週 </summary>
    public ReactiveProperty<string> Tuesday_WeekNum3 { get; set; } = new ReactiveProperty<string>();

    /// <summary> 火曜日 - 第4週 </summary>
    public ReactiveProperty<string> Tuesday_WeekNum4 { get; set; } = new ReactiveProperty<string>();

    /// <summary> 火曜日 - 第5週 </summary>
    public ReactiveProperty<string> Tuesday_WeekNum5 { get; set; } = new ReactiveProperty<string>();

    #endregion

    #region 水曜日

    /// <summary> 水曜日 - 第1週 </summary>
    public ReactiveProperty<string> Wednesday_WeekNum1 { get; set; } = new ReactiveProperty<string>();

    /// <summary> 水曜日 - 第2週 </summary>
    public ReactiveProperty<string> Wednesday_WeekNum2 { get; set; } = new ReactiveProperty<string>();

    /// <summary> 水曜日 - 第3週 </summary>
    public ReactiveProperty<string> Wednesday_WeekNum3 { get; set; } = new ReactiveProperty<string>();

    /// <summary> 水曜日 - 第4週 </summary>
    public ReactiveProperty<string> Wednesday_WeekNum4 { get; set; } = new ReactiveProperty<string>();

    /// <summary> 水曜日 - 第5週 </summary>
    public ReactiveProperty<string> Wednesday_WeekNum5 { get; set; } = new ReactiveProperty<string>();

    #endregion

    #region 木曜日

    /// <summary> 木曜日 - 第1週 </summary>
    public ReactiveProperty<string> Thursday_WeekNum1 { get; set; } = new ReactiveProperty<string>();

    /// <summary> 木曜日 - 第2週 </summary>
    public ReactiveProperty<string> Thursday_WeekNum2 { get; set; } = new ReactiveProperty<string>();

    /// <summary> 木曜日 - 第3週 </summary>
    public ReactiveProperty<string> Thursday_WeekNum3 { get; set; } = new ReactiveProperty<string>();

    /// <summary> 木曜日 - 第4週 </summary>
    public ReactiveProperty<string> Thursday_WeekNum4 { get; set; } = new ReactiveProperty<string>();

    /// <summary> 木曜日 - 第5週 </summary>
    public ReactiveProperty<string> Thursday_WeekNum5 { get; set; } = new ReactiveProperty<string>();

    #endregion

    #region 金曜日

    /// <summary> 金曜日 - 第1週 </summary>
    public ReactiveProperty<string> Friday_WeekNum1 { get; set; } = new ReactiveProperty<string>();

    /// <summary> 金曜日 - 第2週 </summary>
    public ReactiveProperty<string> Friday_WeekNum2 { get; set; } = new ReactiveProperty<string>();

    /// <summary> 金曜日 - 第3週 </summary>
    public ReactiveProperty<string> Friday_WeekNum3 { get; set; } = new ReactiveProperty<string>();

    /// <summary> 金曜日 - 第4週 </summary>
    public ReactiveProperty<string> Friday_WeekNum4 { get; set; } = new ReactiveProperty<string>();

    /// <summary> 金曜日 - 第5週 </summary>
    public ReactiveProperty<string> Friday_WeekNum5 { get; set; } = new ReactiveProperty<string>();

    #endregion

    #region 土曜日

    /// <summary> 土曜日 - 第1週 </summary>
    public ReactiveProperty<string> Saturday_WeekNum1 { get; set; } = new ReactiveProperty<string>();

    /// <summary> 土曜日 - 第2週 </summary>
    public ReactiveProperty<string> Saturday_WeekNum2 { get; set; } = new ReactiveProperty<string>();

    /// <summary> 土曜日 - 第3週 </summary>
    public ReactiveProperty<string> Saturday_WeekNum3 { get; set; } = new ReactiveProperty<string>();

    /// <summary> 土曜日 - 第4週 </summary>
    public ReactiveProperty<string> Saturday_WeekNum4 { get; set; } = new ReactiveProperty<string>();

    /// <summary> 土曜日 - 第5週 </summary>
    public ReactiveProperty<string> Saturday_WeekNum5 { get; set; } = new ReactiveProperty<string>();

    #endregion

    #region 日曜日

    /// <summary> 日曜日 - 第1週 </summary>
    public ReactiveProperty<string> Sunday_WeekNum1 { get; set; } = new ReactiveProperty<string>();

    /// <summary> 日曜日 - 第2週 </summary>
    public ReactiveProperty<string> Sunday_WeekNum2 { get; set; } = new ReactiveProperty<string>();

    /// <summary> 日曜日 - 第3週 </summary>
    public ReactiveProperty<string> Sunday_WeekNum3 { get; set; } = new ReactiveProperty<string>();

    /// <summary> 日曜日 - 第4週 </summary>
    public ReactiveProperty<string> Sunday_WeekNum4 { get; set; } = new ReactiveProperty<string>();

    /// <summary> 日曜日 - 第5週 </summary>
    public ReactiveProperty<string> Sunday_WeekNum5 { get; set; } = new ReactiveProperty<string>();

    #endregion

}
