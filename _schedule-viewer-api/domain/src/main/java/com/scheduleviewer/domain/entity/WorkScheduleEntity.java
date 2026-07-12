package com.scheduleviewer.domain.entity;

/**
 * Entity - 勤怠表
 * <p>注: .NET版のSolidColorBrushはWPF固有のため、CSS互換の16進カラーコード文字列に置き換え</p>
 */
public final class WorkScheduleEntity {

    private final String day;
    private final String backgroundColor;
    private final String startTime;
    private final String endTime;
    private final String lunchTime;
    private final String notification;
    private final String workingTime;
    private final String overtime;
    private final String midnightTime;
    private final String absentedTime;
    private final String remarks;

    /** 祝日用コンストラクタ */
    public WorkScheduleEntity(String date, String backgroundColor, String notification) {
        this(date, backgroundColor, "", "", "", notification, "", "", "", "", "");
    }

    public WorkScheduleEntity(
            String date,
            String backgroundColor,
            String startTime,
            String endTime,
            String lunchTime,
            String notification,
            String workingTime,
            String overtime,
            String midnightTime,
            String absentedTime,
            String remarks) {
        this.day             = date;
        this.backgroundColor = backgroundColor;
        this.startTime       = startTime;
        this.endTime         = endTime;
        this.lunchTime       = lunchTime;
        this.notification    = notification;
        this.workingTime     = workingTime;
        this.overtime        = overtime;
        this.midnightTime    = midnightTime;
        this.absentedTime    = absentedTime;
        this.remarks         = remarks;
    }

    /** 日 */
    public String getDay() { return day; }

    /** 背景色 (例: "#FFFFFF") */
    public String getBackgroundColor() { return backgroundColor; }

    /** 始業時間 */
    public String getStartTime() { return startTime; }

    /** 終業時間 */
    public String getEndTime() { return endTime; }

    /** 昼休憩時間 */
    public String getLunchTime() { return lunchTime; }

    /** 届出 */
    public String getNotification() { return notification; }

    /** 勤務時間 */
    public String getWorkingTime() { return workingTime; }

    /** 残業時間 */
    public String getOvertime() { return overtime; }

    /** 深夜時間 */
    public String getMidnightTime() { return midnightTime; }

    /** 欠課時間 */
    public String getAbsentedTime() { return absentedTime; }

    /** 備考 */
    public String getRemarks() { return remarks; }
}
