package com.scheduleviewer.domain.entity;

import com.scheduleviewer.domain.valueobject.AlternativeValue;

/**
 * Entity - 手当有無
 */
public class AllowanceExistenceEntity {

    private final AlternativeValue perfectAttendance;
    private final AlternativeValue education;
    private final AlternativeValue electricity;
    private final AlternativeValue certification;
    private final AlternativeValue overtime;
    private final AlternativeValue travel;
    private final AlternativeValue housing;
    private final AlternativeValue food;
    private final AlternativeValue lateNight;
    private final AlternativeValue area;
    private final AlternativeValue commution;
    private final AlternativeValue prepaidRetirement;
    private final AlternativeValue dependency;
    private final AlternativeValue executive;
    private final AlternativeValue special;

    public AllowanceExistenceEntity(
            boolean perfectAttendance,
            boolean education,
            boolean electricity,
            boolean certification,
            boolean overtime,
            boolean travel,
            boolean housing,
            boolean food,
            boolean lateNight,
            boolean area,
            boolean commution,
            boolean prepaidRetirement,
            boolean dependency,
            boolean executive,
            boolean special) {
        this.perfectAttendance  = new AlternativeValue(perfectAttendance);
        this.education          = new AlternativeValue(education);
        this.electricity        = new AlternativeValue(electricity);
        this.certification      = new AlternativeValue(certification);
        this.overtime           = new AlternativeValue(overtime);
        this.travel             = new AlternativeValue(travel);
        this.housing            = new AlternativeValue(housing);
        this.food               = new AlternativeValue(food);
        this.lateNight          = new AlternativeValue(lateNight);
        this.area               = new AlternativeValue(area);
        this.commution          = new AlternativeValue(commution);
        this.prepaidRetirement  = new AlternativeValue(prepaidRetirement);
        this.dependency         = new AlternativeValue(dependency);
        this.executive          = new AlternativeValue(executive);
        this.special            = new AlternativeValue(special);
    }

    /** 皆勤手当 */
    public AlternativeValue getPerfectAttendance() { return perfectAttendance; }

    /** 教育手当 */
    public AlternativeValue getEducation() { return education; }

    /** 在宅手当 */
    public AlternativeValue getElectricity() { return electricity; }

    /** 資格手当 */
    public AlternativeValue getCertification() { return certification; }

    /** 時間外手当 */
    public AlternativeValue getOvertime() { return overtime; }

    /** 出張手当 */
    public AlternativeValue getTravel() { return travel; }

    /** 住宅手当 */
    public AlternativeValue getHousing() { return housing; }

    /** 食事手当 */
    public AlternativeValue getFood() { return food; }

    /** 深夜手当 */
    public AlternativeValue getLateNight() { return lateNight; }

    /** 地域手当 */
    public AlternativeValue getArea() { return area; }

    /** 通勤手当 */
    public AlternativeValue getCommution() { return commution; }

    /** 前払退職金 */
    public AlternativeValue getPrepaidRetirement() { return prepaidRetirement; }

    /** 扶養手当 */
    public AlternativeValue getDependency() { return dependency; }

    /** 役職手当 */
    public AlternativeValue getExecutive() { return executive; }

    /** 特別手当 */
    public AlternativeValue getSpecial() { return special; }
}
