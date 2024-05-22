namespace ScheduleViewer.Domain.Exceptions;

/// <summary>
/// ユーザ定義例外 - ファイル読み込み
/// </summary>
public sealed class FileReaderException : ExceptionBase
{
    public FileReaderException(string message) :
        base(message, MethodBase.GetCurrentMethod().DeclaringType.Name, LogType.Error)
    {

    }

    public FileReaderException(string message, Exception ex, LogType logType = LogType.Error) :
       base(message, ex, logType)
    {

    }
}
