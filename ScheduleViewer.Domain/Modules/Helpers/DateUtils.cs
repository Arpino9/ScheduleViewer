namespace ScheduleViewer.Domain.Modules.Helpers;

/// <summary>
/// Helpers - 日付関連
/// </summary>
public static class DateUtils
{
    /// <summary>
    /// SQLiteの値に変換
    /// </summary>
    /// <param name="dateTime">日付</param>
    /// <returns>日付</returns>
    public static string ConvertToSQLiteYearMonth(this DateTime dateTime)
        => dateTime.Year + "-" + dateTime.Month.ToString("D2") + "-" + "01";

    /// <summary>
    /// SQLiteの値に変換
    /// </summary>
    /// <param name="date">日付</param>
    /// <returns>SQLite日付</returns>
    public static string ConvertToSQLiteDate(this DateTime date)
       => date.Year + "-" + date.Month.ToString("D2") + "-" + date.Day.ToString("D2");

    public static DateTime ToDateTime(this string date)
    {
        var year  = int.Parse(date.Substring(0, 4));
        var month = int.Parse(date.Substring(5, 2));
        var day   = int.Parse(date.Substring(8, 2));
        
        var hour    = int.Parse(date.Substring(11, 2));
        var minute  = int.Parse(date.Substring(14, 2));
        var seconds = int.Parse(date.Substring(17, 2));

        return new DateTime(year, month, day, hour, minute, seconds);
    }
}
