namespace ScheduleViewer.Domain.Exceptions;

/// <summary>
/// ユーザ定義例外 - データベース接続
/// </summary>
public sealed class DatabaseException : ExceptionBase
{
    public DatabaseException(string message) :
        base(message, MethodBase.GetCurrentMethod().DeclaringType.Name, LogType.Error)
    {

    }

    public DatabaseException(string message, Exception ex, LogType logType = LogType.Error) :
       base(message, ex, logType)
    {

    }
}