namespace ScheduleViewer.WPF.Models.Presentation;

/// <summary>
/// 勤怠表の1日分を表示・編集するための項目です。
/// </summary>
/// <remarks>
/// 既存のXAMLバインディングとの互換性を保つため、各プロパティにinitアクセサーを設けています。
/// </remarks>
public sealed class WorkScheduleItem
{
    /// <summary>
    /// 勤怠表の1日分を表示・編集するための項目を初期化します。
    /// </summary>
    /// <param name="day">表示する日。</param>
    /// <param name="background">背景色。</param>
    /// <param name="startTime">始業時間。</param>
    /// <param name="endTime">終業時間。</param>
    /// <param name="lunchTime">昼休憩時間。</param>
    /// <param name="notification">届出。</param>
    /// <param name="workingTime">勤務時間。</param>
    /// <param name="overtime">残業時間。</param>
    /// <param name="midnightTime">深夜時間。</param>
    /// <param name="absentedTime">欠課時間。</param>
    /// <param name="remarks">備考。</param>
    public WorkScheduleItem(
        string day,
        SolidColorBrush background,
        string startTime = "",
        string endTime = "",
        string lunchTime = "",
        string notification = "",
        string workingTime = "",
        string overtime = "",
        string midnightTime = "",
        string absentedTime = "",
        string remarks = "")
    {
        Day = day;
        Background = background;
        StartTime = startTime;
        EndTime = endTime;
        LunchTime = lunchTime;
        Notification = notification;
        WorkingTime = workingTime;
        Overtime = overtime;
        MidnightTime = midnightTime;
        AbsentedTime = absentedTime;
        Remarks = remarks;
    }

    /// <summary>表示する日を取得します。</summary>
    public string Day { get; init; }

    /// <summary>背景色を取得します。</summary>
    public SolidColorBrush Background { get; init; }

    /// <summary>始業時間を取得します。</summary>
    public string StartTime { get; init; }

    /// <summary>終業時間を取得します。</summary>
    public string EndTime { get; init; }

    /// <summary>昼休憩時間を取得します。</summary>
    public string LunchTime { get; init; }

    /// <summary>届出を取得します。</summary>
    public string Notification { get; init; }

    /// <summary>勤務時間を取得します。</summary>
    public string WorkingTime { get; init; }

    /// <summary>残業時間を取得します。</summary>
    public string Overtime { get; init; }

    /// <summary>深夜時間を取得します。</summary>
    public string MidnightTime { get; init; }

    /// <summary>欠課時間を取得します。</summary>
    public string AbsentedTime { get; init; }

    /// <summary>備考を取得します。</summary>
    public string Remarks { get; init; }
}
