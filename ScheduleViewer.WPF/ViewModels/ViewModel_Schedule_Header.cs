
namespace ScheduleViewer.WPF.ViewModels;

public sealed class ViewModel_Schedule_Header : ViewModelBase<Model_Schedule>
{
    public override event PropertyChangedEventHandler PropertyChanged;

    public ViewModel_Schedule_Header()
    {
        this.Model.ViewModel_Header = this;

        using (var _ = new CursorWaiting())
        {
            this.Model.Initialize_HeaderAsync();
        }

        this.BindEvents();
    }

    #region 該当年月

    /// <summary> 年 - Text </summary>
    public ReactiveProperty<int> Year_Text { get; set; }
        = new ReactiveProperty<int>();

    /// <summary> 月 - Text </summary>
    public ReactiveProperty<int> Month_Text { get; set; }
        = new ReactiveProperty<int>();

    #endregion

    /// <summary>
    /// Model - スケジュール
    /// </summary>
    protected override Model_Schedule Model => Model_Schedule.GetInstance();

    protected override void BindEvents()
    {
        this.Return_Command.Subscribe(_ => this.Model.ReturnAsync());
        this.Proceed_Command.Subscribe(_ => this.Model.ProceedAsync());
    }

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
