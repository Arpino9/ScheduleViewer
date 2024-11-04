namespace ScheduleViewer.Domain.Modules.Helpers;

/// <summary>
/// Utility -  Log
/// </summary>
/// <remarks>
/// エラー関連は例外クラスでも出力される。
/// </remarks>
public static class LogUtils
{
    private static readonly log4net.ILog _logger =
      log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// デバッグログを出力する
    /// </summary>
    /// <param name="className">クラス名</param>
    /// <param name="message">メッセージ</param>
    public static void Debug(string className, string message)
        => _logger.Debug($"【DEBUG】[{className}] {message}");

    /// <summary>
    /// インフォメーションログを出力する
    /// </summary>
    /// <param name="className">クラス名</param>
    /// <param name="message">メッセージ</param>
    public static void Info(string className, string message)
        => _logger.Info($"【INFO】[{className}] {message}");

    /// <summary>
    /// 警告ログを出力する
    /// </summary>
    /// <param name="className">クラス名</param>
    /// <param name="message">メッセージ</param>
    public static void Warn(string className, string message)
        => _logger.Warn($"【WARN】[{className} {message}");

    /// <summary>
    /// エラーログを出力する
    /// </summary>
    /// <param name="className">クラス名</param>
    /// <param name="message">メッセージ</param>
    public static void Error(string className, string message)
        => _logger.Error($"【ERROR】[{className}] {message}");
}
