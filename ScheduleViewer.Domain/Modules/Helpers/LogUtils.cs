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
    /// <param name="message">メッセージ</param>
    public static void Debug(string message)
        => _logger.Debug(message);

    /// <summary>
    /// インフォメーションログを出力する
    /// </summary>
    /// <param name="message">メッセージ</param>
    public static void Info(string message)
        => _logger.Info(message);

    /// <summary>
    /// 警告ログを出力する
    /// </summary>
    /// <param name="message">メッセージ</param>
    public static void Warn(string message)
        => _logger.Warn(message);

    /// <summary>
    /// エラーログを出力する
    /// </summary>
    /// <param name="message">メッセージ</param>
    public static void Error(string message)
        => _logger.Error(message);
}
