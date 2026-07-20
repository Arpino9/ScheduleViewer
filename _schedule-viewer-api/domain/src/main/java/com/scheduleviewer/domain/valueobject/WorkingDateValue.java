package com.scheduleviewer.domain.valueobject;

import java.time.LocalDate;

/**
 * Value Object - 勤務日
 */
public record WorkingDateValue(LocalDate value) {

    /** 不明 */
    public static final WorkingDateValue UNKNOWN = new WorkingDateValue(LocalDate.MIN);

    /** 就業中 */
    public static final WorkingDateValue WORKING = new WorkingDateValue(LocalDate.MAX);

    /** 不明か */
    public boolean isUnknown() {
        return value.equals(LocalDate.MIN);
    }

    /** 就業中か */
    public boolean isWorking() {
        return value.equals(LocalDate.MAX);
    }

    @Override
    public String toString() {
        return isWorking() ? "就業中" : value.toString();
    }
}
