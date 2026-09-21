package com.scheduleviewer.api.config;

import com.scheduleviewer.infrastructure.config.AppProperties;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Configuration;
import org.springframework.web.servlet.config.annotation.ResourceHandlerRegistry;
import org.springframework.web.servlet.config.annotation.CorsRegistry;
import org.springframework.web.servlet.config.annotation.WebMvcConfigurer;

import java.nio.file.Paths;
import java.util.Arrays;

/**
 * Spring MVC 設定
 * <p>ローカル写真フォルダを静的リソースとして公開する</p>
 */
@Configuration
public class WebMvcConfig implements WebMvcConfigurer {

    private final AppProperties props;
    private final String[] allowedOriginPatterns;

    public WebMvcConfig(
            AppProperties props,
            @Value("${scheduleviewer.cors.allowed-origin-patterns}") String allowedOriginPatterns) {
        this.props = props;
        this.allowedOriginPatterns = Arrays.stream(allowedOriginPatterns.split(","))
                .map(String::trim)
                .filter(pattern -> !pattern.isEmpty())
                .toArray(String[]::new);
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
