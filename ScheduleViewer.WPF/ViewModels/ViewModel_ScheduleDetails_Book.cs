namespace ScheduleViewer.WPF.ViewModels;

/// <summary>
/// ViewModel - スケジュール詳細 (本一覧)
/// </summary>
public sealed class ViewModel_ScheduleDetails_Book : ViewModelBase<Model_ScheduleDetails_Book>
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
        this.Books_SelectionChanged.Subscribe(x => this.Model.Clear_ViewForm());
        this.Books_SelectionChanged.Subscribe(x => this.Model.ListView_SelectionChanged());
        
        this.SortByTitle_Command.Subscribe(x => this.Model.SortByTitle());
        this.SortByAuthor_Command.Subscribe(x => this.Model.SortByAuthor());
        this.SortByPublisher_Command.Subscribe(x => this.Model.SortByPublisher());
    }

    /// <summary> Model - スケジュール詳細 (本一覧) </summary>
    protected override Model_ScheduleDetails_Book Model => Model_ScheduleDetails_Book.GetInstance();

    /// <summary> 一覧 - ItemSource </summary>
    public ReactiveCollection<BookEntity> Books_ItemSource { get; set; } = new ReactiveCollection<BookEntity>();

    /// <summary> 一覧 - SelectedIndex </summary>
    public ReactiveProperty<int> Books_SelectedIndex { get; set; } = new ReactiveProperty<int>();

    /// <summary> 一覧 - SelectionChanged </summary>
    public ReactiveCommand Books_SelectionChanged { get; private set; } = new ReactiveCommand();

    /// <summary> ソート(タイトル) - Command </summary>
    public ReactiveCommand SortByTitle_Command { get; } = new ReactiveCommand();

    /// <summary> ソート(読了日) - Command </summary>
    public ReactiveCommand SortByReadDate_Command { get; } = new ReactiveCommand();

    /// <summary> ソート(著者) - Command </summary>
    public ReactiveCommand SortByAuthor_Command { get; } = new ReactiveCommand();

    /// <summary> ソート(出版社) - Command </summary>
    public ReactiveCommand SortByPublisher_Command { get; } = new ReactiveCommand();

    /// <summary> ソート(発売日) - Command </summary>
    public ReactiveCommand SortByReleasedDate_Command { get; } = new ReactiveCommand();

    /// <summary> ソート(本のタイプ) - Command </summary>
    public ReactiveCommand SortByType_Command { get; } = new ReactiveCommand();

    /// <summary> タイトル - Text </summary>
    public ReactiveProperty<string> Title_Text { get; set; } = new ReactiveProperty<string>();

    /// <summary> 読了日 - Text </summary>
    public ReactiveProperty<string> ReadDate_Text { get; set; } = new ReactiveProperty<string>();

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

    /// <summary> サムネイル - Text </summary>
    public ReactiveProperty<BitmapImage> Thumbnail_Source { get; set; } = new ReactiveProperty<BitmapImage>();

    /// <summary> 評価 - Text </summary>
    public ReactiveProperty<string> Rating_Text { get; set; } = new ReactiveProperty<string>();
}
