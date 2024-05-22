namespace ScheduleViewer.Domain.Exceptions;

/// <summary>
/// ユーザ定義例外
/// </summary>
/// <remarks>
/// デバッグ時のみメッセージ出力する。メッセージ表示後にログ書き出しを行う。
/// </remarks>
public abstract class ExceptionBase : Exception
{
    private static readonly log4net.ILog _logger =
      log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="message">メッセージ</param>
    /// <param name="title">タイトル</param>
    /// <remarks>
    /// 通常の例外
    /// </remarks>
    public ExceptionBase(string message, string title) :
        base(message)
    {

#if DEBUG

        System.Windows.MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);

#endif

    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="message">メッセージ</param>
    /// <param name="logType">ログ区分</param>
    /// <remarks>
    /// ログ書き出しバージョン(簡略版)
    /// </remarks>
    public ExceptionBase(string message, LogType logType)
    {

#if DEBUG

        switch (logType)
        {
            case LogType.Error:
                _logger.Error(message); break;

            case LogType.Fatal:
                _logger.Fatal(message); break;
        }

#endif

    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="message">メッセージ</param>
    /// <param name="title">タイトル</param>
    /// <param name="logType">ログ区分</param>
    /// <remarks>
    /// メッセージ＆ログ書き出しバージョン(簡略版)
    /// </remarks>
    public ExceptionBase(string message, string title, LogType logType)
    {

#if DEBUG

        switch (logType)
        {
            case LogType.Error:
                System.Windows.MessageBox.Show(
                    message, title, MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.Error(message); break;

            case LogType.Fatal:
                System.Windows.MessageBox.Show(
                    message, title, MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.Fatal(message); break;
        }

#endif

    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="title">タイトル</param>
    /// <param name="ex">元になった例外</param>
    /// <remarks>
    /// 内部例外あり＆ログ書き出しバージョン
    /// </remarks>
    public ExceptionBase(string title, Exception ex, LogType logType)
        : base(title, ex)
    {

#if DEBUG

        switch (logType)
        {
            case LogType.Error:
                System.Windows.MessageBox.Show(
                    ex.ToString(), title, MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.Error(title, ex); break;

            case LogType.Fatal:
                System.Windows.MessageBox.Show(
                    ex.ToString(), title, MessageBoxButton.OK, MessageBoxImage.Error);
                _logger.Fatal(title, ex); break;
        }

#endif

    }

    /// <summary>
    /// ログ区分
    /// </summary>
    public enum LogType
    {
        /// <summary> エラー </summary>
        /// <remarks> SQL接続エラーなど </remarks>
        Error,

        /// <summary> ヤバいエラー </summary>
        /// <remarks> システムの信頼性を脅かすエラー </remarks>
        Fatal
    }
}