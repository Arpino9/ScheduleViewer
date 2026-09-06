namespace ScheduleViewer.Domain.Exceptions;

/// <summary>
/// ファイル書き込みで発生したエラーを表します。
/// </summary>
public sealed class FileWriterException : ExceptionBase
{
    /// <summary>ファイル書き込み例外を初期化します。</summary>
    public FileWriterException(string message)
        : base(message, nameof(FileWriterException), LogType.Error)
    {
    }

    /// <summary>原因となった例外を指定してファイル書き込み例外を初期化します。</summary>
    public FileWriterException(string message, Exception ex, LogType logType = LogType.Error)
        : base(message, nameof(FileWriterException), logType, ex)
    {
    }
}
