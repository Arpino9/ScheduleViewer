
namespace ScheduleViewer.WPF.ViewModels;

/// <summary>
/// ViewModel - スケジュール - テーブル
/// </summary>
public sealed class ViewModel_Schedule_Table : ViewModelBase<Model_Schedule>
{
    protected override Model_Schedule Model => Model_Schedule.GetInstance();

    public override event PropertyChangedEventHandler PropertyChanged;

    /// <summary> 実行中判定 </summary>
    public CursorWaiting CursorWaiting { get; set; }

    public ViewModel_Schedule_Table()
    {
        this.Model.ViewModel_Table = this;

        using (this.CursorWaiting = new CursorWaiting())
        {
            this.BindEvents();

            this.Model.Initialize_TableAsync();
        }
    }

    protected override void BindEvents()
    {
        // 第1週
        this.WeekNum1_Monday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum1_Monday.Value.Day_Text));
        this.WeekNum1_Tuesday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum1_Tuesday.Value.Day_Text));
        this.WeekNum1_Wednesday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum1_Wednesday.Value.Day_Text));
        this.WeekNum1_Thursday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum1_Thursday.Value.Day_Text));
        this.WeekNum1_Friday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum1_Friday.Value.Day_Text));
        this.WeekNum1_Saturday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum1_Saturday.Value.Day_Text));
        this.WeekNum1_Sunday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum1_Sunday.Value.Day_Text));
        
        // 第2週
        this.WeekNum2_Monday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum2_Monday.Value.Day_Text));
        this.WeekNum2_Tuesday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum2_Tuesday.Value.Day_Text));
        this.WeekNum2_Wednesday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum2_Wednesday.Value.Day_Text));
        this.WeekNum2_Thursday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum2_Thursday.Value.Day_Text));
        this.WeekNum2_Friday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum2_Friday.Value.Day_Text));
        this.WeekNum2_Saturday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum2_Saturday.Value.Day_Text));
        this.WeekNum2_Sunday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum2_Sunday.Value.Day_Text));

        // 第3週
        this.WeekNum3_Monday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum3_Monday.Value.Day_Text));
        this.WeekNum3_Tuesday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum3_Tuesday.Value.Day_Text));
        this.WeekNum3_Wednesday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum3_Wednesday.Value.Day_Text));
        this.WeekNum3_Thursday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum3_Thursday.Value.Day_Text));
        this.WeekNum3_Friday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum3_Friday.Value.Day_Text));
        this.WeekNum3_Saturday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum3_Saturday.Value.Day_Text));
        this.WeekNum3_Sunday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum3_Sunday.Value.Day_Text));

        // 第4週
        this.WeekNum4_Monday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum4_Monday.Value.Day_Text));
        this.WeekNum4_Tuesday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum4_Tuesday.Value.Day_Text));
        this.WeekNum4_Wednesday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum4_Wednesday.Value.Day_Text));
        this.WeekNum4_Thursday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum4_Thursday.Value.Day_Text));
        this.WeekNum4_Friday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum4_Friday.Value.Day_Text));
        this.WeekNum4_Saturday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum4_Saturday.Value.Day_Text));
        this.WeekNum4_Sunday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum4_Sunday.Value.Day_Text));

        // 第5週
        this.WeekNum5_Monday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum5_Monday.Value.Day_Text));
        this.WeekNum5_Tuesday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum5_Tuesday.Value.Day_Text));
        this.WeekNum5_Wednesday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum5_Wednesday.Value.Day_Text));
        this.WeekNum5_Thursday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum5_Thursday.Value.Day_Text));
        this.WeekNum5_Friday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum5_Friday.Value.Day_Text));
        this.WeekNum5_Saturday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum5_Saturday.Value.Day_Text));
        this.WeekNum5_Sunday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum5_Sunday.Value.Day_Text));
    }

    #region 第1週 - 月曜日

    /// <summary> 第1週 - 月曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum1_Monday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第1週 - 月曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum1_Monday_MouseDoubleClick { get; private set; } = new ReactiveCommand();
    
    #endregion

    #region 第1週 - 火曜日

    /// <summary> 第1週 - 火曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum1_Tuesday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第1週 - 火曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum1_Tuesday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第1週 - 水曜日

    /// <summary> 第1週 - 水曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum1_Wednesday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第1週 - 水曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum1_Wednesday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第1週 - 木曜日

    /// <summary> 第1週 - 木曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum1_Thursday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第1週 - 木曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum1_Thursday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第1週 - 金曜日

    /// <summary> 第1週 - 金曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum1_Friday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第1週 - 金曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum1_Friday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第1週 - 土曜日

    /// <summary> 第1週 - 土曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum1_Saturday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第1週 - 土曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum1_Saturday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第1週 - 日曜日

    /// <summary> 第1週 - 土曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum1_Sunday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第1週 - 日曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum1_Sunday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第2週 - 月曜日

    /// <summary> 第2週 - 月曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum2_Monday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第2週 - 月曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum2_Monday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第2週 - 火曜日

    /// <summary> 第2週 - 火曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum2_Tuesday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第2週 - 火曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum2_Tuesday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第2週 - 水曜日

    /// <summary> 第2週 - 水曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum2_Wednesday { get; set; } = new ReactiveProperty<ScheduleEntity>();
    
    /// <summary> 第2週 - 水曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum2_Wednesday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第2週 - 木曜日

    /// <summary> 第2週 - 木曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum2_Thursday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第2週 - 木曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum2_Thursday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第2週 - 金曜日

    /// <summary> 第2週 - 金曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum2_Friday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第2週 - 金曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum2_Friday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第2週 - 土曜日

    /// <summary> 第2週 - 土曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum2_Saturday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第2週 - 土曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum2_Saturday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第2週 - 日曜日

    /// <summary> 第2週 - 日曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum2_Sunday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第2週 - 日曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum2_Sunday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第3週 - 月曜日

    /// <summary> 第2週 - 月曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum3_Monday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第3週 - 日曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum3_Monday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第3週 - 火曜日

    /// <summary> 第2週 - 火曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum3_Tuesday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第3週 - 日曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum3_Tuesday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第3週 - 水曜日

    /// <summary> 第2週 - 水曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum3_Wednesday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第3週 - 水曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum3_Wednesday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第3週 - 木曜日

    /// <summary> 第2週 - 木曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum3_Thursday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第3週 - 木曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum3_Thursday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第3週 - 金曜日

    /// <summary> 第3週 - 金曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum3_Friday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第3週 - 金曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum3_Friday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第3週 - 土曜日

    /// <summary> 第3週 - 土曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum3_Saturday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第3週 - 土曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum3_Saturday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第3週 - 日曜日

    /// <summary> 第3週 - 日曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum3_Sunday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第3週 - 日曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum3_Sunday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第4週 - 月曜日

    /// <summary> 第4週 - 月曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum4_Monday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第4週 - 月曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum4_Monday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第4週 - 火曜日

    /// <summary> 第4週 - 火曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum4_Tuesday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第4週 - 火曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum4_Tuesday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第4週 - 水曜日

    /// <summary> 第4週 - 水曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum4_Wednesday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第4週 - 水曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum4_Wednesday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第4週 - 木曜日

    /// <summary> 第4週 - 木曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum4_Thursday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第4週 - 木曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum4_Thursday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第4週 - 金曜日

    /// <summary> 第4週 - 金曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum4_Friday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第4週 - 金曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum4_Friday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第4週 - 土曜日

    /// <summary> 第4週 - 土曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum4_Saturday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第4週 - 土曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum4_Saturday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第4週 - 日曜日

    /// <summary> 第4週 - 日曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum4_Sunday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第4週 - 日曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum4_Sunday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第5週 - 月曜日

    /// <summary> 第5週 - 月曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum5_Monday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第5週 - 月曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum5_Monday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第5週 - 火曜日

    /// <summary> 第5週 - 火曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum5_Tuesday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第5週 - 火曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum5_Tuesday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第5週 - 水曜日

    /// <summary> 第5週 - 水曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum5_Wednesday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第5週 - 水曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum5_Wednesday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第5週 - 木曜日

    /// <summary> 第5週 - 木曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum5_Thursday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第5週 - 木曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum5_Thursday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第5週 - 金曜日

    /// <summary> 第5週 - 金曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum5_Friday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第5週 - 金曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum5_Friday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第5週 - 土曜日

    /// <summary> 第5週 - 土曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum5_Saturday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第5週 - 土曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum5_Saturday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第5週 - 日曜日

    /// <summary> 第5週 - 日曜日 </summary>
    public ReactiveProperty<ScheduleEntity> WeekNum5_Sunday { get; set; } = new ReactiveProperty<ScheduleEntity>();

    /// <summary> 第5週 - 日曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum5_Sunday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

}
