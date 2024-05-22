namespace ScheduleViewer.Domain.Modules.Logics;

/// <summary>
/// ファイルパス
/// </summary>
public class FilePath
{
    /// <summary>
    /// プロジェクト名
    /// </summary>
    public enum ProjectName
    {
        /// <summary> ドメイン層 </summary>
        Domain,

        /// <summary> インフラストラクチャ層 </summary>
        Infrastructure,

        /// <summary> WPF層 </summary>
        WPF,
    }

    #region ソリューション

    /// <summary>
    /// ソリューションのパスを取得する
    /// </summary>
    /// <returns>ソリューションのパス</returns>
    public static string GetSolutionPath()
    {
        var appPath = FilePath.GetAppPath();
        var solutionName = FilePath.GetSolutionName();
        var solutionPath = appPath.Substring(0, appPath.IndexOf(solutionName));

        return $"{solutionPath}{solutionName}";
    }

    /// <summary>
    /// ソリューション名を取得する
    /// </summary>
    /// <returns>ソリューション名</returns>
    public static string GetSolutionName()
    {
        var projectName = Assembly.GetExecutingAssembly().GetName().Name;

        return (projectName.Substring(0, projectName.IndexOf(".")));
    }

    #endregion

    #region プロジェクト

    /// <summary>
    /// プロジェクトのパスを取得する
    /// </summary>
    /// <param name="projectName">プロジェクト名</param>
    /// <returns>プロジェクトのパス</returns>
    public static string GetProjectPath(ProjectName projectName)
        => $"{FilePath.GetSolutionPath()}\\{FilePath.GetSolutionName()}.{projectName.ToString()}";

    #endregion

    #region exe

    /// <summary>
    /// exeのパスを取得する。
    /// </summary>
    /// <returns>exeファイルのパス</returns>
    public static string GetAppPath()
        => (Assembly.GetExecutingAssembly().Location);

    /// <summary>
    /// exeのフォルダパスを取得する。
    /// </summary>
    /// <returns>exeファイルのパス</returns>
    public static string GetAppFolderPath()
    {
        var exePath = Assembly.GetExecutingAssembly().Location;
        return (exePath.Substring(0, exePath.LastIndexOf(FilePath.GetSolutionName())));
    }

    #endregion

    #region SQLite

    /// <summary>
    /// SQLiteの初期パスを取得する
    /// </summary>
    /// <returns>SQLiteの初期パス</returns>
    public static string GetSQLiteDefaultPath()
        => $"{FilePath.GetProjectPath(ProjectName.Infrastructure)}\\{FilePath.GetSolutionName()}.db";

    #endregion

    #region Excel

    /// <summary>
    /// Excelテンプレートの初期パスを取得する
    /// </summary>
    /// <returns>SQLiteの初期パス</returns>
    public static string GetExcelTempleteDefaultPath()
        => $"{FilePath.GetProjectPath(ProjectName.Domain)}\\Template\\{Shared.ExcelTemplateName}.xlsx";

    #endregion

    #region XML

    /// <summary>
    /// XMLの初期パスを取得する
    /// </summary>
    /// <returns>XMLの初期パス</returns>
    public static string GetXMLDefaultPath()
        => $"{FilePath.GetProjectPath(ProjectName.Infrastructure)}\\{Shared.XMLName}.xml";

    #endregion

    #region JSON

    /// <summary>
    /// JSONの初期パスを取得する
    /// </summary>
    /// <returns>JSONの初期パス</returns>
    public static string GetJSONDefaultPath()
        => $"{FilePath.GetProjectPath(ProjectName.Infrastructure)}\\{Shared.JSONName}.json";

    /// <summary>
    /// JSONの初期パスを取得する
    /// </summary>
    /// <returns>JSONの初期パス</returns>
    public static string GetJSONHolidayDefaultPath()
        => $"{FilePath.GetProjectPath(ProjectName.Infrastructure)}\\Holiday.json";

    #endregion

    /// <summary>
    /// デスクトップのパスを取得する
    /// </summary>
    /// <returns>デスクトップのパス</returns>
    public static string GetDesktopPath()
        => Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
}