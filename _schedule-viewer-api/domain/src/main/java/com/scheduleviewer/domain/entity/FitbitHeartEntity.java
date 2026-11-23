package com.scheduleviewer.domain.entity;

/**
 * Entity - Fitbit (心拍数)
 */
public final class FitbitHeartEntity {

    private final double restingHeartRate;

    public FitbitHeartEntity(double restingHeartRate) {
        this.restingHeartRate = restingHeartRate;
    }

    /** 安静時心拍数 */
    public double getRestingHeartRate() { return restingHeartRate; }
}
