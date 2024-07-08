namespace ScheduleViewer.WPF.Models;

/// <summary>
/// Model - 写真
/// </summary>
public class Model_ScheduleDetails_Photo : ModelBase<ViewModel_ScheduleDetails_Photo>, IViewer
{

    #region Get Instance

    private static Model_ScheduleDetails_Photo model = null;

    public static Model_ScheduleDetails_Photo GetInstance()
    {
        if (model == null)
        {
            model = new Model_ScheduleDetails_Photo();
        }

        return model;
    }

    #endregion

    /// <summary> ViewModel - 写真 </summary>
    internal override ViewModel_ScheduleDetails_Photo ViewModel { get; set; }
    
    /// <summary> ViewModel - スケジュール詳細 (本一覧) </summary>
    public ViewModel_ScheduleDetails ViewModel_Header { get; set; }

    /// <summary> ViewModel - イメージビューワー </summary>
    internal ViewModel_ImageViewer ViewModel_ImageViewer { get; set; }

    public void Clear_ViewForm()
    {
        // ID
        this.ViewModel.ID.Value = string.Empty;

        // ファイル名
        this.ViewModel.FileName.Value = string.Empty;

        // 説明
        this.ViewModel.Description.Value = string.Empty;

        // 画像
        this.ViewModel.Image.Value  = new BitmapImage();
        this.ViewModel.Height.Value = default(long);
        this.ViewModel.Width.Value  = default(long);

        // URL
        this.ViewModel.URL.Value = string.Empty;

        // MIMEタイプ
        this.ViewModel.MineType.Value = string.Empty;
    }

    public void Initialize()
    {
        this.ViewModel.Photos_ItemSource.Clear();

        var photos = GoogleFacade.Photo.FindByDate(this.ViewModel_Header.Date.Value);

        foreach (var photo in photos)
        {
            this.ViewModel.Photos_ItemSource.Add(photo);
        }

        this.ListView_SelectionChanged();
    }

    public void ListView_SelectionChanged()
    {
        if (this.ViewModel.Photos_ItemSource.IsEmpty())
        {
            // リストが空
            return;
        }

        if (this.ViewModel.Photos_SelectedIndex.Value.IsUnSelected())
        {
            // 未選択
            return;
        }

        var entity = this.ViewModel.Photos_ItemSource[this.ViewModel.Photos_SelectedIndex.Value];

        // ID
        this.ViewModel.ID.Value = entity.ID;

        // ファイル名
        this.ViewModel.FileName.Value = entity.FileName;

        // 説明
        this.ViewModel.Description.Value = entity.Description;

        // 画像
        this.ViewModel.Image.Value  = entity.Image;
        this.ViewModel.Height.Value = entity.Height;
        this.ViewModel.Width.Value  = entity.Width;

        // URL
        this.ViewModel.URL.Value = entity.URL;

        // MIMEタイプ
        this.ViewModel.MineType.Value = entity.MimeType;
    }

    /// <summary>
    /// イメージビューアーを開く
    /// </summary>
    /// <param name="title">タイトル</param>
    /// <param name="height">高さ</param>
    /// <param name="width">幅</param>
    /// <param name="image">画像</param>
    internal void OpenImageViewer(string title, double height, double width, ImageSource image)
    {
        this.ViewModel_ImageViewer = new ViewModel_ImageViewer();

        var viewer = new ImageViewer();

        this.ViewModel_ImageViewer.Window_Title.Value = title;
        this.ViewModel_ImageViewer.Image_Height.Value = height;
        this.ViewModel_ImageViewer.Image_Width.Value  = width;
        this.ViewModel_ImageViewer.Image_Source.Value = image;

        viewer.Show();
    }
}
