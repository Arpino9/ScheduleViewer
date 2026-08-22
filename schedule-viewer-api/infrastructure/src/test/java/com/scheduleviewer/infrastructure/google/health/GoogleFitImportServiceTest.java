package com.scheduleviewer.infrastructure.google.health;

import org.junit.jupiter.api.Test;

import java.io.ByteArrayInputStream;
import java.nio.charset.StandardCharsets;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

class GoogleFitImportServiceTest {

    private final GoogleFitImportService service = new GoogleFitImportService(null);

    @Test
    void parsesGoogleFitExerciseSession() throws Exception {
        var points = parse("walk.json", """
                {
                  "fitnessActivity": "walking",
                  "startTime": "2024-10-15T21:49:22.997Z",
                  "endTime": "2024-10-15T21:59:22.049Z",
                  "duration": "599.052s",
                  "aggregate": [
                    {"metricName":"com.google.calories.expended","floatValue":27.5},
                    {"metricName":"com.google.step_count.delta","intValue":1188},
                    {"metricName":"com.google.distance.delta","floatValue":745.2}
                  ]
                }
                """);

        assertEquals(1, points.size());
        var point = points.get(0);
        assertEquals("exercise", point.dataType());
        assertEquals("WALKING", point.body().path("exercise").path("exerciseType").asText());
        assertEquals("1188", point.body().path("exercise").path("metricsSummary").path("steps").asText());
        assertEquals(745_200, point.body().path("exercise").path("metricsSummary").path("distanceMillimeters").asDouble());
        assertTrue(point.body().path("name").asText().startsWith("users/me/dataTypes/exercise/dataPoints/gfit-"));
    }

    @Test
    void parsesGoogleFitWeightDataPoints() throws Exception {
        var points = parse("weight.json", """
                {"Data Points":[
                  {"dataTypeName":"com.google.weight","startTimeNanos":1717215540000000000,
                   "endTimeNanos":1717215540000000000,"fitValue":[{"value":{"fpVal":43.5}}]}
                ]}
                """);

        assertEquals(1, points.size());
        assertEquals("weight", points.get(0).dataType());
        assertEquals(43_500, points.get(0).body().path("weight").path("weightGrams").asDouble());
    }

    @Test
    void parsesGoogleFitSleepSegment() throws Exception {
        var points = parse("sleep.json", """
                {"Data Points":[
                  {"dataTypeName":"com.google.sleep.segment","startTimeNanos":1716818400000000000,
                   "endTimeNanos":1716850800000000000,"fitValue":[{"value":{"intVal":2}}]}
                ]}
                """);

        assertEquals(1, points.size());
        assertEquals("sleep", points.get(0).dataType());
        assertEquals("ASLEEP", points.get(0).body().path("sleep").path("stages").get(0).path("type").asText());
    }

    @Test
    void groupsContiguousSleepStagesIntoOneSession() throws Exception {
        var points = parse("sleep.json", """
                {"Data Points":[
                  {"dataTypeName":"com.google.sleep.segment","startTimeNanos":1716818400000000000,
                   "endTimeNanos":1716822000000000000,"fitValue":[{"value":{"intVal":4}}]},
                  {"dataTypeName":"com.google.sleep.segment","startTimeNanos":1716822000000000000,
                   "endTimeNanos":1716825600000000000,"fitValue":[{"value":{"intVal":5}}]},
                  {"dataTypeName":"com.google.sleep.segment","startTimeNanos":1716825600000000000,
                   "endTimeNanos":1716829200000000000,"fitValue":[{"value":{"intVal":6}}]}
                ]}
                """);

        assertEquals(1, points.size());
        var sleep = points.get(0).body().path("sleep");
        assertEquals("STAGES", sleep.path("type").asText());
        assertEquals(3, sleep.path("stages").size());
        assertEquals("LIGHT", sleep.path("stages").get(0).path("type").asText());
        assertEquals("DEEP", sleep.path("stages").get(1).path("type").asText());
        assertEquals("REM", sleep.path("stages").get(2).path("type").asText());
    }

    @Test
    void separatesSleepSessionsAcrossGaps() throws Exception {
        var points = parse("sleep.json", """
                {"Data Points":[
                  {"dataTypeName":"com.google.sleep.segment","startTimeNanos":1716818400000000000,
                   "endTimeNanos":1716822000000000000,"fitValue":[{"value":{"intVal":2}}]},
                  {"dataTypeName":"com.google.sleep.segment","startTimeNanos":1716832800000000000,
                   "endTimeNanos":1716836400000000000,"fitValue":[{"value":{"intVal":2}}]}
                ]}
                """);

        assertEquals(2, points.size());
    }

    @Test
    void reportsUnsupportedGoogleFitDataType() {
        assertThrows(GoogleFitImportService.UnsupportedFormatException.class, () -> parse("steps.json", """
                {"Data Points":[{"dataTypeName":"com.google.step_count.delta"}]}
                """));
    }

    @Test
    void stableNameDoesNotDependOnTakeoutFileName() throws Exception {
        String json = """
                {"Data Points":[
                  {"dataTypeName":"com.google.weight","startTimeNanos":1717215540000000000,
                   "endTimeNanos":1717215540000000000,"fitValue":[{"value":{"fpVal":43.5}}]}
                ]}
                """;

        var first = parse("weight.json", json).getFirst();
        var renamed = parse("renamed-weight.json", json).getFirst();

        assertEquals(first.name(), renamed.name());
    }

    private java.util.List<GoogleFitImportService.HealthPoint> parse(String name, String json) throws Exception {
        return service.parse(name, new ByteArrayInputStream(json.getBytes(StandardCharsets.UTF_8)));
    }
}
