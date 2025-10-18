
namespace ScheduleViewer.WPF.ViewModels;

public class ViewModel_ScheduleDetails_Anime : ViewModelBase<Model_ScheduleDetails_Anime>
{
    public override event PropertyChangedEventHandler PropertyChanged;

    public ViewModel_ScheduleDetails_Anime()
    {
        this.Model.ViewModel = this;

        this.Model.InitializeAsync();
        this.BindEvents();
    }

    protected override Model_ScheduleDetails_Anime Model => Model_ScheduleDetails_Anime.GetInstance();

    protected override void BindEvents()
    {
        this.Animes_SelectionChanged.Subscribe(x => this.Model.Clear_ViewForm());
        this.Animes_SelectionChanged.Subscribe(x => this.Model.ListView_SelectionChanged());

        this.SortByTitle_Command.Subscribe(x => this.Model.SortByTitle());
        this.SortByPart_Command.Subscribe(x => this.Model.SortByPart());
        this.SortBySubTitle_Command.Subscribe(x => this.Model.SortBySubTitle());
        this.SortByWatchedFrom_Command.Subscribe(x => this.Model.SortByWatchedFrom());
    }

    /// <summary> 一覧 - ItemSource </summary>
    public ReactiveCollection<AnimeEntity> Animes_ItemSource { get; set; } = new ReactiveCollection<AnimeEntity>();

    /// <summary> 一覧 - SelectedIndex </summary>
    public ReactiveProperty<int> Animes_SelectedIndex { get; set; } = new ReactiveProperty<int>();

    /// <summary> 一覧 - SelectionChanged </summary>
    public ReactiveCommand Animes_SelectionChanged { get; private set; } = new ReactiveCommand();

    /// <summary> ソート(著者) - Command </summary>
    public ReactiveCommand SortByTitle_Command { get; } = new ReactiveCommand();

    /// <summary> ソート(話数) - Command </summary>
    public ReactiveCommand SortByPart_Command { get; } = new ReactiveCommand();

    /// <summary> ソート(サブタイトル) - Command </summary>
    public ReactiveCommand SortBySubTitle_Command { get; } = new ReactiveCommand();

    /// <summary> ソート(視聴先) - Command </summary>
    public ReactiveCommand SortByWatchedFrom_Command { get; } = new ReactiveCommand();

    /// <summary> タイトル - Text </summary>
    public ReactiveProperty<string> Title_Text { get; set; } = new ReactiveProperty<string>();

    /// <summary> 話数 - Text </summary>
    public ReactiveProperty<string> Part_Text { get; set; } = new ReactiveProperty<string>();

    /// <summary> サブタイトル - Text </summary>
    public ReactiveProperty<string> Subtitle_Text { get; set; } = new ReactiveProperty<string>();

    /// <summary> 視聴先 - Text </summary>
    public ReactiveProperty<string> WatchedFrom_Text { get; set; } = new ReactiveProperty<string>();

    /// <summary> 概要 - Text </summary>
    public ReactiveProperty<string> Caption_Text { get; set; } = new ReactiveProperty<string>();

    /// <summary> キャスト - Text </summary>
    public ReactiveProperty<string> Casts_Text { get; set; } = new ReactiveProperty<string>();
    
    /// <summary> 放送時期 - Text </summary>
    public ReactiveProperty<string> Season_Text { get; set; } = new ReactiveProperty<string>();
    
    /// <summary> 視聴先 - Text </summary>
    public ReactiveProperty<string> WatchedFrom { get; set; } = new ReactiveProperty<string>();

    /// <summary> 公式サイト - Text </summary>
    public ReactiveProperty<string> OfficialSite_Text { get; set; } = new ReactiveProperty<string>();

    /// <summary> Wikipedia - Text </summary>
    public ReactiveProperty<string> WikipediaUrl_Text { get; set; } = new ReactiveProperty<string>();

    /// <summary> サムネイル - Source </summary>
    public ReactiveProperty<BitmapImage> Thumbnail_Source { get; set; } = new ReactiveProperty<BitmapImage>();



}
