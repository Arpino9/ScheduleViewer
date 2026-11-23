package com.scheduleviewer.domain.valueobject;

/**
 * Value Object - 会社名
 */
public record CompanyNameValue(String text) {

    /** 未登録 */
    public static final CompanyNameValue UNDEFINED = new CompanyNameValue("");

    /** 株式会社か */
    public boolean isInc() {
        return text.contains("株式会社") || text.contains("(株)") || text.contains("（株）");
    }

    /** 有限会社か */
    public boolean isLimited() {
        return text.contains("有限会社") || text.contains("(有)") || text.contains("（有）");
    }

    /** 表示用 */
    public String displayValue() {
        return this.equals(UNDEFINED) ? "<未登録>" : text;
    }
}
