package com.scheduleviewer.infrastructure.config;

import org.springframework.boot.context.properties.ConfigurationProperties;
import org.springframework.stereotype.Component;

/**
 * アプリケーション設定プロパティ (application.yml マッピング)
 */
@Component
@ConfigurationProperties(prefix = "scheduleviewer")
public class AppProperties {

    private String databasePath;
    private final Google google = new Google();
    private final Fitbit fitbit = new Fitbit();
    private final Annict annict = new Annict();

    public String getDatabasePath() { return databasePath; }
    public void setDatabasePath(String databasePath) { this.databasePath = databasePath; }
    public Google getGoogle() { return google; }
    public Fitbit getFitbit() { return fitbit; }
    public Annict getAnnict() { return annict; }

    public static class Google {
        private String apiKey;
        private String clientSecretPath;
        private String driveFolderId;
        private String calendarId;

        public String getApiKey() { return apiKey; }
        public void setApiKey(String apiKey) { this.apiKey = apiKey; }
        public String getClientSecretPath() { return clientSecretPath; }
        public void setClientSecretPath(String clientSecretPath) { this.clientSecretPath = clientSecretPath; }
        public String getDriveFolderId() { return driveFolderId; }
        public void setDriveFolderId(String driveFolderId) { this.driveFolderId = driveFolderId; }
        public String getCalendarId() { return calendarId; }
        public void setCalendarId(String calendarId) { this.calendarId = calendarId; }
    }

    public static class Fitbit {
        private String clientId;
        private String clientSecret;
        private String redirectUri;
        private String tokenRequestUri;

        public String getClientId() { return clientId; }
        public void setClientId(String clientId) { this.clientId = clientId; }
        public String getClientSecret() { return clientSecret; }
        public void setClientSecret(String clientSecret) { this.clientSecret = clientSecret; }
        public String getRedirectUri() { return redirectUri; }
        public void setRedirectUri(String redirectUri) { this.redirectUri = redirectUri; }
        public String getTokenRequestUri() { return tokenRequestUri; }
        public void setTokenRequestUri(String tokenRequestUri) { this.tokenRequestUri = tokenRequestUri; }
    }

    public static class Annict {
        private String token;
        public String getToken() { return token; }
        public void setToken(String token) { this.token = token; }
    }
}
