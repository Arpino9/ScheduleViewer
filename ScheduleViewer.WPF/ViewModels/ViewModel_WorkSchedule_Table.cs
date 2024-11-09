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

    /// <summary> 更新ボタン - Command </summary>
    public ReactiveCommand Update_Command { get; private set; } = new ReactiveCommand();
}