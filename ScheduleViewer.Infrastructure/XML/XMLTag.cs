namespace ScheduleViewer.Infrastructure.XML;

/// <summary>
/// XMLタグ
/// </summary>
/// <remarks>
/// XML生成時に出力されるタグの一覧。適宜追加すること。
/// </remarks>
public sealed record class XMLTag
{
    /// <summary> Excelテンプレートのパス </summary>
    public string ExcelTemplatePath;

    /// <summary> SQLiteのパス </summary>
    public string SQLitePath;

    /// <summary> フォントファミリ </summary>
    public string FontFamily;

    /// <summary> フォントサイズ </summary>
    public decimal FontSize;

    /// <summary> 初期表示時にデフォルト明細を表示する </summary>
    public bool ShowDefaultPayslip;

    /// <summary> 背景色 (カラーコード) </summary>
    public string BackgroundColor_ColorCode;

    /// <summary> 背景色 (ARGB) </summary>
    public string BackgroundColor;

    /// <summary> DBへの画像の保存方法 </summary>
    public string HowToSaveImage;
    /// <summary> 画像の格納パス </summary>
    public string ImageFolderPath;

    /// <summary> 認証ファイル </summary>
    public string PrivateKeyPath_SpreadSheet;
    /// <summary> シートID </summary>
    public string SheetId;

    /// <summary> 認証ファイル </summary>
    public string PrivateKeyPath_Calendar;
    /// <summary> カレンダーID </summary>
    public string CalendarId;

    /// <summary> PDFのパスワード </summary>
    public string PDFPassword;
}