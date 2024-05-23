using ScheduleViewer.WPF.Models;

namespace ScheduleViewer.WPF.ViewModels;

/// <summary>
/// ViewModel - 勤怠表 (ヘッダ)
/// </summary>
public class ViewModel_WorkSchedule_Header : ViewModelBase
{
    public override event PropertyChangedEventHandler PropertyChanged;

    public ViewModel_WorkSchedule_Header()
    {
        this.Model.ViewModel_Header = this;

        using (var _ = new CursorWaiting())
        {
            this.Model.Initialize_Header();
        }

        this.BindEvents();
    }

    protected override void BindEvents()
    {
        this.Return_Command.Subscribe(_ => this.Model.Return());
        this.Proceed_Command.Subscribe(_ => this.Model.Proceed());
    }

    /// <summary>
    /// Model - 勤務表
    /// </summary>
    private Model_WorkSchedule Model = Model_WorkSchedule.GetInstance();

    #region Window

    /// <summary> Window - Title </summary>
    public ReactiveProperty<string> Window_Title { get; }
        = new ReactiveProperty<string>("勤怠表");

    #endregion

    #region 派遣元

    /// <summary> 派遣元 - Text </summary>
    public ReactiveProperty<string> DispatchingCompany_Text { get; set; }
        = new ReactiveProperty<string>();

    #endregion

    #region 派遣先

    /// <summary> 派遣先 - Text </summary>
    public ReactiveProperty<string> DispatchedCompany_Text { get; set; }
        = new ReactiveProperty<string>();

    #endregion

    #region 勤務日数

    /// <summary> 勤務日数 - Text </summary>
    public ReactiveProperty<string> WorkDaysTotal_Text { get; set; }
        = new ReactiveProperty<string>();

    #endregion

    #region 残業時間

    /// <summary> 残業時間 - Text </summary>
    public ReactiveProperty<string> OvertimeTotal_Text { get; set; }
        = new ReactiveProperty<string>();

    #endregion

    #region 勤務時間

    /// <summary> 勤務時間 - Text </summary>
    public ReactiveProperty<string> WorkingTimeTotal_Text { get; set; }
        = new ReactiveProperty<string>();

    #endregion

    #region 欠勤時間

    /// <summary> 欠勤時間 - Text </summary>
    public ReactiveProperty<string> AbsentTime_Text { get; set; }
        = new ReactiveProperty<string>();

    #endregion

    #region 欠勤日数

    /// <summary> 欠勤日数 - Text </summary>
    public ReactiveProperty<string> Absent_Text { get; set; }
        = new ReactiveProperty<string>();

    #endregion

    #region 有給日数

    /// <summary> 有給日数 - Text </summary>
    public ReactiveProperty<string> PaidVacation_Text { get; set; }
        = new ReactiveProperty<string>();

    #endregion

    #region 該当年月

    /// <summary> 年 - Text </summary>
    public ReactiveProperty<int> Year_Text { get; set; }
        = new ReactiveProperty<int>();

    /// <summary> 月 - Text </summary>
    public ReactiveProperty<int> Month_Text { get; set; }
        = new ReactiveProperty<int>();

    #endregion

    #region 戻る

    /// <summary> 戻る - Command </summary>
    public ReactiveCommand Return_Command { get; private set; }
        = new ReactiveCommand();

    #endregion

    #region 進む

    /// <summary> 進む - Command </summary>
    public ReactiveCommand Proceed_Command { get; private set; }
        = new ReactiveCommand();

    #endregion

}