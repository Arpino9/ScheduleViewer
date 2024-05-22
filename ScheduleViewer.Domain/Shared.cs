namespace ScheduleViewer.Domain;

/// <summary>
/// Shared
/// </summary>
/// <remarks>
/// configファイルとDomain, Infrastructure層の橋渡しクラス。
/// </remarks>
public static class Shared
{
    /// <summary> 読み込むDBのパス </summary>
    public static string DatabasePath = ConfigurationManager.AppSettings["DatabasePath"];

    /// <summary> 日付フォーマット </summary>
    public static string YearFormat = ConfigurationManager.AppSettings["YearFormat"];

    /// <summary> CSVの保存ディレクトリ </summary>
    public static string DirectoryCSV = ConfigurationManager.AppSettings["DirectoryCSV"];

    /// <summary> 給与明細Excelのテンプレートファイル名 </summary>
    public static string ExcelTemplateName = ConfigurationManager.AppSettings["ExcelTemplateName"];

    /// <summary> オプションの保存形式 </summary>
    public static string SavingExtension = ConfigurationManager.AppSettings["SavingExtension"];

    /// <summary> XMLのファイル名 </summary>
    public static string XMLName = ConfigurationManager.AppSettings["XMLName"];

    /// <summary> JSONのファイル名 </summary>
    public static string JSONName = ConfigurationManager.AppSettings["JSONName"];

    /// <summary> フォントファミリ </summary>
    public static string FontFamily = ConfigurationManager.AppSettings["FontFamily"];

    /// <summary> フォントサイズ </summary>
    public static string FontSize = ConfigurationManager.AppSettings["FontSize"];

    /// <summary> 初期表示時にデフォルト明細を表示する </summary>
    public static string ShowDefaultPayslip = ConfigurationManager.AppSettings["ShowDefaultPayslip"];

    /// <summary> システム名 </summary>
    public static string SystemName = ConfigurationManager.AppSettings["SystemName"];
}