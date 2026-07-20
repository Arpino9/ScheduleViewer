package com.scheduleviewer.infrastructure.config;

import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.web.client.RestTemplate;

/**
 * Infrastructure層のBean設定
 */
@Configuration
public class InfrastructureConfig {

    /**
     * 外部HTTPリクエスト用 RestTemplate
     * (Fitbit, Annict, Nominatim 等で使用)
     */
    @Bean
    public RestTemplate restTemplate() {
        return new RestTemplate();
    }
}
