namespace ScheduleViewer.WPF.ViewModels;

public sealed class ViewModel_ScheduleDetails_Photo : ViewModelBase<Model_ScheduleDetails_Photo>
{
    protected override Model_ScheduleDetails_Photo Model => Model_ScheduleDetails_Photo.GetInstance();

    public override event PropertyChangedEventHandler PropertyChanged;

    public ViewModel_ScheduleDetails_Photo()
    {
        this.Model.ViewModel = this;

        this.Model.Initialize();

        this.BindEvents();
    }

    protected override void BindEvents()
    {
        Photos_SelectionChanged.Subscribe(_ => this.Model.Clear_ViewForm());
        Photos_SelectionChanged.Subscribe(_ => this.Model.ListView_SelectionChanged());

        Photo_MouseDoubleClick.Subscribe(_ => this.Model.OpenImageViewer(this.FileName.Value,
                                                                         this.Height.Value,
                                                                         this.Width.Value,
                                                                         this.Image.Value));
    }

    /// <summary> 一覧 - ItemSource </summary>
    public ReactiveCollection<PhotoEntity> Photos_ItemSource { get; set; } = new ReactiveCollection<PhotoEntity>();

    /// <summary> 一覧 - SelectedIndex </summary>
    public ReactiveProperty<int> Photos_SelectedIndex { get; set; } = new ReactiveProperty<int>();

    /// <summary> 一覧 - SelectionChanged </summary>
    public ReactiveCommand Photos_SelectionChanged { get; private set; } = new ReactiveCommand();

    /// <summary> 写真 - MouseLeftButtonDown </summary>
    public ReactiveCommand Photo_MouseDoubleClick { get; private set; } = new ReactiveCommand();

    /// <summary> ID - Text </summary>
    public ReactiveProperty<string> ID { get; set; } = new ReactiveProperty<string>();

    /// <summary> ファイル名 - Text </summary>
    public ReactiveProperty<string> FileName { get; set; } = new ReactiveProperty<string>();

    /// <summary> 説明 - Text </summary>
    public ReactiveProperty<string> Description { get; set; } = new ReactiveProperty<string>();

    /// <summary> 画像 - Text </summary>
    public ReactiveProperty<BitmapImage> Image { get; set; } = new ReactiveProperty<BitmapImage>();

    /// <summary> URL - Text </summary>
    public ReactiveProperty<string> URL { get; set; } = new ReactiveProperty<string>();

    /// <summary> MIMEタイプ - Text </summary>
    public ReactiveProperty<string> MineType { get; set; } = new ReactiveProperty<string>();

    /// <summary> 高さ - Text </summary>
    public ReactiveProperty<long> Height { get; set; } = new ReactiveProperty<long>();

    /// <summary> 幅 - Text </summary>
    public ReactiveProperty<long> Width { get; set; } = new ReactiveProperty<long>();
}
