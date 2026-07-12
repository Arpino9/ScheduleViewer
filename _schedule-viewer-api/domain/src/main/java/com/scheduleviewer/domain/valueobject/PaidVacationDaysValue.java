package com.scheduleviewer.domain.valueobject;

/**
 * Value Object - 有給日数
 */
public record PaidVacationDaysValue(double value) {

    /** 上限値 */
    public static final int MAXIMUM = 40;

    /** 下限値 */
    public static final int MINIMUM = 0;

    public PaidVacationDaysValue {
        if (value < MINIMUM) {
            throw new IllegalArgumentException("有給日数の下限値を下回っています。");
        }
        if (value > MAXIMUM) {
            throw new IllegalArgumentException("有給日数の上限値を超えています。");
        }
    }

    /** 表示テキスト */
    public String text() {
        return value + "日";
    }

    @Override
    public String toString() {
        return "有給日数：" + value + "日";
    }
}
