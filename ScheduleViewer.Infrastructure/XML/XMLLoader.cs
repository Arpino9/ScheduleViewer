namespace ScheduleViewer.Infrastructure.XML;

/// <summary>
/// XMLローダー
/// </summary>
/// <remarks>
/// staticValuesと挙動は同じだが、using絡みで他XMLクラスに依存するため分離。
/// 呼び出しが面倒(コンストラクタ部分にあたるDeserialize()で逐一usingする必要がある)なので、
/// あえてインターフェースを介さないことにした。
/// </remarks>
public static class XMLLoader
{
    static XMLLoader()
    {
        XMLLoader.Deserialize();
    }

    private static XMLTag _tag;

    /// <summary>
    /// デシリアライズ
    /// </summary>
    public static void Deserialize()
    {
        using (var reader = new XMLReader(FilePath.GetXMLDefaultPath(), new XMLTag().GetType()))
        {
            _tag = reader.Deserialize() as XMLTag;
        }
    }

    /// <summary>
    /// SQLiteのパスを取得
    /// </summary>
    /// <returns>Excelテンプレートのパス</returns>
    public static string FetchSQLitePath()
    {
        XMLLoader.Deserialize();
        return _tag?.SQLitePath ?? FilePath.GetSQLiteDefaultPath(); ;
    }

    /// <summary>
    /// Excelテンプレートのパスを取得
    /// </summary>
    /// <returns>Excelテンプレートのパス</returns>
    public static string FetchExcelTemplatePath()
    {
        XMLLoader.Deserialize();
        return _tag?.ExcelTemplatePath ?? FilePath.GetExcelTempleteDefaultPath();
    }

    /// <summary>
    /// フォントファミリを取得
    /// </summary>
    /// <returns>フォントファミリ</returns>
    public static string FetchFontFamilyText()
    {
        XMLLoader.Deserialize();
        return _tag?.FontFamily ?? Shared.FontFamily;
    }

    /// <summary>
    /// フォントサイズを取得
    /// </summary>
    /// <returns>フォントサイズ</returns>
    public static decimal FetchFontSize()
    {
        XMLLoader.Deserialize();
        return _tag?.FontSize ?? decimal.Parse(Shared.FontSize);
    }

    /// <summary>背景色の初期値。</summary>
    private static readonly ArgbColorValue DefaultBackgroundColor = new(255, 227, 227, 227);

    /// <summary>
    /// 背景色をARGB値として取得します。
    /// </summary>
    /// <returns>UIフレームワークに依存しない背景色。</returns>
    public static ArgbColorValue FetchBackgroundColor()
    {
        XMLLoader.Deserialize();
        if (string.IsNullOrWhiteSpace(_tag?.BackgroundColor))
        {
            return DefaultBackgroundColor;
        }

        var color = _tag.BackgroundColor.Separate();

        return new ArgbColorValue(
            ParseColorComponent(color, 0),
            ParseColorComponent(color, 1),
            ParseColorComponent(color, 2),
            ParseColorComponent(color, 3));
    }

    /// <summary>
    /// ARGB配列から指定位置の色成分を取得します。
    /// </summary>
    /// <param name="color">ARGB順の色成分。</param>
    /// <param name="index">取得する位置。</param>
    /// <returns>色成分。指定位置が存在しない場合は0。</returns>
    private static byte ParseColorComponent(IReadOnlyList<string> color, int index)
        => index < color.Count ? byte.Parse(color[index]) : (byte)0;

    /// <summary>
    /// 「初期表示時にデフォルト明細を表示する」のチェック有無を取得する
    /// </summary>
    /// <returns></returns>
    public static bool FetchShowDefaultPayslip()
    {
        XMLLoader.Deserialize();
        return _tag?.ShowDefaultPayslip ?? bool.Parse(Shared.ShowDefaultPayslip);
    }

    /// <summary>
    /// 画像の保存方法を取得する
    /// </summary>
    /// <returns></returns>
    public static string FetchHowToSaveImage()
    {
        XMLLoader.Deserialize();
        return _tag?.HowToSaveImage;
    }

    /// <summary>
    /// 画像の格納フォルダパスを取得する
    /// </summary>
    /// <returns>画像の格納フォルダパ</returns>
    public static string FetchImageFolder()
    {
        XMLLoader.Deserialize();
        return _tag?.ImageFolderPath ?? FilePath.GetDesktopPath();
    }

    /// <summary>
    /// 認証ファイルのパスを取得する
    /// </summary>
    /// <returns>認証ファイルのパス</returns>
    public static string FetchPrivateKeyPath_SpreadSheet()
    {
        XMLLoader.Deserialize();
        return _tag?.PrivateKeyPath_SpreadSheet ?? string.Empty;
    }

    /// <summary>
    /// 認証ファイルのパスを取得する
    /// </summary>
    /// <returns>認証ファイルのパス</returns>
    public static string FetchPrivateKeyPath_Calendar()
    {
        XMLLoader.Deserialize();
        return _tag?.PrivateKeyPath_Calendar ?? string.Empty;
    }

    /// <summary>
    /// カレンダーIDを取得する
    /// </summary>
    /// <returns>カレンダーID</returns>
    public static string FetchCalendarId()
    {
        XMLLoader.Deserialize();
        return _tag?.CalendarId ?? string.Empty;
    }

    /// <summary>
    /// シートIDを取得する
    /// </summary>
    /// <returns>シートID</returns>
    public static string FetchSheetId()
    {
        XMLLoader.Deserialize();
        return _tag?.SheetId ?? string.Empty;
    }

    /// <summary>
    /// PDFのパスワードを取得する
    /// </summary>
    /// <returns>PDFのパスワード</returns>
    public static string FetchPDFPassword()
    {
        XMLLoader.Deserialize();
        return _tag?.PDFPassword ?? string.Empty;
    }
}
