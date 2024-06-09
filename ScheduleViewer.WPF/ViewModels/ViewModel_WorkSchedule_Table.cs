namespace ScheduleViewer.WPF.ViewModels;

/// <summary>
/// ViewModel - 勤務表
/// </summary>
public class ViewModel_WorkSchedule_Table : ViewModelBase<Model_WorkSchedule>
{
    public override event PropertyChangedEventHandler PropertyChanged;

    public ViewModel_WorkSchedule_Table()
    {
        this.Model.ViewModel_Table = this;
        
        this.Model.Initialize_Table();

        this.BindEvents();
    }

    protected override void BindEvents()
    {
        this.Day1_Schedule_Details_Command.Subscribe(_  => this.Model.OpenDetialsWindow(1));
        this.Day2_Schedule_Details_Command.Subscribe(_  => this.Model.OpenDetialsWindow(2));
        this.Day3_Schedule_Details_Command.Subscribe(_  => this.Model.OpenDetialsWindow(3));
        this.Day4_Schedule_Details_Command.Subscribe(_  => this.Model.OpenDetialsWindow(4));
        this.Day5_Schedule_Details_Command.Subscribe(_  => this.Model.OpenDetialsWindow(5));
        this.Day6_Schedule_Details_Command.Subscribe(_  => this.Model.OpenDetialsWindow(6));
        this.Day7_Schedule_Details_Command.Subscribe(_  => this.Model.OpenDetialsWindow(7));
        this.Day8_Schedule_Details_Command.Subscribe(_  => this.Model.OpenDetialsWindow(8));
        this.Day9_Schedule_Details_Command.Subscribe(_  => this.Model.OpenDetialsWindow(9));
        this.Day10_Schedule_Details_Command.Subscribe(_ => this.Model.OpenDetialsWindow(10));
        this.Day11_Schedule_Details_Command.Subscribe(_ => this.Model.OpenDetialsWindow(11));
        this.Day12_Schedule_Details_Command.Subscribe(_ => this.Model.OpenDetialsWindow(12));
        this.Day13_Schedule_Details_Command.Subscribe(_ => this.Model.OpenDetialsWindow(13));
        this.Day14_Schedule_Details_Command.Subscribe(_ => this.Model.OpenDetialsWindow(14));
        this.Day15_Schedule_Details_Command.Subscribe(_ => this.Model.OpenDetialsWindow(15));
        this.Day16_Schedule_Details_Command.Subscribe(_ => this.Model.OpenDetialsWindow(16));
        this.Day17_Schedule_Details_Command.Subscribe(_ => this.Model.OpenDetialsWindow(17));
        this.Day18_Schedule_Details_Command.Subscribe(_ => this.Model.OpenDetialsWindow(18));
        this.Day19_Schedule_Details_Command.Subscribe(_ => this.Model.OpenDetialsWindow(19));
        this.Day20_Schedule_Details_Command.Subscribe(_ => this.Model.OpenDetialsWindow(20));
        this.Day21_Schedule_Details_Command.Subscribe(_ => this.Model.OpenDetialsWindow(21));
        this.Day22_Schedule_Details_Command.Subscribe(_ => this.Model.OpenDetialsWindow(22));
        this.Day23_Schedule_Details_Command.Subscribe(_ => this.Model.OpenDetialsWindow(23));
        this.Day24_Schedule_Details_Command.Subscribe(_ => this.Model.OpenDetialsWindow(24));
        this.Day25_Schedule_Details_Command.Subscribe(_ => this.Model.OpenDetialsWindow(25));
        this.Day26_Schedule_Details_Command.Subscribe(_ => this.Model.OpenDetialsWindow(26));
        this.Day27_Schedule_Details_Command.Subscribe(_ => this.Model.OpenDetialsWindow(27));
        this.Day28_Schedule_Details_Command.Subscribe(_ => this.Model.OpenDetialsWindow(28));
        this.Day29_Schedule_Details_Command.Subscribe(_ => this.Model.OpenDetialsWindow(29));
        this.Day30_Schedule_Details_Command.Subscribe(_ => this.Model.OpenDetialsWindow(30));
        this.Day31_Schedule_Details_Command.Subscribe(_ => this.Model.OpenDetialsWindow(31));
    }

    /// <summary>
    /// Model - 勤務表
    /// </summary>
    protected override Model_WorkSchedule Model => Model_WorkSchedule.GetInstance();

    #region 1日ごとの予定

    /// <summary> 1日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleEntity> Day1_Schedule { get; set; } = new ReactiveProperty<WorkScheduleEntity>();

    /// <summary> 2日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleEntity> Day2_Schedule { get; set; } = new ReactiveProperty<WorkScheduleEntity>();

    /// <summary> 3日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleEntity> Day3_Schedule { get; set; } = new ReactiveProperty<WorkScheduleEntity>();

    /// <summary> 4日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleEntity> Day4_Schedule { get; set; } = new ReactiveProperty<WorkScheduleEntity>();

    /// <summary> 5日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleEntity> Day5_Schedule { get; set; } = new ReactiveProperty<WorkScheduleEntity>();

    /// <summary> 6日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleEntity> Day6_Schedule { get; set; } = new ReactiveProperty<WorkScheduleEntity>();

    /// <summary> 7日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleEntity> Day7_Schedule { get; set; } = new ReactiveProperty<WorkScheduleEntity>();

    /// <summary> 8日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleEntity> Day8_Schedule { get; set; } = new ReactiveProperty<WorkScheduleEntity>();

    /// <summary> 9日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleEntity> Day9_Schedule { get; set; } = new ReactiveProperty<WorkScheduleEntity>();

    /// <summary> 10日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleEntity> Day10_Schedule { get; set; } = new ReactiveProperty<WorkScheduleEntity>();

    /// <summary> 11日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleEntity> Day11_Schedule { get; set; } = new ReactiveProperty<WorkScheduleEntity>();

    /// <summary> 12日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleEntity> Day12_Schedule { get; set; } = new ReactiveProperty<WorkScheduleEntity>();

    /// <summary> 13日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleEntity> Day13_Schedule { get; set; } = new ReactiveProperty<WorkScheduleEntity>();

    /// <summary> 14日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleEntity> Day14_Schedule { get; set; } = new ReactiveProperty<WorkScheduleEntity>();

    /// <summary> 15日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleEntity> Day15_Schedule { get; set; } = new ReactiveProperty<WorkScheduleEntity>();

    /// <summary> 16日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleEntity> Day16_Schedule { get; set; } = new ReactiveProperty<WorkScheduleEntity>();

    /// <summary> 17日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleEntity> Day17_Schedule { get; set; } = new ReactiveProperty<WorkScheduleEntity>();

    /// <summary> 18日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleEntity> Day18_Schedule { get; set; } = new ReactiveProperty<WorkScheduleEntity>();

    /// <summary> 19日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleEntity> Day19_Schedule { get; set; } = new ReactiveProperty<WorkScheduleEntity>();

    /// <summary> 20日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleEntity> Day20_Schedule { get; set; } = new ReactiveProperty<WorkScheduleEntity>();

    /// <summary> 21日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleEntity> Day21_Schedule { get; set; } = new ReactiveProperty<WorkScheduleEntity>();

    /// <summary> 22日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleEntity> Day22_Schedule { get; set; }= new ReactiveProperty<WorkScheduleEntity>();

    /// <summary> 23日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleEntity> Day23_Schedule { get; set; } = new ReactiveProperty<WorkScheduleEntity>();

    /// <summary> 24日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleEntity> Day24_Schedule { get; set; } = new ReactiveProperty<WorkScheduleEntity>();

    /// <summary> 25日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleEntity> Day25_Schedule { get; set; } = new ReactiveProperty<WorkScheduleEntity>();

    /// <summary> 26日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleEntity> Day26_Schedule { get; set; } = new ReactiveProperty<WorkScheduleEntity>();

    /// <summary> 27日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleEntity> Day27_Schedule { get; set; } = new ReactiveProperty<WorkScheduleEntity>();

    /// <summary> 28日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleEntity> Day28_Schedule { get; set; } = new ReactiveProperty<WorkScheduleEntity>();

    /// <summary> 29日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleEntity> Day29_Schedule { get; set; } = new ReactiveProperty<WorkScheduleEntity>();

    /// <summary> 30日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleEntity> Day30_Schedule { get; set; } = new ReactiveProperty<WorkScheduleEntity>();

    /// <summary> 31日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleEntity> Day31_Schedule { get; set; } = new ReactiveProperty<WorkScheduleEntity>();

    #endregion

    #region 詳細ボタン

    /// <summary> 1日 - 詳細ボタン - Command </summary>
    public ReactiveCommand Day1_Schedule_Details_Command { get; private set; } = new ReactiveCommand();

    /// <summary> 2日 - 詳細ボタン - Command </summary>
    public ReactiveCommand Day2_Schedule_Details_Command { get; private set; } = new ReactiveCommand();

    /// <summary> 3日 - 詳細ボタン - Command </summary>
    public ReactiveCommand Day3_Schedule_Details_Command { get; private set; } = new ReactiveCommand();

    /// <summary> 4日 - 詳細ボタン - Command </summary>
    public ReactiveCommand Day4_Schedule_Details_Command { get; private set; } = new ReactiveCommand();

    /// <summary> 5日 - 詳細ボタン - Command </summary>
    public ReactiveCommand Day5_Schedule_Details_Command { get; private set; } = new ReactiveCommand();

    /// <summary> 6日 - 詳細ボタン - Command </summary>
    public ReactiveCommand Day6_Schedule_Details_Command { get; private set; } = new ReactiveCommand();

    /// <summary> 7日 - 詳細ボタン - Command </summary>
    public ReactiveCommand Day7_Schedule_Details_Command { get; private set; } = new ReactiveCommand();

    /// <summary> 8日 - 詳細ボタン - Command </summary>
    public ReactiveCommand Day8_Schedule_Details_Command { get; private set; } = new ReactiveCommand();

    /// <summary> 9日 - 詳細ボタン - Command </summary>
    public ReactiveCommand Day9_Schedule_Details_Command { get; private set; } = new ReactiveCommand();

    /// <summary> 10日 - 詳細ボタン - Command </summary>
    public ReactiveCommand Day10_Schedule_Details_Command { get; private set; } = new ReactiveCommand();

    /// <summary> 11日 - 詳細ボタン - Command </summary>
    public ReactiveCommand Day11_Schedule_Details_Command { get; private set; } = new ReactiveCommand();

    /// <summary> 12日 - 詳細ボタン - Command </summary>
    public ReactiveCommand Day12_Schedule_Details_Command { get; private set; } = new ReactiveCommand();

    /// <summary> 13日 - 詳細ボタン - Command </summary>
    public ReactiveCommand Day13_Schedule_Details_Command { get; private set; } = new ReactiveCommand();

    /// <summary> 14日 - 詳細ボタン - Command </summary>
    public ReactiveCommand Day14_Schedule_Details_Command { get; private set; } = new ReactiveCommand();

    /// <summary> 15日 - 詳細ボタン - Command </summary>
    public ReactiveCommand Day15_Schedule_Details_Command { get; private set; } = new ReactiveCommand();

    /// <summary> 16日 - 詳細ボタン - Command </summary>
    public ReactiveCommand Day16_Schedule_Details_Command { get; private set; } = new ReactiveCommand();

    /// <summary> 17日 - 詳細ボタン - Command </summary>
    public ReactiveCommand Day17_Schedule_Details_Command { get; private set; } = new ReactiveCommand();

    /// <summary> 18日 - 詳細ボタン - Command </summary>
    public ReactiveCommand Day18_Schedule_Details_Command { get; private set; } = new ReactiveCommand();

    /// <summary> 19日 - 詳細ボタン - Command </summary>
    public ReactiveCommand Day19_Schedule_Details_Command { get; private set; } = new ReactiveCommand();

    /// <summary> 20日 - 詳細ボタン - Command </summary>
    public ReactiveCommand Day20_Schedule_Details_Command { get; private set; } = new ReactiveCommand();

    /// <summary> 21日 - 詳細ボタン - Command </summary>
    public ReactiveCommand Day21_Schedule_Details_Command { get; private set; } = new ReactiveCommand();

    /// <summary> 22日 - 詳細ボタン - Command </summary>
    public ReactiveCommand Day22_Schedule_Details_Command { get; private set; } = new ReactiveCommand();

    /// <summary> 23日 - 詳細ボタン - Command </summary>
    public ReactiveCommand Day23_Schedule_Details_Command { get; private set; } = new ReactiveCommand();

    /// <summary> 24日 - 詳細ボタン - Command </summary>
    public ReactiveCommand Day24_Schedule_Details_Command { get; private set; } = new ReactiveCommand();

    /// <summary> 25日 - 詳細ボタン - Command </summary>
    public ReactiveCommand Day25_Schedule_Details_Command { get; private set; } = new ReactiveCommand();

    /// <summary> 26日 - 詳細ボタン - Command </summary>
    public ReactiveCommand Day26_Schedule_Details_Command { get; private set; } = new ReactiveCommand();

    /// <summary> 27日 - 詳細ボタン - Command </summary>
    public ReactiveCommand Day27_Schedule_Details_Command { get; private set; } = new ReactiveCommand();

    /// <summary> 28日 - 詳細ボタン - Command </summary>
    public ReactiveCommand Day28_Schedule_Details_Command { get; private set; } = new ReactiveCommand();

    /// <summary> 29日 - 詳細ボタン - Command </summary>
    public ReactiveCommand Day29_Schedule_Details_Command { get; private set; } = new ReactiveCommand();

    /// <summary> 30日 - 詳細ボタン - Command </summary>
    public ReactiveCommand Day30_Schedule_Details_Command { get; private set; } = new ReactiveCommand();

    /// <summary> 31日 - 詳細ボタン - Command </summary>
    public ReactiveCommand Day31_Schedule_Details_Command { get; private set; } = new ReactiveCommand();

    #endregion

}