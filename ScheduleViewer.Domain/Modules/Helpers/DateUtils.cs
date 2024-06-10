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
    public static string ToSQLiteDate(this DateTime date)
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

    /// <summary>
    /// 日付をナノ秒に変換
    /// </summary>
    /// <param name="date">日付</param>
    /// <returns>ナノ秒</returns>
    public static long ToNanos(this DateTimeOffset date)
        => date.ToUnixTimeMilliseconds() * 1000000;

    /// <summary>
    /// 日付(開始日)をナノ秒に変換
    /// </summary>
    /// <param name="date">日付</param>
    /// <returns>ナノ秒</returns>
    public static long ToNanos_StartDate(this DateTime date)
        => ToNanos(new DateTimeOffset(date.Date));

    /// <summary>
    /// 日付(終了日)をナノ秒に変換
    /// </summary>
    /// <param name="date">日付</param>
    /// <returns>ナノ秒</returns>
    public static long ToNanos_EndDate(this DateTime date)
        => ToNanos(date.Date.AddDays(1).AddMilliseconds(-1));

    /// <summary>
    /// DatetimeOffsetに変換
    /// </summary>
    /// <param name="date">日付</param>
    /// <returns>DatetimeOffset値</returns>
    public static DateTimeOffset ToOffset(this DateTime date)
        => (DateTimeOffset)DateTime.SpecifyKind(date, DateTimeKind.Utc);
}
