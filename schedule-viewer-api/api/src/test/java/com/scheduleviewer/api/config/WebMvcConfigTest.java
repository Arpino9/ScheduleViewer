package com.scheduleviewer.api.config;

import com.scheduleviewer.api.controller.FitbitController;
import com.scheduleviewer.infrastructure.config.AppProperties;
import com.scheduleviewer.infrastructure.google.health.GoogleFitImportService;
import com.scheduleviewer.infrastructure.google.health.GoogleHealthApiService;
import com.scheduleviewer.infrastructure.google.health.GoogleHealthAuthService;
import org.junit.jupiter.api.Test;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.mock.web.MockServletContext;
import org.springframework.web.context.support.AnnotationConfigWebApplicationContext;
import org.springframework.web.servlet.config.annotation.EnableWebMvc;

import static org.mockito.ArgumentMatchers.any;
import static org.mockito.ArgumentMatchers.eq;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.when;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.*;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.*;
import static org.springframework.test.web.servlet.setup.MockMvcBuilders.webAppContextSetup;

/** Exercise updates must work through the same CORS policy as the deployed browser client. */
class WebMvcConfigTest {
    private static final String WEB_ORIGIN = "https://zealous-dune-09e3c0c10.1.azurestaticapps.net";

    @Configuration
    @EnableWebMvc
    static class Config {
        @Bean WebMvcConfig webMvcConfig() {
            return new WebMvcConfig(new AppProperties(), "http://localhost:*");
        }
        @Bean GoogleHealthApiService healthApi() { return mock(GoogleHealthApiService.class); }
        @Bean FitbitController fitbitController(GoogleHealthApiService healthApi) {
            return new FitbitController(healthApi, mock(GoogleHealthAuthService.class), mock(GoogleFitImportService.class));
        }
    }

    @Test
    void exercisePatchPassesBrowserPreflightAndReachesUpdateHandler() throws Exception {
        try (var context = new AnnotationConfigWebApplicationContext()) {
            context.setServletContext(new MockServletContext());
            context.register(Config.class);
            context.refresh();
            var mvc = webAppContextSetup(context).build();
            when(context.getBean(GoogleHealthApiService.class).saveExercise(any(), eq(true)))
                    .thenReturn(new GoogleHealthApiService.ExerciseSaveResult("saved"));

            for (String origin : new String[] { WEB_ORIGIN, "http://localhost:5000" }) {
                mvc.perform(options("/api/fitbit/exercises")
                                .header("Origin", origin)
                                .header("Access-Control-Request-Method", "PATCH")
                                .header("Access-Control-Request-Headers", "authorization,content-type"))
                        .andExpect(status().isOk())
                        .andExpect(header().string("Access-Control-Allow-Origin", origin))
                        .andExpect(header().string("Access-Control-Allow-Methods", "GET,POST,PATCH,OPTIONS"));

                mvc.perform(patch("/api/fitbit/exercises").header("Origin", origin)
                                .contentType("application/json").content("""
                                    {"name":"users/123/dataTypes/exercise/dataPoints/456",
                                     "exerciseType":"BIKING","startTime":"2026-10-17T12:00:00",
                                     "endTime":"2026-10-17T12:30:00","notes":"updated"}
                                    """))
                        .andExpect(status().isOk())
                        .andExpect(header().string("Access-Control-Allow-Origin", origin))
                        .andExpect(jsonPath("$.status").value("saved"));
            }

            // Adding PATCH must not broaden the origin allowlist.
            mvc.perform(options("/api/fitbit/exercises")
                            .header("Origin", "https://untrusted.example")
                            .header("Access-Control-Request-Method", "PATCH"))
                    .andExpect(status().isForbidden())
                    .andExpect(header().doesNotExist("Access-Control-Allow-Origin"));
        }
    }
}
