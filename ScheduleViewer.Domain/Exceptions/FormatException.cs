namespace ScheduleViewer.Domain.Exceptions;

/// <summary>
/// ユーザ定義例外 - フォーマットエラー
/// </summary>
public sealed class FormatException : ExceptionBase
{
    public FormatException(string message) :
        base(message, MethodBase.GetCurrentMethod().DeclaringType.Name, LogType.Error)
    {

    }

    public FormatException(string message, Exception ex, LogType logType = LogType.Error) :
       base(message, ex, logType)
    {

    }
}
