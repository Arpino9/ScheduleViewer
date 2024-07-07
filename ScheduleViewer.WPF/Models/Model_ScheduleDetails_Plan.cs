namespace ScheduleViewer.WPF.Models;

/// <summary>
/// Model - スケジュール詳細 (予定一覧)
/// </summary>
public sealed class Model_ScheduleDetails_Plan : ModelBase<ViewModel_ScheduleDetails_Plan>, IViewer
{
    public Model_ScheduleDetails_Plan()
    {
        
    }

    #region Get Instance

    private static Model_ScheduleDetails_Plan model = null;

    public static Model_ScheduleDetails_Plan GetInstance()
    {
        if (model == null)
        {
            model = new Model_ScheduleDetails_Plan();
        }

        return model;
    }

    #endregion

    /// <summary> ViewModel - スケジュール詳細 </summary>
    public ViewModel_ScheduleDetails ViewModel_Header { get; set; }

    /// <summary> ViewModel - スケジュール詳細 (予定一覧) </summary>
    internal override ViewModel_ScheduleDetails_Plan ViewModel { get; set; }

    /// <summary> ViewModel - イメージビューワー </summary>
    internal ViewModel_ImageViewer ViewModel_ImageViewer { get; set; }

    public void Initialize()
    {
        var events = CalendarReader.FindByDate(this.ViewModel_Header.Date.Value);

        this.ViewModel.Events_ItemSource.Clear();

        var schedules = events.Where(x => x.Place != string.Empty);

        foreach (var schedule in schedules)
        {
            this.ViewModel.Events_ItemSource.Add(schedule);
        }

        this.ListView_SelectionChanged();
    }

    /// <summary>
    /// 予定一覧 - SelectionChanged
    /// </summary>
    public void ListView_SelectionChanged()
    {
        using(new CursorWaiting())
        {
            if (this.ViewModel.Events_ItemSource.IsEmpty())
            {
                // リストが空
                return;
            }

            if (this.ViewModel.Events_SelectedIndex.Value.IsUnSelected())
            {
                // 未選択
                return;
            }

            var entity = this.ViewModel.Events_ItemSource[this.ViewModel.Events_SelectedIndex.Value];

            // タイトル
            this.ViewModel.Title_Text.Value = entity.Title;
            // 開始時刻
            this.ViewModel.StartTime_Text.Value = entity.StartDate.ToString("HH:mm");
            // 終了時刻
            this.ViewModel.EndTime_Text.Value = entity.EndDate.ToString("HH:mm");
            // 場所
            this.ViewModel.Place_Text.Value = entity.Place;
            // 詳細
            this.ViewModel.Description_Text.Value = entity.Description;

            // 地図情報
            this.ShowMapImage();

            // 写真
            var photo = JSONExtension.GetPhotoSource(this.ViewModel.Place_Text.Value);

            if (photo.Image != null)
            {
                this.ViewModel.Photo_Source.Value = photo.Image;
                this.ViewModel.Photo_Height.Value = photo.Height;
                this.ViewModel.Photo_Width.Value = photo.Width;
            }
        }
    }

    /// <summary>
    /// Clear - 閲覧項目
    /// </summary>
    public void Clear_ViewForm()
    {
        // 地図
        this.ViewModel.Map_Source.Value   = new BitmapImage();
        // 写真
        this.ViewModel.Photo_Source.Value = new BitmapImage();

        // タイトル
        this.ViewModel.Title_Text.Value       = string.Empty;
        // 開始時刻
        this.ViewModel.StartTime_Text.Value   = string.Empty;
        // 終了時刻
        this.ViewModel.EndTime_Text.Value     = string.Empty;
        // 場所
        this.ViewModel.Place_Text.Value       = string.Empty;
        // 詳細
        this.ViewModel.Description_Text.Value = string.Empty;
    }

    /// <summary>
    /// 地図のイメージを取得する
    /// </summary>
    private async void ShowMapImage()
    {
        var imageUrl = GetImageurl();

        if (string.IsNullOrEmpty(imageUrl)) 
        {
            // 住所が未指定
            return; 
        }

        try
        {
            // Webリクエストを送信して地図画像を取得
            WebClient webClient = new WebClient();
            byte[] imageBytes   = await Task.Run(() => webClient.DownloadData(imageUrl));

            // 地図画像をBitmapImageに変換
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new MemoryStream(imageBytes);
            bitmapImage.EndInit();

            // Imageコントロールに地図画像を表示
            this.ViewModel.Map_Source.Value = bitmapImage;
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Error: {ex.Message}");
        }
    }

    /// <summary>
    /// 住所からGoogle Place APIのURLを取得する
    /// </summary>
    /// <returns>URL</returns>
    private string GetImageurl()
    {
        // 地図情報
        var location = PlaceReader.ReadPlaceLocation(this.ViewModel.Place_Text.Value);

        if (location == (double.MinValue, double.MinValue))
        {
            // 住所が未指定
            return string.Empty;
        }

        // Google Maps Static APIのURLを構築
        string latitude  = location.Latitude.Value.ToString(); // 緯度
        string longitude = location.Longitude.Value.ToString(); // 経度
        int zoom         = 15; // ズームレベル

        return $"https://maps.googleapis.com/maps/api/staticmap?center={latitude},{longitude}&zoom={zoom}&size=600x400&markers=color:red%7C{latitude},{longitude}&key={Shared.API_Key}";
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
