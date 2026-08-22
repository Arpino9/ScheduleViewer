package com.scheduleviewer.infrastructure.google.health;

import com.fasterxml.jackson.databind.ObjectMapper;
import org.junit.jupiter.api.Test;

import java.time.Duration;

import static org.junit.jupiter.api.Assertions.assertEquals;

class GoogleHealthApiServiceTest {

    private final ObjectMapper mapper = new ObjectMapper();

    @Test
    void emptyRollupsReturnZeroValuesInsteadOfThrowing() throws Exception {
        var empty = mapper.readTree("{\"rollupDataPoints\":[]}");

        var activity = GoogleHealthApiService.parseActivity(empty, empty, empty, empty);
        var weight = GoogleHealthApiService.parseWeight(empty);

        assertEquals(0, activity.getSteps());
        assertEquals(0, activity.getCaloriesOut());
        assertEquals(0, activity.getElevation());
        assertEquals(0, activity.getDistance());
        assertEquals(0, weight.getWeight());
        assertEquals(0, weight.getBmi());
    }

    @Test
    void parsesGoogleHealthDailyRollupsUsingLegacyDisplayUnits() throws Exception {
        var steps = mapper.readTree("{\"rollupDataPoints\":[{\"steps\":{\"countSum\":\"12345\"}}]}");
        var calories = mapper.readTree("{\"rollupDataPoints\":[{\"totalCalories\":{\"kcalSum\":2345.5}}]}");
        var altitude = mapper.readTree("{\"rollupDataPoints\":[{\"altitude\":{\"gainMillimetersSum\":\"123000\"}}]}");
        var distance = mapper.readTree("{\"rollupDataPoints\":[{\"distance\":{\"millimetersSum\":\"6789000\"}}]}");
        var weight = mapper.readTree("{\"rollupDataPoints\":[{\"weight\":{\"weightGramsAvg\":65400}}]}");

        var activity = GoogleHealthApiService.parseActivity(steps, calories, altitude, distance);
        var body = GoogleHealthApiService.parseWeight(weight);

        assertEquals(12345, activity.getSteps());
        assertEquals(2345.5, activity.getCaloriesOut());
        assertEquals(123, activity.getElevation());
        assertEquals(6.789, activity.getDistance(), 0.0001);
        assertEquals(65.4, body.getWeight(), 0.0001);
    }

    @Test
    void parsesReconciledRestingHeartRate() throws Exception {
        var response = mapper.readTree("""
                {"dataPoints":[{"dailyRestingHeartRate":{"beatsPerMinute":"58"}}]}
                """);

        assertEquals(58, GoogleHealthApiService.parseHeart(response).getRestingHeartRate());
    }

    @Test
    void parsesReconciledSleepAndReturnsSafeEmptyValue() throws Exception {
        var response = mapper.readTree("""
                {"dataPoints":[{"sleep":{
                  "interval":{
                    "civilStartTime":{"date":{"year":2026,"month":8,"day":4},"time":{"hours":23}},
                    "civilEndTime":{"date":{"year":2026,"month":8,"day":5},"time":{"hours":7}}
                  },
                  "summary":{"stagesSummary":[
                    {"type":"AWAKE","minutes":"30"},
                    {"type":"LIGHT","minutes":"210"},
                    {"type":"DEEP","minutes":"90"},
                    {"type":"REM","minutes":"120"}
                  ]}
                }}]}
                """);

        var sleep = GoogleHealthApiService.parseSleep(response);
        var empty = GoogleHealthApiService.parseSleep(mapper.readTree("{\"dataPoints\":[]}"));

        assertEquals(Duration.ofHours(7).plusMinutes(30), sleep.getSleeping());
        assertEquals(Duration.ofMinutes(30), sleep.getAwake());
        assertEquals(Duration.ofMinutes(300), sleep.getAsleep());
        assertEquals(Duration.ofMinutes(120), sleep.getRem());
        assertEquals(1, empty.getStartTime().getYear());
        assertEquals(Duration.ZERO, empty.getSleeping());
    }
}
