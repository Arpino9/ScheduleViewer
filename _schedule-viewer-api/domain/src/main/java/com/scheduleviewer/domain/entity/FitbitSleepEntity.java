package com.scheduleviewer.domain.entity;

import java.time.Duration;
import java.time.LocalDateTime;

/**
 * Entity - Fitbit (睡眠データ)
 */
public final class FitbitSleepEntity {

    private final LocalDateTime startTime;
    private final LocalDateTime endTime;
    private final Duration sleeping;
    private final Duration awake;
    private final Duration restless;
    private final Duration rem;
    private final Duration asleep;

    public FitbitSleepEntity(
            LocalDateTime startTime,
            LocalDateTime endTime,
            Duration awake,
            Duration restless,
            Duration rem,
            Duration asleep) {
        this.startTime = startTime;
        this.endTime   = endTime;
        this.sleeping  = Duration.between(startTime, endTime).minus(awake);
        this.awake     = awake;
        this.restless  = restless;
        this.rem       = rem;
        this.asleep    = asleep;
    }

    /** 就寝時刻 */
    public LocalDateTime getStartTime() { return startTime; }

    /** 起床時刻 */
    public LocalDateTime getEndTime() { return endTime; }

    /** 睡眠時間 */
    public Duration getSleeping() { return sleeping; }

    /** 覚醒状態 */
    public Duration getAwake() { return awake; }

    /** 寝付けない */
    public Duration getRestless() { return restless; }

    /** レム睡眠 */
    public Duration getRem() { return rem; }

    /** 睡眠中 */
    public Duration getAsleep() { return asleep; }

    @Override
    public String toString() {
        long totalHours = sleeping.toHours();
        long minutes = sleeping.toMinutesPart();
        return totalHours == 0 ? "データなし" : totalHours + "時間" + minutes + "分";
    }
}
