package com.scheduleviewer.api.config;

import com.scheduleviewer.infrastructure.config.AppProperties;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Configuration;
import org.springframework.web.servlet.config.annotation.ResourceHandlerRegistry;
import org.springframework.web.servlet.config.annotation.CorsRegistry;
import org.springframework.web.servlet.config.annotation.WebMvcConfigurer;

import java.nio.file.Paths;
import java.util.Arrays;
import java.util.stream.Stream;

/**
 * Spring MVC 設定
 * <p>ローカル写真フォルダを静的リソースとして公開する</p>
 */
@Configuration
public class WebMvcConfig implements WebMvcConfigurer {

    private static final Logger log = LoggerFactory.getLogger(WebMvcConfig.class);
    private static final String PRODUCTION_WEB_ORIGIN =
            "https://zealous-dune-09e3c0c10.1.azurestaticapps.net";

    private final AppProperties props;
    private final String[] allowedOriginPatterns;

    public WebMvcConfig(
            AppProperties props,
            @Value("${scheduleviewer.cors.allowed-origin-patterns}") String allowedOriginPatterns) {
        this.props = props;

        var configuredPatterns = Arrays.stream(allowedOriginPatterns.split(","))
                .map(String::trim)
                .filter(pattern -> !pattern.isEmpty());
        this.allowedOriginPatterns = Stream.concat(
                        configuredPatterns,
                        Stream.of(PRODUCTION_WEB_ORIGIN))
                .distinct()
                .toArray(String[]::new);

        log.info("CORS allowed origin patterns: {}", Arrays.toString(this.allowedOriginPatterns));
    }

    @Override
    public void addCorsMappings(CorsRegistry registry) {
        registry.addMapping("/api/**")
                .allowedOriginPatterns(allowedOriginPatterns)
                .allowedMethods("GET", "POST", "OPTIONS")
                .allowedHeaders("*");
    }

    @Override
    public void addResourceHandlers(ResourceHandlerRegistry registry) {
        String basePath = props.getLocalPhotoBasePath();
        if (basePath == null || basePath.isBlank()) return;

        String location = Paths.get(basePath).toUri().toString();
        if (!location.endsWith("/")) location += "/";

        registry.addResourceHandler("/local-photos/**")
                .addResourceLocations(location);
    }
}
