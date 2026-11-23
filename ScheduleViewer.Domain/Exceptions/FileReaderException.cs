namespace ScheduleViewer.Domain.Exceptions;

/// <summary>
/// ファイル読み込みで発生したエラーを表します。
/// </summary>
public sealed class FileReaderException : ExceptionBase
{
    /// <summary>ファイル読み込み例外を初期化します。</summary>
    public FileReaderException(string message)
        : base(message, nameof(FileReaderException), LogType.Error)
    {
    }

    /// <summary>原因となった例外を指定してファイル読み込み例外を初期化します。</summary>
    public FileReaderException(string message, Exception ex, LogType logType = LogType.Error)
        : base(message, nameof(FileReaderException), logType, ex)
    {
    }
}
