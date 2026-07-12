package com.scheduleviewer.domain.entity;

/**
 * Entity - Fitbit (アクティビティ)
 */
public final class FitbitActivityEntity {

    private final double steps;
    private final double caloriesOut;
    private final double elevation;
    private final double distance;

    public FitbitActivityEntity(double steps, double caloriesOut, double elevation, double distance) {
        this.steps       = steps;
        this.caloriesOut = caloriesOut;
        this.elevation   = elevation;
        this.distance    = distance;
    }

    /** 歩数 */
    public double getSteps() { return steps; }

    /** 消費エネルギー */
    public double getCaloriesOut() { return caloriesOut; }

    /** 階数 */
    public double getElevation() { return elevation; }

    /** 距離 */
    public double getDistance() { return distance; }
}
