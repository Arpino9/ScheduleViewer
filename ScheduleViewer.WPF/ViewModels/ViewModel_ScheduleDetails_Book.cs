namespace ScheduleViewer.WPF.ViewModels;

/// <summary>
/// ViewModel - スケジュール詳細 (本一覧)
/// </summary>
public sealed class ViewModel_ScheduleDetails_Book : ViewModelBase
{
    public override event PropertyChangedEventHandler PropertyChanged;

    public ViewModel_ScheduleDetails_Book()
    {
        this.Model.ViewModel = this;

        this.Model.Initialize();

        this.BindEvents();
    }

    protected override void BindEvents()
    {
        Books_SelectionChanged.Subscribe(x => this.Model.ListView_SelectionChanged());
    }

    /// <summary> Model - スケジュール詳細 (本一覧) </summary>
    public Model_ScheduleDetails_Book Model = Model_ScheduleDetails_Book.GetInstance();

    /// <summary> 一覧 - ItemSource </summary>
    public ReactiveCollection<BookEntity> Books_ItemSource { get; set; } = new ReactiveCollection<BookEntity>();

    /// <summary> 一覧 - SelectedIndex </summary>
    public ReactiveProperty<int> Books_SelectedIndex { get; set; } = new ReactiveProperty<int>();

    /// <summary> 一覧 - SelectionChanged </summary>
    public ReactiveCommand Books_SelectionChanged { get; private set; } = new ReactiveCommand();

    /// <summary> タイトル - Text </summary>
    public ReactiveProperty<string> Title_Text { get; set; } = new ReactiveProperty<string>();

    /// <summary> 日付 - Text </summary>
    public ReactiveProperty<DateTime> ReadDate_Text { get; set; } = new ReactiveProperty<DateTime>();

    /// <summary> 著者 - Text </summary>
    public ReactiveProperty<string> Author_Text { get; set; } = new ReactiveProperty<string>();

    /// <summary> 出版社 - Text </summary>
    public ReactiveProperty<string> Publisher_Text { get; set; } = new ReactiveProperty<string>();

    /// <summary> 発売日 - Text </summary>
    public ReactiveProperty<string> ReleasedDate_Text { get; set; } = new ReactiveProperty<string>();

    /// <summary> 本のタイプ - Text </summary>
    public ReactiveProperty<string> Type_Text { get; set; } = new ReactiveProperty<string>();

    /// <summary> ISBN-10 - Text </summary>
    public ReactiveProperty<string> ISBN_10_Text { get; set; } = new ReactiveProperty<string>();

    /// <summary> ISBN-13 - Text </summary>
    public ReactiveProperty<string> ISBN_13_Text { get; set; } = new ReactiveProperty<string>();

    /// <summary> 概要 - Text </summary>
    public ReactiveProperty<string> Caption_Text { get; set; } = new ReactiveProperty<string>();
    
    /// <summary> 評価 - Text </summary>
    public ReactiveProperty<string> Rating_Text { get; set; } = new ReactiveProperty<string>();
}
