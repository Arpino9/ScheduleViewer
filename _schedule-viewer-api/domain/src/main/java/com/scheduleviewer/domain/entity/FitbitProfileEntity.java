package com.scheduleviewer.domain.entity;

import java.time.LocalDateTime;
import java.util.List;

/**
 * Entity - Fitbit (プロフィール)
 */
public final class FitbitProfileEntity {

    private final String name;
    private final int age;
    private final String gender;
    private final double height;
    private final double weight;
    private final List<EarnedBadge> badges;

    public FitbitProfileEntity(
            String name,
            int age,
            String gender,
            double height,
            double weight,
            List<EarnedBadge> badges) {
        this.name   = name;
        this.age    = age;
        this.gender = gender;
        this.height = height;
        this.weight = weight;
        this.badges = List.copyOf(badges);
    }

    /** 名前 */
    public String getName() { return name; }

    /** 年齢 */
    public int getAge() { return age; }

    /** 性別 */
    public String getGender() { return gender; }

    /** 身長 */
    public double getHeight() { return height; }

    /** 体重 */
    public double getWeight() { return weight; }

    /** 獲得したバッジ */
    public List<EarnedBadge> getBadges() { return badges; }

    /**
     * Entity - Fitbit (獲得バッジ)
     */
    public record EarnedBadge(
            String name,
            LocalDateTime earnedDate,
            String description) {
    }
}
