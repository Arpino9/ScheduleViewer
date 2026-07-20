package com.scheduleviewer.domain.entity;

import java.time.LocalDateTime;

/**
 * Entity - 家計簿
 * <p>注: .NET版はString[]から直接パースしていたが、Javaでは各フィールドを明示的に受け取る。
 * パース処理はInfrastructure層(SpreadSheet読み込み側)に移譲する。</p>
 */
public final class ExpenditureEntity {

    private final String id;
    private final String canCalc;
    private final LocalDateTime date;
    private final String itemName;
    private final long price;
    private final String financialInstitutions;
    private final String categoryLarge;
    private final String categoryMiddle;
    private final String memo;
    private final String change;

    public ExpenditureEntity(
            String id,
            boolean canCalc,
            LocalDateTime date,
            String itemName,
            long price,
            String financialInstitutions,
            String categoryLarge,
            String categoryMiddle,
            String memo,
            boolean change) {
        this.id                     = id;
        this.canCalc                = canCalc ? "はい" : "いいえ";
        this.date                   = date;
        this.itemName               = itemName;
        this.price                  = price;
        this.financialInstitutions  = financialInstitutions;
        this.categoryLarge          = categoryLarge;
        this.categoryMiddle         = categoryMiddle;
        this.memo                   = memo;
        this.change                 = change ? "はい" : "いいえ";
    }

    /** ID */
    public String getId() { return id; }

    /** 計算対象 */
    public String getCanCalc() { return canCalc; }

    /** 日付 */
    public LocalDateTime getDate() { return date; }

    /** 内容 */
    public String getItemName() { return itemName; }

    /** 金額(円) */
    public long getPrice() { return price; }

    /** 保有金融機関 */
    public String getFinancialInstitutions() { return financialInstitutions; }

    /** 大項目 */
    public String getCategoryLarge() { return categoryLarge; }

    /** 中項目 */
    public String getCategoryMiddle() { return categoryMiddle; }

    /** メモ */
    public String getMemo() { return memo; }

    /** 振替 */
    public String getChange() { return change; }
}
