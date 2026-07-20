package com.scheduleviewer.domain.entity;

import com.scheduleviewer.domain.valueobject.CompanyNameValue;

import java.time.Duration;
import java.time.LocalDate;

/**
 * Entity - 就業場所
 */
public final class WorkingPlaceEntity {

    /**
     * 時間範囲 (始業〜終業 など)
     */
    public record TimeRange(Duration start, Duration end) {}

    private final int id;
    private final CompanyNameValue dispatchingCompany;
    private final CompanyNameValue dispatchedCompany;
    private final CompanyNameValue workingPlaceName;
    private final String workingPlaceAddress;
    private final LocalDate workingStart;
    private LocalDate workingEnd;
    private final boolean isWaiting;
    private final boolean isWorking;
    private final TimeRange workingTime;
    private final TimeRange lunchTime;
    private final TimeRange breakTime;
    private final String remarks;

    public WorkingPlaceEntity(
            int id,
            String dispatchingCompany,
            String dispatchedCompany,
            String workingPlace,
            String workingAddress,
            LocalDate workingStart,
            LocalDate workingEnd,
            boolean isWaiting,
            boolean isWorking,
            int workingStartHour, int workingStartMinute,
            int workingEndHour,   int workingEndMinute,
            int lunchStartHour,   int lunchStartMinute,
            int lunchEndHour,     int lunchEndMinute,
            int breakStartHour,   int breakStartMinute,
            int breakEndHour,     int breakEndMinute,
            String remarks) {
        this.id                   = id;
        this.dispatchingCompany   = new CompanyNameValue(dispatchingCompany);
        this.dispatchedCompany    = new CompanyNameValue(dispatchedCompany);
        this.workingPlaceName     = new CompanyNameValue(workingPlace);
        this.workingPlaceAddress  = workingAddress;
        this.workingStart         = workingStart;
        this.workingEnd           = workingEnd;
        this.isWaiting            = isWaiting;
        this.isWorking            = isWorking;
        this.workingTime = new TimeRange(
                Duration.ofHours(workingStartHour).plusMinutes(workingStartMinute),
                Duration.ofHours(workingEndHour).plusMinutes(workingEndMinute));
        this.lunchTime = new TimeRange(
                Duration.ofHours(lunchStartHour).plusMinutes(lunchStartMinute),
                Duration.ofHours(lunchEndHour).plusMinutes(lunchEndMinute));
        this.breakTime = new TimeRange(
                Duration.ofHours(breakStartHour).plusMinutes(breakStartMinute),
                Duration.ofHours(breakEndHour).plusMinutes(breakEndMinute));
        this.remarks = remarks;
    }

    /** ID */
    public int getId() { return id; }

    /** 派遣元会社 */
    public CompanyNameValue getDispatchingCompany() { return dispatchingCompany; }

    /** 派遣先会社 */
    public CompanyNameValue getDispatchedCompany() { return dispatchedCompany; }

    /** 就業先(名称) */
    public CompanyNameValue getWorkingPlaceName() { return workingPlaceName; }

    /** 就業先(住所) */
    public String getWorkingPlaceAddress() { return workingPlaceAddress; }

    /** 勤務開始 */
    public LocalDate getWorkingStart() { return workingStart; }

    /**
     * 勤務終了
     * <p>就業中の場合は本日の日付を返す</p>
     */
    public LocalDate getWorkingEnd() {
        return isWorking ? LocalDate.now() : workingEnd;
    }

    public void setWorkingEnd(LocalDate workingEnd) { this.workingEnd = workingEnd; }

    /** 待機中か */
    public boolean isWaiting() { return isWaiting; }

    /** 就業中か */
    public boolean isWorking() { return isWorking; }

    /** 労働時間 (始業時刻, 終業時刻) */
    public TimeRange getWorkingTime() { return workingTime; }

    /** 昼休憩 (開始時刻, 終了時刻) */
    public TimeRange getLunchTime() { return lunchTime; }

    /** 休憩 (開始時刻, 終了時刻) */
    public TimeRange getBreakTime() { return breakTime; }

    /** 備考 */
    public String getRemarks() { return remarks; }

    /** 名目労働時間 */
    public Duration nominalWorkTimeSpan() {
        return workingTime.end().minus(workingTime.start());
    }

    /** 実働労働時間 */
    public Duration actualWorkTimeSpan() {
        return nominalWorkTimeSpan().minus(lunchTimeSpan());
    }

    /** 昼休憩時間 */
    public Duration lunchTimeSpan() {
        return lunchTime.end().minus(lunchTime.start());
    }
}
