namespace ScheduleViewer.Domain.Exceptions;

/// <summary>
/// データベース処理で発生したエラーを表します。
/// </summary>
public sealed class DatabaseException : ExceptionBase
{
    /// <summary>データベース例外を初期化します。</summary>
    public DatabaseException(string message)
        : base(message, nameof(DatabaseException), LogType.Error)
    {
    }

    /// <summary>原因となった例外を指定してデータベース例外を初期化します。</summary>
    public DatabaseException(string message, Exception ex, LogType logType = LogType.Error)
        : base(message, nameof(DatabaseException), logType, ex)
    {
    }
}
