package com.scheduleviewer.api;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;

import java.time.ZoneId;
import java.util.TimeZone;

/**
 * ScheduleViewer Spring Boot アプリケーション
 */
@SpringBootApplication(scanBasePackages = "com.scheduleviewer")
public class ScheduleViewerApplication {

    private static final String DEFAULT_TIME_ZONE = "Asia/Tokyo";

    public static void main(String[] args) {
        String configuredTimeZone = System.getenv()
                .getOrDefault("APP_TIME_ZONE", DEFAULT_TIME_ZONE);
        TimeZone.setDefault(TimeZone.getTimeZone(ZoneId.of(configuredTimeZone)));

        SpringApplication.run(ScheduleViewerApplication.class, args);
    }
}
