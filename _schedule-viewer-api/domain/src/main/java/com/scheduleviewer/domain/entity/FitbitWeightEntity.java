package com.scheduleviewer.domain.entity;

/**
 * Entity - Fitbit (体重)
 */
public final class FitbitWeightEntity {

    private final double bmi;
    private final double weight;

    public FitbitWeightEntity(double bmi, double weight) {
        this.bmi    = bmi;
        this.weight = weight;
    }

    /** BMI */
    public double getBmi() { return bmi; }

    /** 体重 */
    public double getWeight() { return weight; }
}
