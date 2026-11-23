namespace ScheduleViewer.Domain.Exceptions;

/// <summary>
/// アプリケーション固有例外の基底クラスです。
/// </summary>
public abstract class ExceptionBase : Exception
{
    private static readonly log4net.ILog Logger =
        log4net.LogManager.GetLogger(typeof(ExceptionBase));

    /// <summary>
    /// 例外を初期化します。
    /// </summary>
    /// <param name="message">例外メッセージ。</param>
    /// <param name="title">ユーザーへ通知する際のタイトル。</param>
    /// <param name="logType">ログの重要度。</param>
    /// <param name="innerException">この例外の原因となった例外。</param>
    protected ExceptionBase(
        string message,
        string title,
        LogType logType,
        Exception? innerException = null)
        : base(message, innerException)
    {
        Title = title;
        Severity = logType;
        WriteLog(message, innerException, logType);
    }

    /// <summary>
    /// ユーザーへ通知する際のタイトルを取得します。
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// ログの重要度を取得します。
    /// </summary>
    public LogType Severity { get; }

    [System.Diagnostics.Conditional("DEBUG")]
    private static void WriteLog(string message, Exception? exception, LogType logType)
    {
        switch (logType)
        {
            case LogType.Error:
                if (exception is null)
                {
                    Logger.Error(message);
                }
                else
                {
                    Logger.Error(message, exception);
                }

                break;

            case LogType.Fatal:
                if (exception is null)
                {
                    Logger.Fatal(message);
                }
                else
                {
                    Logger.Fatal(message, exception);
                }

                break;
        }
    }

    /// <summary>
    /// ログの重要度を表します。
    /// </summary>
    public enum LogType
    {
        /// <summary>通常のエラー。</summary>
        Error,

        /// <summary>システムの信頼性を脅かす重大なエラー。</summary>
        Fatal
    }
}
