﻿namespace ScheduleViewer.Domain.Entities;

/// <summary>
/// Entity - 勤怠表
/// </summary>
/// <remarks>
/// XAMLから初期化時に呼び出されるので、ここだけinitアクセサをつける。
/// </remarks>
public sealed class WorkScheduleEntity
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="date">日</param>
    /// <param name="background">背景色</param>
    /// <param name="notification">届出</param>
    /// <remarks>
    /// 旧祝日用
    /// </remarks>
    public WorkScheduleEntity(
        string date,
        SolidColorBrush background,
        string notification) : this(date,
                                     background,
                                     string.Empty,
                                     string.Empty,
                                     string.Empty,
                                     notification,
                                     string.Empty,
                                     string.Empty,
                                     string.Empty,
                                     string.Empty,
                                     string.Empty)
    {

    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="date">日</param>
    /// <param name="background">背景色</param>
    /// <param name="startTime">始業時間</param>
    /// <param name="endTime">終業時間</param>
    /// <param name="lunchTime">昼休憩時間</param>
    /// <param name="notification">届出</param>
    /// <param name="workingTime">勤務時間</param>
    /// <param name="overtime">残業時間</param>
    /// <param name="midnightTime">深夜時間</param>
    /// <param name="absentedTime">欠課時間</param>
    /// <param name="remarks">備考</param>
    public WorkScheduleEntity(
        string date,
        SolidColorBrush background,
        string startTime,
        string endTime,
        string lunchTime,
        string notification,
        string workingTime,
        string overtime,
        string midnightTime,
        string absentedTime,
        string remarks)
    {
        this.Day = date;
        this.Background = background;
        this.StartTime = startTime;
        this.EndTime = endTime;
        this.LunchTime = lunchTime;
        this.Notification = notification;
        this.WorkingTime = workingTime;
        this.Overtime = overtime;
        this.MidnightTime = midnightTime;
        this.AbsentedTime = absentedTime;
        this.Remarks = remarks;
    }

    /// <summary> 日 </summary>
    public string Day { get; init; }

    /// <summary> 背景色 </summary>
    public SolidColorBrush Background { get; init; }

    /// <summary> 始業時間 </summary>
    public string StartTime { get; init; }

    /// <summary> 終業時間 </summary>
    public string EndTime { get; init; }

    /// <summary> 昼休憩時間 </summary>
    public string LunchTime { get; init; }

    /// <summary> 届出 </summary>
    public string Notification { get; init; }

    /// <summary> 勤務時間 </summary>
    public string WorkingTime { get; init; }

    /// <summary> 残業時間 </summary>
    public string Overtime { get; init; }

    /// <summary> 深夜時間 </summary>
    public string MidnightTime { get; init; }

    /// <summary> 欠課時間 </summary>
    public string AbsentedTime { get; init; }

    /// <summary> 備考</summary>
    public string Remarks { get; init; }
}