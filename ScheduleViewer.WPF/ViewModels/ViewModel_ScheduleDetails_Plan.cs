namespace ScheduleViewer.WPF.ViewModels;

/// <summary>
/// ViewModel - スケジュール詳細 (予定一覧)
/// </summary>
public sealed class ViewModel_ScheduleDetails_Plan : ViewModelBase<Model_ScheduleDetails_Plan>
{        
    public override event PropertyChangedEventHandler PropertyChanged;

    public ViewModel_ScheduleDetails_Plan()
    {
        this.Model.ViewModel = this;

        this.Model.Initialize();

        BindEvents();
    }

    /// <summary> Model - スケジュール詳細 (本一覧) </summary>
    protected override Model_ScheduleDetails_Plan Model => Model_ScheduleDetails_Plan.GetInstance();

    protected override void BindEvents()
    {
        this.Default_MouseLeave.Subscribe(_ => this.Model.ShowDetails(CalendarEventsEntity.Empty));
        this.Time_6_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_6_1_Entity.Value));
        this.Time_6_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_6_2_Entity.Value));
        this.Time_6_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_6_3_Entity.Value));
        this.Time_6_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_6_4_Entity.Value));
        this.Time_6_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_6_5_Entity.Value));
        this.Time_6_30min_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_6_30min_1_Entity.Value));
        this.Time_6_30min_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_6_30min_2_Entity.Value));
        this.Time_6_30min_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_6_30min_3_Entity.Value));
        this.Time_6_30min_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_6_30min_4_Entity.Value));
        this.Time_6_30min_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_6_30min_5_Entity.Value));

        this.Time_7_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_7_1_Entity.Value));
        this.Time_7_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_7_2_Entity.Value));
        this.Time_7_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_7_3_Entity.Value));
        this.Time_7_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_7_4_Entity.Value));
        this.Time_7_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_7_5_Entity.Value));
        this.Time_7_30min_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_7_30min_1_Entity.Value));
        this.Time_7_30min_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_7_30min_2_Entity.Value));
        this.Time_7_30min_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_7_30min_3_Entity.Value));
        this.Time_7_30min_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_7_30min_4_Entity.Value));
        this.Time_7_30min_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_7_30min_5_Entity.Value));

        this.Time_8_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_8_1_Entity.Value));
        this.Time_8_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_8_2_Entity.Value));
        this.Time_8_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_8_3_Entity.Value));
        this.Time_8_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_8_4_Entity.Value));
        this.Time_8_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_8_5_Entity.Value));
        this.Time_8_30min_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_8_30min_1_Entity.Value));
        this.Time_8_30min_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_8_30min_2_Entity.Value));
        this.Time_8_30min_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_8_30min_3_Entity.Value));
        this.Time_8_30min_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_8_30min_4_Entity.Value));
        this.Time_8_30min_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_8_30min_5_Entity.Value));

        this.Time_9_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_9_1_Entity.Value));
        this.Time_9_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_9_2_Entity.Value));
        this.Time_9_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_9_3_Entity.Value));
        this.Time_9_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_9_4_Entity.Value));
        this.Time_9_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_9_5_Entity.Value));
        this.Time_9_30min_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_9_30min_1_Entity.Value));
        this.Time_9_30min_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_9_30min_2_Entity.Value));
        this.Time_9_30min_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_9_30min_3_Entity.Value));
        this.Time_9_30min_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_9_30min_4_Entity.Value));
        this.Time_9_30min_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_9_30min_5_Entity.Value));

        this.Time_10_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_10_1_Entity.Value));
        this.Time_10_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_10_2_Entity.Value));
        this.Time_10_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_10_3_Entity.Value));
        this.Time_10_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_10_4_Entity.Value));
        this.Time_10_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_10_5_Entity.Value));
        this.Time_10_30min_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_10_30min_1_Entity.Value));
        this.Time_10_30min_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_10_30min_2_Entity.Value));
        this.Time_10_30min_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_10_30min_3_Entity.Value));
        this.Time_10_30min_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_10_30min_4_Entity.Value));
        this.Time_10_30min_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_10_30min_5_Entity.Value));

        this.Time_11_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_11_1_Entity.Value));
        this.Time_11_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_11_2_Entity.Value));
        this.Time_11_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_11_3_Entity.Value));
        this.Time_11_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_11_4_Entity.Value));
        this.Time_11_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_11_5_Entity.Value));
        this.Time_11_30min_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_11_30min_1_Entity.Value));
        this.Time_11_30min_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_11_30min_2_Entity.Value));
        this.Time_11_30min_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_11_30min_3_Entity.Value));
        this.Time_11_30min_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_11_30min_4_Entity.Value));
        this.Time_11_30min_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_11_30min_5_Entity.Value));

        this.Time_12_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_12_1_Entity.Value));
        this.Time_12_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_12_2_Entity.Value));
        this.Time_12_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_12_3_Entity.Value));
        this.Time_12_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_12_4_Entity.Value));
        this.Time_12_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_12_5_Entity.Value));
        this.Time_12_30min_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_12_30min_1_Entity.Value));
        this.Time_12_30min_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_12_30min_2_Entity.Value));
        this.Time_12_30min_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_12_30min_3_Entity.Value));
        this.Time_12_30min_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_12_30min_4_Entity.Value));
        this.Time_12_30min_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_12_30min_5_Entity.Value));

        this.Time_13_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_13_1_Entity.Value));
        this.Time_13_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_13_2_Entity.Value));
        this.Time_13_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_13_3_Entity.Value));
        this.Time_13_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_13_4_Entity.Value));
        this.Time_13_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_13_5_Entity.Value));
        this.Time_13_30min_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_13_30min_1_Entity.Value));
        this.Time_13_30min_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_13_30min_2_Entity.Value));
        this.Time_13_30min_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_13_30min_3_Entity.Value));
        this.Time_13_30min_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_13_30min_4_Entity.Value));
        this.Time_13_30min_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_13_30min_5_Entity.Value));

        this.Time_14_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_14_1_Entity.Value));
        this.Time_14_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_14_2_Entity.Value));
        this.Time_14_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_14_3_Entity.Value));
        this.Time_14_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_14_4_Entity.Value));
        this.Time_14_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_14_5_Entity.Value));
        this.Time_14_30min_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_14_30min_1_Entity.Value));
        this.Time_14_30min_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_14_30min_2_Entity.Value));
        this.Time_14_30min_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_14_30min_3_Entity.Value));
        this.Time_14_30min_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_14_30min_4_Entity.Value));
        this.Time_14_30min_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_14_30min_5_Entity.Value));

        this.Time_15_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_15_1_Entity.Value));
        this.Time_15_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_15_2_Entity.Value));
        this.Time_15_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_15_3_Entity.Value));
        this.Time_15_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_15_4_Entity.Value));
        this.Time_15_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_15_5_Entity.Value));
        this.Time_15_30min_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_15_30min_1_Entity.Value));
        this.Time_15_30min_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_15_30min_2_Entity.Value));
        this.Time_15_30min_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_15_30min_3_Entity.Value));
        this.Time_15_30min_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_15_30min_4_Entity.Value));
        this.Time_15_30min_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_15_30min_5_Entity.Value));

        this.Time_16_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_16_1_Entity.Value));
        this.Time_16_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_16_2_Entity.Value));
        this.Time_16_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_16_3_Entity.Value));
        this.Time_16_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_16_4_Entity.Value));
        this.Time_16_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_16_5_Entity.Value));
        this.Time_16_30min_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_16_30min_1_Entity.Value));
        this.Time_16_30min_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_16_30min_2_Entity.Value));
        this.Time_16_30min_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_16_30min_3_Entity.Value));
        this.Time_16_30min_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_16_30min_4_Entity.Value));
        this.Time_16_30min_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_16_30min_5_Entity.Value));

        this.Time_17_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_17_1_Entity.Value));
        this.Time_17_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_17_2_Entity.Value));
        this.Time_17_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_17_3_Entity.Value));
        this.Time_17_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_17_4_Entity.Value));
        this.Time_17_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_17_5_Entity.Value));
        this.Time_17_30min_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_17_30min_1_Entity.Value));
        this.Time_17_30min_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_17_30min_2_Entity.Value));
        this.Time_17_30min_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_17_30min_3_Entity.Value));
        this.Time_17_30min_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_17_30min_4_Entity.Value));
        this.Time_17_30min_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_17_30min_5_Entity.Value));

        this.Time_18_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_18_1_Entity.Value));
        this.Time_18_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_18_2_Entity.Value));
        this.Time_18_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_18_3_Entity.Value));
        this.Time_18_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_18_4_Entity.Value));
        this.Time_18_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_18_5_Entity.Value));
        this.Time_18_30min_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_18_30min_1_Entity.Value));
        this.Time_18_30min_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_18_30min_2_Entity.Value));
        this.Time_18_30min_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_18_30min_3_Entity.Value));
        this.Time_18_30min_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_18_30min_4_Entity.Value));
        this.Time_18_30min_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_18_30min_5_Entity.Value));

        this.Time_19_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_19_1_Entity.Value));
        this.Time_19_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_19_2_Entity.Value));
        this.Time_19_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_19_3_Entity.Value));
        this.Time_19_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_19_4_Entity.Value));
        this.Time_19_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_19_5_Entity.Value));
        this.Time_19_30min_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_19_30min_1_Entity.Value));
        this.Time_19_30min_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_19_30min_2_Entity.Value));
        this.Time_19_30min_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_19_30min_3_Entity.Value));
        this.Time_19_30min_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_19_30min_4_Entity.Value));
        this.Time_19_30min_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_19_30min_5_Entity.Value));

        this.Time_20_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_20_1_Entity.Value));
        this.Time_20_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_20_2_Entity.Value));
        this.Time_20_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_20_3_Entity.Value));
        this.Time_20_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_20_4_Entity.Value));
        this.Time_20_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_20_5_Entity.Value));
        this.Time_20_30min_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_20_30min_1_Entity.Value));
        this.Time_20_30min_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_20_30min_2_Entity.Value));
        this.Time_20_30min_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_20_30min_3_Entity.Value));
        this.Time_20_30min_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_20_30min_4_Entity.Value));
        this.Time_20_30min_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_20_30min_5_Entity.Value));

        this.Time_21_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_21_1_Entity.Value));
        this.Time_21_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_21_2_Entity.Value));
        this.Time_21_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_21_3_Entity.Value));
        this.Time_21_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_21_4_Entity.Value));
        this.Time_21_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_21_5_Entity.Value));
        this.Time_21_30min_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_21_30min_1_Entity.Value));
        this.Time_21_30min_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_21_30min_2_Entity.Value));
        this.Time_21_30min_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_21_30min_3_Entity.Value));
        this.Time_21_30min_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_21_30min_4_Entity.Value));
        this.Time_21_30min_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_21_30min_5_Entity.Value));

        this.Time_22_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_22_1_Entity.Value));
        this.Time_22_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_22_2_Entity.Value));
        this.Time_22_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_22_3_Entity.Value));
        this.Time_22_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_22_4_Entity.Value));
        this.Time_22_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_22_5_Entity.Value));
        this.Time_22_30min_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_22_30min_1_Entity.Value));
        this.Time_22_30min_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_22_30min_2_Entity.Value));
        this.Time_22_30min_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_22_30min_3_Entity.Value));
        this.Time_22_30min_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_22_30min_4_Entity.Value));
        this.Time_22_30min_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_22_30min_5_Entity.Value));

        this.Time_23_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_23_1_Entity.Value));
        this.Time_23_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_23_2_Entity.Value));
        this.Time_23_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_23_3_Entity.Value));
        this.Time_23_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_23_4_Entity.Value));
        this.Time_23_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_23_5_Entity.Value));
        this.Time_23_30min_1_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_23_30min_1_Entity.Value));
        this.Time_23_30min_2_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_23_30min_2_Entity.Value));
        this.Time_23_30min_3_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_23_30min_3_Entity.Value));
        this.Time_23_30min_4_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_23_30min_4_Entity.Value));
        this.Time_23_30min_5_MouseMove.Subscribe(_ => this.Model.ShowDetails(this.Time_23_30min_5_Entity.Value));

        Events_SelectionChanged.Subscribe(_ => this.Model.Clear_ViewForm());
        Events_SelectionChanged.Subscribe(_ => this.Model.ListView_SelectionChanged());

        Map_MouseLeftButtonDown.Subscribe(_ => this.Model.OpenImageViewer(this.Title_Text.Value, 
                                                                          500,
                                                                          500, 
                                                                          Map_Source.Value));

        Photo_MouseDoubleClick.Subscribe(_ => this.Model.OpenImageViewer(this.Title_Text.Value, 
                                                                         this.Photo_Height.Value,
                                                                         this.Photo_Width.Value, 
                                                                         this.Photo_Source.Value));
    }

    /// <summary> イベント - Source </summary>
    public ReactiveCollection<CalendarEventsEntity> Events_ItemSource { get; set; } = new ReactiveCollection<CalendarEventsEntity>();

    /// <summary> 地図 - Source </summary>
    public ReactiveProperty<ImageSource> Map_Source { get; set; } = new ReactiveProperty<ImageSource>();

    /// <summary> 予定 - MouseLeftButtonDown </summary>
    public ReactiveCommand Map_MouseLeftButtonDown { get; private set; } = new ReactiveCommand();

    /// <summary> 写真 - Source </summary>
    public ReactiveProperty<ImageSource> Photo_Source { get; set; } = new ReactiveProperty<ImageSource>();

    /// <summary> 写真 - MouseDoubleClick </summary>
    public ReactiveCommand Photo_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    /// <summary> 写真 - Height </summary>
    public ReactiveProperty<double> Photo_Height { get; set; } = new ReactiveProperty<double>();

    /// <summary> 写真 - Width </summary>
    public ReactiveProperty<double> Photo_Width { get; set; } = new ReactiveProperty<double>();

    /// <summary> 予定 - SelectionChanged </summary>
    public ReactiveCommand Events_SelectionChanged { get; private set; } = new ReactiveCommand();

    /// <summary> 予定 - SelectedIndex </summary>
    public ReactiveProperty<int> Events_SelectedIndex { get; set; } = new ReactiveProperty<int>();

    /// <summary> 初期状態 - MouseLeave </summary>
    public ReactiveCommand Default_MouseLeave { get; private set; } = new ReactiveCommand();

    #region 6時

    /// <summary> 6時 - 1 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_6_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 6時 - 1 - MouseMove </summary>
    public ReactiveCommand Time_6_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 6時 - 2 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_6_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 6時 - 2 - MouseMove </summary>
    public ReactiveCommand Time_6_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 6時 - 3 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_6_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 6時 - 3 - MouseMove </summary>
    public ReactiveCommand Time_6_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 6時 - 4 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_6_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 6時 - 4 - MouseMove </summary>
    public ReactiveCommand Time_6_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 6時 - 5 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_6_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 6時 - 5 - MouseMove </summary>
    public ReactiveCommand Time_6_5_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 6時30分 - 1 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_6_30min_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 6時30分 - 1 - MouseMove </summary>
    public ReactiveCommand Time_6_30min_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 6時30分 - 2 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_6_30min_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 6時30分 - 2 - MouseMove </summary>
    public ReactiveCommand Time_6_30min_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 6時30分 - 3 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_6_30min_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 6時30分 - 3 - MouseMove </summary>
    public ReactiveCommand Time_6_30min_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 6時30分 - 4 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_6_30min_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 6時30分 - 4 - MouseMove </summary>
    public ReactiveCommand Time_6_30min_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 6時30分 - 5 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_6_30min_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 6時30分 - 5 - MouseMove </summary>
    public ReactiveCommand Time_6_30min_5_MouseMove { get; private set; } = new ReactiveCommand();

    #endregion

    #region 7時

    /// <summary> 7時 - 1 </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_7_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 7時 - 1 - MouseMove </summary>
    public ReactiveCommand Time_7_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 7時 - 2 </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_7_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 7時 - 2 - MouseMove </summary>
    public ReactiveCommand Time_7_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 7時 - 3 </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_7_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 7時 - 3 - MouseMove </summary>
    public ReactiveCommand Time_7_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 7時 - 4 </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_7_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 7時 - 4 - MouseMove </summary>
    public ReactiveCommand Time_7_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 7時 - 5 </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_7_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 7時 - 5 - MouseMove </summary>
    public ReactiveCommand Time_7_5_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 7時30分 - 1 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_7_30min_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 7時30分 - 1 - MouseMove </summary>
    public ReactiveCommand Time_7_30min_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 7時30分 - 2 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_7_30min_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 7時30分 - 2 - MouseMove </summary>
    public ReactiveCommand Time_7_30min_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 7時30分 - 3 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_7_30min_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 7時30分 - 3 - MouseMove </summary>
    public ReactiveCommand Time_7_30min_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 7時30分 - 4 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_7_30min_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 7時30分 - 4 - MouseMove </summary>
    public ReactiveCommand Time_7_30min_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 7時30分 - 5 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_7_30min_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 7時30分 - 5 - MouseMove </summary>
    public ReactiveCommand Time_7_30min_5_MouseMove { get; private set; } = new ReactiveCommand();

    #endregion

    #region 8時

    /// <summary> 8時 - 1 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_8_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 8時 - 1 - MouseMove </summary>
    public ReactiveCommand Time_8_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 8時 - 2 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_8_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 8時 - 2 - MouseMove </summary>
    public ReactiveCommand Time_8_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 8時 - 3 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_8_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 8時 - 3 - MouseMove </summary>
    public ReactiveCommand Time_8_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 8時 - 4 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_8_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 8時 - 4 - MouseMove </summary>
    public ReactiveCommand Time_8_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 8時 - 5 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_8_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 8時 - 5 - MouseMove </summary>
    public ReactiveCommand Time_8_5_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 8時30分 - 1 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_8_30min_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 8時30分 - 1 - MouseMove </summary>
    public ReactiveCommand Time_8_30min_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 8時30分 - 2 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_8_30min_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 8時30分 - 2 - MouseMove </summary>
    public ReactiveCommand Time_8_30min_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 8時30分 - 3 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_8_30min_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 8時30分 - 3 - MouseMove </summary>
    public ReactiveCommand Time_8_30min_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 8時30分 - 4 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_8_30min_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 8時30分 - 4 - MouseMove </summary>
    public ReactiveCommand Time_8_30min_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 8時30分 - 5 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_8_30min_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 8時30分 - 5 - MouseMove </summary>
    public ReactiveCommand Time_8_30min_5_MouseMove { get; private set; } = new ReactiveCommand();

    #endregion

    #region 9時

    /// <summary> 9時 - 1 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_9_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 9時 - 1 - MouseMove </summary>
    public ReactiveCommand Time_9_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 9時 - 2 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_9_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 9時 - 2 - MouseMove </summary>
    public ReactiveCommand Time_9_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 9時 - 3 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_9_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 9時 - 3 - MouseMove </summary>
    public ReactiveCommand Time_9_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 9時 - 4 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_9_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 9時 - 4 - MouseMove </summary>
    public ReactiveCommand Time_9_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 9時 - 5 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_9_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 9時 - 5 - MouseMove </summary>
    public ReactiveCommand Time_9_5_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 9時30分 - 1 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_9_30min_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 9時30分 - 1 - MouseMove </summary>
    public ReactiveCommand Time_9_30min_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 9時30分 - 2 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_9_30min_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 9時30分 - 2 - MouseMove </summary>
    public ReactiveCommand Time_9_30min_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 9時30分 - 3 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_9_30min_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 9時30分 - 3 - MouseMove </summary>
    public ReactiveCommand Time_9_30min_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 9時30分 - 4 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_9_30min_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 9時30分 - 4 - MouseMove </summary>
    public ReactiveCommand Time_9_30min_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 9時30分 - 5 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_9_30min_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 9時30分 - 5 - MouseMove </summary>
    public ReactiveCommand Time_9_30min_5_MouseMove { get; private set; } = new ReactiveCommand();

    #endregion

    #region 10時

    /// <summary> 10時 - 1 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_10_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 10時 - 1 - MouseMove </summary>
    public ReactiveCommand Time_10_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 10時 - 2 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_10_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 10時 - 2 - MouseMove </summary>
    public ReactiveCommand Time_10_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 10時 - 3 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_10_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 10時 - 3 - MouseMove </summary>
    public ReactiveCommand Time_10_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 10時 - 4 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_10_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 10時 - 4 - MouseMove </summary>
    public ReactiveCommand Time_10_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 10時 - 5 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_10_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 10時 - 5 - MouseMove </summary>
    public ReactiveCommand Time_10_5_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 10時30分 - 1 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_10_30min_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 10時30分 - 1 - MouseMove </summary>
    public ReactiveCommand Time_10_30min_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 10時30分 - 2 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_10_30min_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 10時30分 - 2 - MouseMove </summary>
    public ReactiveCommand Time_10_30min_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 10時30分 - 3 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_10_30min_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 10時30分 - 3 - MouseMove </summary>
    public ReactiveCommand Time_10_30min_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 10時30分 - 4 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_10_30min_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 10時30分 - 4 - MouseMove </summary>
    public ReactiveCommand Time_10_30min_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 10時30分 - 5 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_10_30min_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 10時30分 - 5 - MouseMove </summary>
    public ReactiveCommand Time_10_30min_5_MouseMove { get; private set; } = new ReactiveCommand();

    #endregion

    #region 11時

    /// <summary> 11時 - 1 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_11_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 11時 - 1 - MouseMove </summary>
    public ReactiveCommand Time_11_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 11時 - 2 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_11_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 11時 - 2 - MouseMove </summary>
    public ReactiveCommand Time_11_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 11時 - 3 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_11_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 11時 - 3 - MouseMove </summary>
    public ReactiveCommand Time_11_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 11時 - 4 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_11_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 11時 - 4 - MouseMove </summary>
    public ReactiveCommand Time_11_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 11時 - 5 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_11_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 11時 - 5 - MouseMove </summary>
    public ReactiveCommand Time_11_5_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 11時30分 - 1 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_11_30min_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 11時30分 - 1 - MouseMove </summary>
    public ReactiveCommand Time_11_30min_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 11時30分 - 2 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_11_30min_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 11時30分 - 2 - MouseMove </summary>
    public ReactiveCommand Time_11_30min_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 11時30分 - 3 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_11_30min_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 11時30分 - 3 - MouseMove </summary>
    public ReactiveCommand Time_11_30min_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 11時30分 - 4 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_11_30min_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 11時30分 - 4 - MouseMove </summary>
    public ReactiveCommand Time_11_30min_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 11時30分 - 5 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_11_30min_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 11時30分 - 5 - MouseMove </summary>
    public ReactiveCommand Time_11_30min_5_MouseMove { get; private set; } = new ReactiveCommand();

    #endregion

    #region 12時

    /// <summary> 12時 - 1 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_12_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 12時 - 1 - MouseMove </summary>
    public ReactiveCommand Time_12_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 12時 - 2 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_12_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 12時 - 2 - MouseMove </summary>
    public ReactiveCommand Time_12_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 12時 - 3 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_12_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 12時 - 3 - MouseMove </summary>
    public ReactiveCommand Time_12_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 12時 - 4 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_12_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 12時 - 4 - MouseMove </summary>
    public ReactiveCommand Time_12_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 12時 - 5 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_12_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 12時 - 5 - MouseMove </summary>
    public ReactiveCommand Time_12_5_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 12時30分 - 1 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_12_30min_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 12時30分 - 1 - MouseMove </summary>
    public ReactiveCommand Time_12_30min_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 12時30分 - 2 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_12_30min_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 12時30分 - 2 - MouseMove </summary>
    public ReactiveCommand Time_12_30min_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 12時30分 - 3 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_12_30min_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 12時30分 - 3 - MouseMove </summary>
    public ReactiveCommand Time_12_30min_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 12時30分 - 4 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_12_30min_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 12時30分 - 4 - MouseMove </summary>
    public ReactiveCommand Time_12_30min_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 12時30分 - 5 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_12_30min_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 12時30分 - 5 - MouseMove </summary>
    public ReactiveCommand Time_12_30min_5_MouseMove { get; private set; } = new ReactiveCommand();

    #endregion

    #region 13時

    /// <summary> 13時 - 1 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_13_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 13時 - 1 - MouseMove </summary>
    public ReactiveCommand Time_13_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 13時 - 2 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_13_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 13時 - 2 - MouseMove </summary>
    public ReactiveCommand Time_13_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 13時 - 3 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_13_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 13時 - 3 - MouseMove </summary>
    public ReactiveCommand Time_13_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 13時 - 4 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_13_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 13時 - 4 - MouseMove </summary>
    public ReactiveCommand Time_13_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 13時 - 5 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_13_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 13時 - 5 - MouseMove </summary>
    public ReactiveCommand Time_13_5_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 13時30分 - 1 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_13_30min_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 13時30分 - 1 - MouseMove </summary>
    public ReactiveCommand Time_13_30min_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 13時30分 - 2 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_13_30min_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 13時30分 - 2 - MouseMove </summary>
    public ReactiveCommand Time_13_30min_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 13時30分 - 3 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_13_30min_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 13時30分 - 3 - MouseMove </summary>
    public ReactiveCommand Time_13_30min_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 13時30分 - 4 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_13_30min_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 13時30分 - 4 - MouseMove </summary>
    public ReactiveCommand Time_13_30min_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 13時30分 - 5 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_13_30min_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 13時30分 - 5 - MouseMove </summary>
    public ReactiveCommand Time_13_30min_5_MouseMove { get; private set; } = new ReactiveCommand();

    #endregion

    #region 14時

    /// <summary> 14時 - 1 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_14_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 14時 - 1 - MouseMove </summary>
    public ReactiveCommand Time_14_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 14時 - 2 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_14_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 14時 - 2 - MouseMove </summary>
    public ReactiveCommand Time_14_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 14時 - 3 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_14_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 14時 - 3 - MouseMove </summary>
    public ReactiveCommand Time_14_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 14時 - 4 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_14_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 14時 - 4 - MouseMove </summary>
    public ReactiveCommand Time_14_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 14時 - 5 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_14_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 14時 - 5 - MouseMove </summary>
    public ReactiveCommand Time_14_5_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 14時30分 - 1 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_14_30min_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 14時30分 - 1 - MouseMove </summary>
    public ReactiveCommand Time_14_30min_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 14時30分 - 2 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_14_30min_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 14時30分 - 2 - MouseMove </summary>
    public ReactiveCommand Time_14_30min_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 14時30分 - 3 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_14_30min_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 14時30分 - 3 - MouseMove </summary>
    public ReactiveCommand Time_14_30min_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 14時30分 - 4 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_14_30min_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 14時30分 - 4 - MouseMove </summary>
    public ReactiveCommand Time_14_30min_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 14時30分 - 5 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_14_30min_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 14時30分 - 5 - MouseMove </summary>
    public ReactiveCommand Time_14_30min_5_MouseMove { get; private set; } = new ReactiveCommand();

    #endregion

    #region 15時

    /// <summary> 15時 - 1 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_15_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 15時 - 1 - MouseMove </summary>
    public ReactiveCommand Time_15_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 15時 - 2 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_15_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 15時 - 2 - MouseMove </summary>
    public ReactiveCommand Time_15_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 15時 - 3 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_15_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 15時 - 3 - MouseMove </summary>
    public ReactiveCommand Time_15_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 15時 - 4 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_15_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 15時 - 4 - MouseMove </summary>
    public ReactiveCommand Time_15_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 15時 - 5 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_15_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 15時 - 5 - MouseMove </summary>
    public ReactiveCommand Time_15_5_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 15時30分 - 1 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_15_30min_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 15時30分 - 1 - MouseMove </summary>
    public ReactiveCommand Time_15_30min_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 15時30分 - 2 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_15_30min_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 15時30分 - 2 - MouseMove </summary>
    public ReactiveCommand Time_15_30min_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 15時30分 - 3 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_15_30min_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 15時30分 - 3 - MouseMove </summary>
    public ReactiveCommand Time_15_30min_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 15時30分 - 4 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_15_30min_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 15時30分 - 4 - MouseMove </summary>
    public ReactiveCommand Time_15_30min_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 15時30分 - 5 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_15_30min_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 15時30分 - 5 - MouseMove </summary>
    public ReactiveCommand Time_15_30min_5_MouseMove { get; private set; } = new ReactiveCommand();

    #endregion

    #region 16時

    /// <summary> 16時 - 1 </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_16_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 16時 - 1 - MouseMove </summary>
    public ReactiveCommand Time_16_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 16時 - 2 </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_16_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 16時 - 2 - MouseMove </summary>
    public ReactiveCommand Time_16_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 16時 - 3 </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_16_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 16時 - 3 - MouseMove </summary>
    public ReactiveCommand Time_16_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 16時 - 4 </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_16_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 16時 - 4 - MouseMove </summary>
    public ReactiveCommand Time_16_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 16時 - 5 </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_16_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 16時 - 5 - MouseMove </summary>
    public ReactiveCommand Time_16_5_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 16時30分 - 1 </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_16_30min_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 16時30分 - 1 - MouseMove </summary>
    public ReactiveCommand Time_16_30min_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 16時30分 - 2 </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_16_30min_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 16時30分 - 2 - MouseMove </summary>
    public ReactiveCommand Time_16_30min_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 16時30分 - 3 </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_16_30min_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 16時30分 - 3 - MouseMove </summary>
    public ReactiveCommand Time_16_30min_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 16時30分 - 4 </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_16_30min_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 16時30分 - 4 - MouseMove </summary>
    public ReactiveCommand Time_16_30min_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 16時30分 - 5 </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_16_30min_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 16時30分 - 5 - MouseMove </summary>
    public ReactiveCommand Time_16_30min_5_MouseMove { get; private set; } = new ReactiveCommand();

    #endregion

    #region 17時

    /// <summary> 17時 - 1 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_17_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 17時 - 1 - MouseMove </summary>
    public ReactiveCommand Time_17_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 17時 - 2 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_17_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 17時 - 2 - MouseMove </summary>
    public ReactiveCommand Time_17_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 17時 - 3 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_17_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 17時 - 3 - MouseMove </summary>
    public ReactiveCommand Time_17_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 17時 - 4 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_17_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 17時 - 4 - MouseMove </summary>
    public ReactiveCommand Time_17_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 17時 - 5 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_17_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 17時 - 5 - MouseMove </summary>
    public ReactiveCommand Time_17_5_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 17時30分 - 1 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_17_30min_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 17時30分 - 1 - MouseMove </summary>
    public ReactiveCommand Time_17_30min_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 17時30分 - 2 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_17_30min_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 17時30分 - 2 - MouseMove </summary>
    public ReactiveCommand Time_17_30min_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 17時30分 - 3 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_17_30min_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 17時30分 - 3 - MouseMove </summary>
    public ReactiveCommand Time_17_30min_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 17時30分 - 4 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_17_30min_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 17時30分 - 4 - MouseMove </summary>
    public ReactiveCommand Time_17_30min_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 17時30分 - 5 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_17_30min_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 17時30分 - 5 - MouseMove </summary>
    public ReactiveCommand Time_17_30min_5_MouseMove { get; private set; } = new ReactiveCommand();

    #endregion

    #region 18時

    /// <summary> 18時 - 1 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_18_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 18時 - 1 - MouseMove </summary>
    public ReactiveCommand Time_18_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 18時 - 2 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_18_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 18時 - 2 - MouseMove </summary>
    public ReactiveCommand Time_18_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 18時 - 3 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_18_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 18時 - 3 - MouseMove </summary>
    public ReactiveCommand Time_18_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 18時 - 4 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_18_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 18時 - 4 - MouseMove </summary>
    public ReactiveCommand Time_18_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 18時 - 5 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_18_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 18時 - 5 - MouseMove </summary>
    public ReactiveCommand Time_18_5_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 18時30分 - 1 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_18_30min_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 18時30分 - 1 - MouseMove </summary>
    public ReactiveCommand Time_18_30min_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 18時30分 - 2 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_18_30min_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 18時30分 - 2 - MouseMove </summary>
    public ReactiveCommand Time_18_30min_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 18時30分 - 3 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_18_30min_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 18時30分 - 3 - MouseMove </summary>
    public ReactiveCommand Time_18_30min_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 18時30分 - 4 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_18_30min_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 18時30分 - 4 - MouseMove </summary>
    public ReactiveCommand Time_18_30min_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 18時30分 - 5 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_18_30min_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 18時30分 - 5 - MouseMove </summary>
    public ReactiveCommand Time_18_30min_5_MouseMove { get; private set; } = new ReactiveCommand();

    #endregion

    #region 19時

    /// <summary> 19時 - 1 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_19_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 19時 - 1 - MouseMove </summary>
    public ReactiveCommand Time_19_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 19時 - 2 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_19_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 19時 - 2 - MouseMove </summary>
    public ReactiveCommand Time_19_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 19時 - 3 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_19_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 19時 - 3 - MouseMove </summary>
    public ReactiveCommand Time_19_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 19時 - 4 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_19_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 19時 - 4 - MouseMove </summary>
    public ReactiveCommand Time_19_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 19時 - 5 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_19_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 19時 - 5 - MouseMove </summary>
    public ReactiveCommand Time_19_5_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 19時30分 - 1 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_19_30min_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 19時30分 - 1 - MouseMove </summary>
    public ReactiveCommand Time_19_30min_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 19時30分 - 2 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_19_30min_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 19時30分 - 2 - MouseMove </summary>
    public ReactiveCommand Time_19_30min_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 19時30分 - 3 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_19_30min_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 19時30分 - 3 - MouseMove </summary>
    public ReactiveCommand Time_19_30min_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 19時30分 - 4 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_19_30min_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 19時30分 - 4 - MouseMove </summary>
    public ReactiveCommand Time_19_30min_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 19時30分 - 5 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_19_30min_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 19時30分 - 5 - MouseMove </summary>
    public ReactiveCommand Time_19_30min_5_MouseMove { get; private set; } = new ReactiveCommand();

    #endregion

    #region 20時

    /// <summary> 20時 - 1 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_20_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 20時 - 1 - MouseMove </summary>
    public ReactiveCommand Time_20_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 20時 - 2 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_20_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 20時 - 2 - MouseMove </summary>
    public ReactiveCommand Time_20_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 20時 - 3 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_20_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 20時 - 3 - MouseMove </summary>
    public ReactiveCommand Time_20_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 20時 - 4 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_20_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 20時 - 4 - MouseMove </summary>
    public ReactiveCommand Time_20_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 20時 - 5 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_20_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 20時 - 5 - MouseMove </summary>
    public ReactiveCommand Time_20_5_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 20時30分 - 1 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_20_30min_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 20時30分 - 1 - MouseMove </summary>
    public ReactiveCommand Time_20_30min_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 20時30分 - 2 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_20_30min_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 20時30分 - 2 - MouseMove </summary>
    public ReactiveCommand Time_20_30min_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 20時30分 - 3 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_20_30min_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 20時30分 - 3 - MouseMove </summary>
    public ReactiveCommand Time_20_30min_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 20時30分 - 4 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_20_30min_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 20時30分 - 4 - MouseMove </summary>
    public ReactiveCommand Time_20_30min_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 20時30分 - 5 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_20_30min_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 20時30分 - 5 - MouseMove </summary>
    public ReactiveCommand Time_20_30min_5_MouseMove { get; private set; } = new ReactiveCommand();

    #endregion

    #region 21時

    /// <summary> 21時 - 1 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_21_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 21時 - 1 - MouseMove </summary>
    public ReactiveCommand Time_21_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 21時 - 2 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_21_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 21時 - 2 - MouseMove </summary>
    public ReactiveCommand Time_21_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 21時 - 3 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_21_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 21時 - 3 - MouseMove </summary>
    public ReactiveCommand Time_21_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 21時 - 4 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_21_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 21時 - 4 - MouseMove </summary>
    public ReactiveCommand Time_21_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 21時 - 5 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_21_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 21時 - 5 - MouseMove </summary>
    public ReactiveCommand Time_21_5_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 21時30分 - 1 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_21_30min_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 21時30分 - 1 - MouseMove </summary>
    public ReactiveCommand Time_21_30min_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 21時30分 - 2 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_21_30min_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 21時30分 - 2 - MouseMove </summary>
    public ReactiveCommand Time_21_30min_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 21時30分 - 3 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_21_30min_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 21時30分 - 3 - MouseMove </summary>
    public ReactiveCommand Time_21_30min_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 21時30分 - 4 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_21_30min_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 21時30分 - 4 - MouseMove </summary>
    public ReactiveCommand Time_21_30min_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 21時30分 - 5 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_21_30min_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 21時30分 - 5 - MouseMove </summary>
    public ReactiveCommand Time_21_30min_5_MouseMove { get; private set; } = new ReactiveCommand();

    #endregion

    #region 22時

    /// <summary> 22時 - 1 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_22_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 22時 - 1 - MouseMove </summary>
    public ReactiveCommand Time_22_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 22時 - 2 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_22_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 22時 - 1 - MouseMove </summary>
    public ReactiveCommand Time_22_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 22時 - 3 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_22_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 22時 - 3 - MouseMove </summary>
    public ReactiveCommand Time_22_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 22時 - 4 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_22_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 22時 - 4 - MouseMove </summary>
    public ReactiveCommand Time_22_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 22時 - 5 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_22_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 22時 - 5 - MouseMove </summary>
    public ReactiveCommand Time_22_5_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 22時30分 - 1 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_22_30min_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 22時30分 - 1 - MouseMove </summary>
    public ReactiveCommand Time_22_30min_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 22時30分 - 2 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_22_30min_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 22時30分 - 2 - MouseMove </summary>
    public ReactiveCommand Time_22_30min_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 22時30分 - 3 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_22_30min_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 22時30分 - 3 - MouseMove </summary>
    public ReactiveCommand Time_22_30min_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 22時30分 - 4 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_22_30min_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 22時30分 - 4 - MouseMove </summary>
    public ReactiveCommand Time_22_30min_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 22時30分 - 5 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_22_30min_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 22時30分 - 5 - MouseMove </summary>
    public ReactiveCommand Time_22_30min_5_MouseMove { get; private set; } = new ReactiveCommand();

    #endregion

    #region 23時

    /// <summary> 23時 - 1 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_23_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 23時 - 1 - MouseMove </summary>
    public ReactiveCommand Time_23_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 23時 - 2 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_23_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 23時 - 2 - MouseMove </summary>
    public ReactiveCommand Time_23_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 23時 - 3 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_23_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 23時 - 3 - MouseMove </summary>
    public ReactiveCommand Time_23_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 23時 - 4 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_23_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 23時 - 4 - MouseMove </summary>
    public ReactiveCommand Time_23_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 23時 - 5 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_23_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 23時 - 5 - MouseMove </summary>
    public ReactiveCommand Time_23_5_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 23時30分 - 1 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_23_30min_1_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 23時30分 - 1 - MouseMove </summary>
    public ReactiveCommand Time_23_30min_1_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 23時30分 - 2 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_23_30min_2_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 23時30分 - 2 - MouseMove </summary>
    public ReactiveCommand Time_23_30min_2_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 23時30分 - 3 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_23_30min_3_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 23時30分 - 3 - MouseMove </summary>
    public ReactiveCommand Time_23_30min_3_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 23時30分 - 4 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_23_30min_4_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 23時30分 - 4 - MouseMove </summary>
    public ReactiveCommand Time_23_30min_4_MouseMove { get; private set; } = new ReactiveCommand();

    /// <summary> 23時30分 - 5 - Entity </summary>
    public ReactiveProperty<CalendarEventsEntity> Time_23_30min_5_Entity { get; set; } = new ReactiveProperty<CalendarEventsEntity>();

    /// <summary> 23時30分 - 5 - MouseMove </summary>
    public ReactiveCommand Time_23_30min_5_MouseMove { get; private set; } = new ReactiveCommand();

    #endregion

    #region スケジュール詳細

    /// <summary> タイトル - Text </summary>
    public ReactiveProperty<string> Title_Text { get; set; } = new ReactiveProperty<string>();

    /// <summary> 開始時刻 - Text </summary>
    public ReactiveProperty<string> StartTime_Text { get; set; } = new ReactiveProperty<string>();

    /// <summary> 終了時刻 - Text </summary>
    public ReactiveProperty<string> EndTime_Text { get; set; } = new ReactiveProperty<string>();

    /// <summary> 場所 - Text </summary>
    public ReactiveProperty<string> Place_Text { get; set; } = new ReactiveProperty<string>();
    
    /// <summary> 詳細 - Text </summary>
    public ReactiveProperty<string> Description_Text { get; set; } = new ReactiveProperty<string>();
    
    #endregion

}
