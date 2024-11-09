
namespace ScheduleViewer.WPF.ViewModels;

public sealed class ViewModel_Schedule_Table : ViewModelBase<Model_Schedule>
{
    protected override Model_Schedule Model => Model_Schedule.GetInstance();

    public override event PropertyChangedEventHandler PropertyChanged;

    public CursorWaiting CursorWaiting { get; set; }

    public ViewModel_Schedule_Table()
    {
        this.Model.ViewModel_Table = this;

        using (this.CursorWaiting = new CursorWaiting())
        {
            this.Model.Initialize_TableAsync();

            this.BindEvents();
        }
    }

    protected override void BindEvents()
    {
        // 第1週
        this.WeekNum1_Monday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum1_Monday_Content.Value, DayOfWeek.Monday));
        this.WeekNum1_Tuesday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum1_Tuesday_Content.Value, DayOfWeek.Tuesday));
        this.WeekNum1_Wednesday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum1_Wednesday_Content.Value, DayOfWeek.Wednesday));
        this.WeekNum1_Thursday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum1_Thursday_Content.Value, DayOfWeek.Thursday));
        this.WeekNum1_Friday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum1_Friday_Content.Value, DayOfWeek.Friday));
        this.WeekNum1_Saturday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum1_Saturday_Content.Value, DayOfWeek.Saturday));
        this.WeekNum1_Sunday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum1_Sunday_Content.Value, DayOfWeek.Sunday));
        
        // 第2週
        this.WeekNum2_Monday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum2_Monday_Content.Value, DayOfWeek.Monday));
        this.WeekNum2_Tuesday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum2_Tuesday_Content.Value, DayOfWeek.Tuesday));
        this.WeekNum2_Wednesday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum2_Wednesday_Content.Value, DayOfWeek.Wednesday));
        this.WeekNum2_Thursday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum2_Thursday_Content.Value, DayOfWeek.Thursday));
        this.WeekNum2_Friday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum2_Friday_Content.Value, DayOfWeek.Friday));
        this.WeekNum2_Saturday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum2_Saturday_Content.Value, DayOfWeek.Saturday));
        this.WeekNum2_Sunday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum2_Sunday_Content.Value, DayOfWeek.Sunday));

        // 第3週
        this.WeekNum3_Monday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum3_Monday_Content.Value, DayOfWeek.Monday));
        this.WeekNum3_Tuesday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum3_Tuesday_Content.Value, DayOfWeek.Tuesday));
        this.WeekNum3_Wednesday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum3_Wednesday_Content.Value, DayOfWeek.Wednesday));
        this.WeekNum3_Thursday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum3_Thursday_Content.Value, DayOfWeek.Thursday));
        this.WeekNum3_Friday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum3_Friday_Content.Value, DayOfWeek.Friday));
        this.WeekNum3_Saturday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum3_Saturday_Content.Value, DayOfWeek.Saturday));
        this.WeekNum3_Sunday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum3_Sunday_Content.Value, DayOfWeek.Sunday));

        // 第4週
        this.WeekNum4_Monday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum4_Monday_Content.Value, DayOfWeek.Monday));
        this.WeekNum4_Tuesday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum4_Tuesday_Content.Value, DayOfWeek.Tuesday));
        this.WeekNum4_Wednesday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum4_Wednesday_Content.Value, DayOfWeek.Wednesday));
        this.WeekNum4_Thursday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum4_Thursday_Content.Value, DayOfWeek.Thursday));
        this.WeekNum4_Friday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum4_Friday_Content.Value, DayOfWeek.Friday));
        this.WeekNum4_Saturday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum4_Saturday_Content.Value, DayOfWeek.Saturday));
        this.WeekNum4_Sunday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum4_Sunday_Content.Value, DayOfWeek.Sunday));

        // 第5週
        this.WeekNum5_Monday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum5_Monday_Content.Value, DayOfWeek.Monday));
        this.WeekNum5_Tuesday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum5_Tuesday_Content.Value, DayOfWeek.Tuesday));
        this.WeekNum5_Wednesday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum5_Wednesday_Content.Value, DayOfWeek.Wednesday));
        this.WeekNum5_Thursday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum5_Thursday_Content.Value, DayOfWeek.Thursday));
        this.WeekNum5_Friday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum5_Friday_Content.Value, DayOfWeek.Friday));
        this.WeekNum5_Saturday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum5_Saturday_Content.Value, DayOfWeek.Saturday));
        this.WeekNum5_Sunday_MouseDoubleClick.Subscribe(_ => Model.ShowDetailWindow(this.WeekNum5_Sunday_Content.Value, DayOfWeek.Sunday));
    }

    #region 第1週 - 月曜日

    /// <summary> 第1週 - 月曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum1_Monday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第1週 - 月曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum1_Monday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第1週 - 火曜日

    /// <summary> 第1週 - 火曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum1_Tuesday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第1週 - 火曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum1_Tuesday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第1週 - 水曜日

    /// <summary> 第1週 - 水曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum1_Wednesday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第1週 - 水曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum1_Wednesday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第1週 - 木曜日

    /// <summary> 第1週 - 木曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum1_Thursday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第1週 - 木曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum1_Thursday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第1週 - 金曜日

    /// <summary> 第1週 - 金曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum1_Friday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第1週 - 金曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum1_Friday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第1週 - 土曜日

    /// <summary> 第1週 - 土曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum1_Saturday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第1週 - 土曜日 - MouseDoubleClick </summary>
    public ReactiveProperty<string> WeekNum1_Saturday_MouseDoubleClick { get; set; } = new ReactiveProperty<string>();

    #endregion

    #region 第1週 - 日曜日

    /// <summary> 第1週 - 日曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum1_Sunday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第1週 - 日曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum1_Sunday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第2週 - 月曜日

    /// <summary> 第2週 - 月曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum2_Monday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第2週 - 月曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum2_Monday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第2週 - 火曜日

    /// <summary> 第2週 - 火曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum2_Tuesday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第2週 - 火曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum2_Tuesday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第2週 - 水曜日

    /// <summary> 第2週 - 水曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum2_Wednesday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第2週 - 水曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum2_Wednesday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第2週 - 木曜日

    /// <summary> 第2週 - 木曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum2_Thursday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第2週 - 木曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum2_Thursday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第2週 - 金曜日

    /// <summary> 第2週 - 金曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum2_Friday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第2週 - 金曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum2_Friday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第2週 - 土曜日

    /// <summary> 第2週 - 土曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum2_Saturday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第2週 - 土曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum2_Saturday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第2週 - 日曜日

    /// <summary> 第2週 - 日曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum2_Sunday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第2週 - 日曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum2_Sunday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第3週 - 月曜日

    /// <summary> 第3週 - 月曜日 - Content  </summary>
    public ReactiveProperty<string> WeekNum3_Monday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第3週 - 日曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum3_Monday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第3週 - 火曜日

    /// <summary> 第3週 - 火曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum3_Tuesday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第3週 - 日曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum3_Tuesday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第3週 - 水曜日

    /// <summary> 第3週 - 水曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum3_Wednesday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第3週 - 水曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum3_Wednesday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第3週 - 木曜日

    /// <summary> 第3週 - 木曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum3_Thursday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第3週 - 木曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum3_Thursday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第3週 - 金曜日

    /// <summary> 第3週 - 金曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum3_Friday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第3週 - 金曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum3_Friday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第3週 - 土曜日

    /// <summary> 第3週 - 土曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum3_Saturday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第3週 - 土曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum3_Saturday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第3週 - 日曜日

    /// <summary> 第3週 - 日曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum3_Sunday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第3週 - 日曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum3_Sunday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第4週 - 月曜日

    /// <summary> 第4週 - 月曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum4_Monday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第4週 - 月曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum4_Monday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第4週 - 火曜日

    /// <summary> 第4週 - 火曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum4_Tuesday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第4週 - 火曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum4_Tuesday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第4週 - 水曜日

    /// <summary> 第4週 - 水曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum4_Wednesday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第4週 - 水曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum4_Wednesday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第4週 - 木曜日

    /// <summary> 第4週 - 木曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum4_Thursday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第4週 - 木曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum4_Thursday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第4週 - 金曜日

    /// <summary> 第4週 - 金曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum4_Friday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第4週 - 金曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum4_Friday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第4週 - 土曜日

    /// <summary> 第4週 - 土曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum4_Saturday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第4週 - 土曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum4_Saturday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第4週 - 日曜日

    /// <summary> 第4週 - 日曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum4_Sunday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第4週 - 日曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum4_Sunday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第5週 - 月曜日

    /// <summary> 第5週 - 月曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum5_Monday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第5週 - 月曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum5_Monday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第5週 - 火曜日

    /// <summary> 第5週 - 火曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum5_Tuesday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第5週 - 火曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum5_Tuesday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第5週 - 水曜日

    /// <summary> 第5週 - 水曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum5_Wednesday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第5週 - 水曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum5_Wednesday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第5週 - 木曜日

    /// <summary> 第5週 - 木曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum5_Thursday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第5週 - 木曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum5_Thursday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第5週 - 金曜日

    /// <summary> 第5週 - 金曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum5_Friday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第5週 - 金曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum5_Friday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第5週 - 土曜日

    /// <summary> 第5週 - 土曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum5_Saturday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第5週 - 土曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum5_Saturday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

    #region 第5週 - 日曜日

    /// <summary> 第5週 - 日曜日 - Content </summary>
    public ReactiveProperty<string> WeekNum5_Sunday_Content { get; set; } = new ReactiveProperty<string>();

    /// <summary> 第5週 - 日曜日 - MouseDoubleClick </summary>
    public ReactiveCommand WeekNum5_Sunday_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    #endregion

}
