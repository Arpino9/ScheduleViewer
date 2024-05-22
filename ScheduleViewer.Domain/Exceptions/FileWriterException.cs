namespace ScheduleViewer.Domain.Exceptions;

/// <summary>
/// ユーザ定義例外 - ファイル書き込み
/// </summary>
public sealed class FileWriterException : ExceptionBase
{
    public FileWriterException(string message) :
        base(message, MethodBase.GetCurrentMethod().DeclaringType.Name, LogType.Error)
    {

    }

    public FileWriterException(string message, Exception ex, LogType logType = LogType.Error) :
       base(message, ex, logType)
    {

    }
}
