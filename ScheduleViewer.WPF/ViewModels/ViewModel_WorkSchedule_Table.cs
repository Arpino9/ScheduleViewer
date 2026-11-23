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

        this.Model.Initialize_TableAsync();

        this.BindEvents();
    }

    protected override void BindEvents()
    {
        this.Update_Command.Subscribe(_ => this.Model.Update());
    }

    /// <summary> Model - 勤務表 </summary>
    protected override Model_WorkSchedule Model => Model_WorkSchedule.GetInstance();

    #region 1日ごとの予定

    /// <summary> 1日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleItem> Day1_Schedule { get; set; } = new ReactiveProperty<WorkScheduleItem>();

    /// <summary> 2日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleItem> Day2_Schedule { get; set; } = new ReactiveProperty<WorkScheduleItem>();

    /// <summary> 3日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleItem> Day3_Schedule { get; set; } = new ReactiveProperty<WorkScheduleItem>();

    /// <summary> 4日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleItem> Day4_Schedule { get; set; } = new ReactiveProperty<WorkScheduleItem>();

    /// <summary> 5日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleItem> Day5_Schedule { get; set; } = new ReactiveProperty<WorkScheduleItem>();

    /// <summary> 6日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleItem> Day6_Schedule { get; set; } = new ReactiveProperty<WorkScheduleItem>();

    /// <summary> 7日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleItem> Day7_Schedule { get; set; } = new ReactiveProperty<WorkScheduleItem>();

    /// <summary> 8日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleItem> Day8_Schedule { get; set; } = new ReactiveProperty<WorkScheduleItem>();

    /// <summary> 9日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleItem> Day9_Schedule { get; set; } = new ReactiveProperty<WorkScheduleItem>();

    /// <summary> 10日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleItem> Day10_Schedule { get; set; } = new ReactiveProperty<WorkScheduleItem>();

    /// <summary> 11日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleItem> Day11_Schedule { get; set; } = new ReactiveProperty<WorkScheduleItem>();

    /// <summary> 12日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleItem> Day12_Schedule { get; set; } = new ReactiveProperty<WorkScheduleItem>();

    /// <summary> 13日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleItem> Day13_Schedule { get; set; } = new ReactiveProperty<WorkScheduleItem>();

    /// <summary> 14日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleItem> Day14_Schedule { get; set; } = new ReactiveProperty<WorkScheduleItem>();

    /// <summary> 15日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleItem> Day15_Schedule { get; set; } = new ReactiveProperty<WorkScheduleItem>();

    /// <summary> 16日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleItem> Day16_Schedule { get; set; } = new ReactiveProperty<WorkScheduleItem>();

    /// <summary> 17日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleItem> Day17_Schedule { get; set; } = new ReactiveProperty<WorkScheduleItem>();

    /// <summary> 18日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleItem> Day18_Schedule { get; set; } = new ReactiveProperty<WorkScheduleItem>();

    /// <summary> 19日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleItem> Day19_Schedule { get; set; } = new ReactiveProperty<WorkScheduleItem>();

    /// <summary> 20日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleItem> Day20_Schedule { get; set; } = new ReactiveProperty<WorkScheduleItem>();

    /// <summary> 21日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleItem> Day21_Schedule { get; set; } = new ReactiveProperty<WorkScheduleItem>();

    /// <summary> 22日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleItem> Day22_Schedule { get; set; }= new ReactiveProperty<WorkScheduleItem>();

    /// <summary> 23日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleItem> Day23_Schedule { get; set; } = new ReactiveProperty<WorkScheduleItem>();

    /// <summary> 24日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleItem> Day24_Schedule { get; set; } = new ReactiveProperty<WorkScheduleItem>();

    /// <summary> 25日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleItem> Day25_Schedule { get; set; } = new ReactiveProperty<WorkScheduleItem>();

    /// <summary> 26日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleItem> Day26_Schedule { get; set; } = new ReactiveProperty<WorkScheduleItem>();

    /// <summary> 27日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleItem> Day27_Schedule { get; set; } = new ReactiveProperty<WorkScheduleItem>();

    /// <summary> 28日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleItem> Day28_Schedule { get; set; } = new ReactiveProperty<WorkScheduleItem>();

    /// <summary> 29日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleItem> Day29_Schedule { get; set; } = new ReactiveProperty<WorkScheduleItem>();

    /// <summary> 30日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleItem> Day30_Schedule { get; set; } = new ReactiveProperty<WorkScheduleItem>();

    /// <summary> 31日 - スケジュール </summary>
    public ReactiveProperty<WorkScheduleItem> Day31_Schedule { get; set; } = new ReactiveProperty<WorkScheduleItem>();

    #endregion

    /// <summary> 更新ボタン - Command </summary>
    public ReactiveCommand Update_Command { get; private set; } = new ReactiveCommand();
}