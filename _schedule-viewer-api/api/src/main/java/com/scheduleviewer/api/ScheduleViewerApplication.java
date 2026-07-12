package com.scheduleviewer.api;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.boot.context.properties.EnableConfigurationProperties;

/**
 * ScheduleViewer Spring Boot アプリケーション
 */
@SpringBootApplication(scanBasePackages = "com.scheduleviewer")
public class ScheduleViewerApplication {

    public static void main(String[] args) {
        SpringApplication.run(ScheduleViewerApplication.class, args);
    }
}
