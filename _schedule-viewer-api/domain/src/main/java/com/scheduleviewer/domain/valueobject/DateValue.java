package com.scheduleviewer.domain.valueobject;

import java.time.LocalDate;
import java.time.chrono.JapaneseDate;
import java.time.format.DateTimeFormatter;
import java.util.Locale;

/**
 * Value Object - 年表示
 * <p>西暦と和暦の変換用</p>
 */
public record DateValue(LocalDate value) {

    public DateValue {
        if (value.getYear() < 1970) {
            throw new IllegalArgumentException("日付書式が不正です。");
        }
    }

    public DateValue(int year, int month) {
        this(LocalDate.of(year, month, 1));
    }

    /** 年テキスト (例: 2023年) */
    public String text() {
        return value.getYear() + "年";
    }

    /**
     * 和暦 (例: 令和5年)
     */
    public String japaneseCalendar() {
        JapaneseDate japaneseDate = JapaneseDate.from(value);
        DateTimeFormatter formatter = DateTimeFormatter.ofPattern("GGGGy年", new Locale("ja", "JP"));
        return formatter.format(japaneseDate);
    }

    /** 西暦 + 和暦 (例: 2023年(令和5年)) */
    public String yearWithJapaneseCalendar() {
        return text() + " (" + japaneseCalendar() + ")";
    }

    /** 長い曜日名 (例: 月曜日) */
    public String weekLongName() {
        return value.getDayOfWeek().getDisplayName(java.time.format.TextStyle.FULL, Locale.JAPANESE);
    }

    /** 短い曜日名 (例: 月) */
    public String weekShortName() {
        return value.getDayOfWeek().getDisplayName(java.time.format.TextStyle.SHORT, Locale.JAPANESE);
    }

    /** 土曜日か */
    public boolean isSaturday() {
        return value.getDayOfWeek() == java.time.DayOfWeek.SATURDAY;
    }

    /** 日曜日か */
    public boolean isSunday() {
        return value.getDayOfWeek() == java.time.DayOfWeek.SUNDAY;
    }

    /** 週末か */
    public boolean isWeekend() {
        return isSaturday() || isSunday();
    }

    /** 日付文字列 yyyy/M/d(曜) */
    public String dateYYYYMMDDWithWeekName() {
        return String.format("%d/%d/%d(%s)", value.getYear(), value.getMonthValue(), value.getDayOfMonth(), weekShortName());
    }

    /** 日付文字列 M/d(曜) */
    public String dateMMDDWithWeekName() {
        return String.format("%d/%d(%s)", value.getMonthValue(), value.getDayOfMonth(), weekShortName());
    }

    /** 月初日付 */
    public LocalDate firstDateOfMonth() {
        return value.withDayOfMonth(1);
    }

    /** 月末日付 */
    public LocalDate lastDateOfMonth() {
        return value.withDayOfMonth(value.lengthOfMonth());
    }

    /** 月末日 */
    public int lastDayOfMonth() {
        return value.lengthOfMonth();
    }
}
