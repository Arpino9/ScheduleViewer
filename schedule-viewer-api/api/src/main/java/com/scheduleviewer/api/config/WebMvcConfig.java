package com.scheduleviewer.api.config;

import com.scheduleviewer.infrastructure.config.AppProperties;
import org.springframework.context.annotation.Configuration;
import org.springframework.web.servlet.config.annotation.ResourceHandlerRegistry;
import org.springframework.web.servlet.config.annotation.WebMvcConfigurer;

import java.nio.file.Paths;

/**
 * Spring MVC 設定
 * <p>ローカル写真フォルダを静的リソースとして公開する</p>
 */
@Configuration
public class WebMvcConfig implements WebMvcConfigurer {

    private final AppProperties props;

    public WebMvcConfig(AppProperties props) {
        this.props = props;
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
