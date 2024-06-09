namespace ScheduleViewer.WPF.ViewModels;

/// <summary>
/// ViewModel - イメージビューワー
/// </summary>
public class ViewModel_ImageViewer : ViewModelBase<Model_ScheduleDetails_Plan>
{
    public override event PropertyChangedEventHandler PropertyChanged;

    public ViewModel_ImageViewer()
    {
        this.Model.ViewModel_ImageViewer = this;
    }

    protected override void BindEvents()
    {
        throw new NotImplementedException();
    }

    /// <summary> Model - 添付ファイル管理 </summary>
    protected override Model_ScheduleDetails_Plan Model => Model_ScheduleDetails_Plan.GetInstance();

    #region Window

    /// <summary> Window - Title </summary>
    public ReactiveProperty<string> Window_Title { get; set; } = new ReactiveProperty<string>();

    #endregion

    #region 画像

    /// <summary> 画像 - Height </summary>
    public ReactiveProperty<double> Image_Height { get; set; } = new ReactiveProperty<double>();

    /// <summary> 画像 - Width </summary>
    public ReactiveProperty<double> Image_Width { get; set; } = new ReactiveProperty<double>();

    /// <summary> 画像 - Image </summary>
    public ReactiveProperty<ImageSource> Image_Source { get; set; }
        = new ReactiveProperty<ImageSource>();

    #endregion
}
