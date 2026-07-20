package com.scheduleviewer.domain.valueobject;

/**
 * Value Object - 二者択一
 */
public record AlternativeValue(boolean value) {

    /** ○ */
    public static final AlternativeValue VALID = new AlternativeValue(true);

    /** × */
    public static final AlternativeValue INVALID = new AlternativeValue(false);

    /** 表示用テキスト */
    public String text() {
        return value ? "○" : "×";
    }
}
