namespace ScheduleViewer.WPF.ViewModels;

/// <summary>
/// ViewModel - イメージビューワー
/// </summary>
public class ViewModel_ImageViewer : ViewModelBase<Model_ImageViewer>
{
    public override event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <remarks>
    /// 対応するUserControlのModelを設定する。
    /// </remarks>
    public ViewModel_ImageViewer()
    {
        this.Model_Plan.ViewModel_ImageViewer  = this;
        this.Model_Photo.ViewModel_ImageViewer = this;
    }

    protected override void BindEvents()
    {
        throw new NotImplementedException();
    }

    /// <summary> Model - スケジュール </summary>
    protected Model_ScheduleDetails_Plan Model_Plan => Model_ScheduleDetails_Plan.GetInstance();
    
    /// <summary> Model - 添付ファイル管理 </summary>
    protected Model_ScheduleDetails_Photo Model_Photo => Model_ScheduleDetails_Photo.GetInstance();

    #region Window

    /// <summary> Window - Title </summary>
    public ReactiveProperty<string> Window_Title { get; set; } = new ReactiveProperty<string>();

    #endregion

    #region 画像

    /// <summary> Model - ビューワー </summary>
    protected override Model_ImageViewer Model => Model_ImageViewer.GetInstance();

    /// <summary> 画像 - Height </summary>
    public ReactiveProperty<double> Image_Height { get; set; } = new ReactiveProperty<double>();

    /// <summary> 画像 - Width </summary>
    public ReactiveProperty<double> Image_Width { get; set; } = new ReactiveProperty<double>();

    /// <summary> 画像 - Image </summary>
    public ReactiveProperty<ImageSource> Image_Source { get; set; }
        = new ReactiveProperty<ImageSource>();

    #endregion
}
