package com.scheduleviewer.domain.entity;

import java.time.LocalDate;

/**
 * Entity - 自宅
 */
public final class HomeEntity {

    private final int id;
    private final String displayName;
    private final LocalDate livingStart;
    private LocalDate livingEnd;
    private final boolean isLiving;
    private final String postCode;
    private final String address;
    private final String addressGoogle;
    private final String remarks;

    public HomeEntity(
            int id,
            String displayName,
            LocalDate livingStart,
            LocalDate livingEnd,
            boolean isLiving,
            String postCode,
            String address,
            String addressGoogle,
            String remarks) {
        this.id            = id;
        this.displayName   = displayName;
        this.livingStart   = livingStart;
        this.livingEnd     = livingEnd;
        this.isLiving      = isLiving;
        this.postCode      = postCode;
        this.address       = address;
        this.addressGoogle = addressGoogle;
        this.remarks       = remarks;
    }

    /** ID */
    public int getId() { return id; }

    /** 名称 */
    public String getDisplayName() { return displayName; }

    /** 郵便番号 */
    public String getPostCode() { return postCode; }

    /** 在住開始日 */
    public LocalDate getLivingStart() { return livingStart; }

    /**
     * 在住終了日
     * <p>在住中の場合は本日の日付を返す</p>
     */
    public LocalDate getLivingEnd() {
        return isLiving ? LocalDate.now() : livingEnd;
    }

    public void setLivingEnd(LocalDate livingEnd) { this.livingEnd = livingEnd; }

    /** 在住中か */
    public boolean isLiving() { return isLiving; }

    /** 住所 */
    public String getAddress() { return address; }

    /** 住所 (Google Maps用) */
    public String getAddressGoogle() { return addressGoogle; }

    /** 備考 */
    public String getRemarks() { return remarks; }
}
