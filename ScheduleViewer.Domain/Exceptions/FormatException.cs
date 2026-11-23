namespace ScheduleViewer.Domain.Exceptions;

/// <summary>
/// データ形式が不正な場合のエラーを表します。
/// </summary>
public sealed class FormatException : ExceptionBase
{
    /// <summary>フォーマット例外を初期化します。</summary>
    public FormatException(string message)
        : base(message, nameof(FormatException), LogType.Error)
    {
    }

    /// <summary>原因となった例外を指定してフォーマット例外を初期化します。</summary>
    public FormatException(string message, Exception ex, LogType logType = LogType.Error)
        : base(message, nameof(FormatException), logType, ex)
    {
    }
}
