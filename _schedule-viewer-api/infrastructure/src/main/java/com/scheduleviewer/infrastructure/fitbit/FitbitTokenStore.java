package com.scheduleviewer.infrastructure.fitbit;

import com.fasterxml.jackson.databind.ObjectMapper;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Component;

import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.time.Instant;
import java.util.Map;

/**
 * Fitbit アクセストークン管理 (ファイル永続化)
 */
@Component
public class FitbitTokenStore {

    private static final Logger log = LoggerFactory.getLogger(FitbitTokenStore.class);
    private static final Path TOKEN_FILE =
            Paths.get(System.getProperty("user.home"), ".scheduleviewer", "fitbit", "token.json");

    private final ObjectMapper objectMapper = new ObjectMapper();

    private String accessToken  = "";
    private String refreshToken = "";
    private Instant expiry      = Instant.EPOCH;

    public FitbitTokenStore() {
        loadFromFile();
    }

    public String getAccessToken()  { return accessToken; }
    public String getRefreshToken() { return refreshToken; }
    public Instant getExpiry()      { return expiry; }

    public boolean hasToken()  { return !accessToken.isEmpty(); }
    public boolean isExpired() { return Instant.now().isAfter(expiry); }

    public void store(String accessToken, String refreshToken, long expiresInSeconds) {
        this.accessToken  = accessToken;
        this.refreshToken = refreshToken;
        this.expiry       = Instant.now().plusSeconds(expiresInSeconds);
        saveToFile();
    }

    public void clear() {
        this.accessToken  = "";
        this.refreshToken = "";
        this.expiry       = Instant.EPOCH;
        saveToFile();
    }

    private void loadFromFile() {
        try {
            if (!Files.exists(TOKEN_FILE)) return;
            @SuppressWarnings("unchecked")
            Map<String, String> map = objectMapper.readValue(TOKEN_FILE.toFile(), Map.class);
            this.accessToken  = map.getOrDefault("accessToken", "");
            this.refreshToken = map.getOrDefault("refreshToken", "");
            this.expiry       = Instant.ofEpochSecond(Long.parseLong(map.getOrDefault("expiryEpoch", "0")));
            log.info("Fitbitトークンをファイルから読み込みました");
        } catch (Exception e) {
            log.warn("Fitbitトークンファイルの読み込み失敗: {}", e.getMessage());
        }
    }

    private void saveToFile() {
        try {
            Files.createDirectories(TOKEN_FILE.getParent());
            var map = Map.of(
                "accessToken",  accessToken,
                "refreshToken", refreshToken,
                "expiryEpoch",  String.valueOf(expiry.getEpochSecond())
            );
            objectMapper.writeValue(TOKEN_FILE.toFile(), map);
        } catch (Exception e) {
            log.warn("Fitbitトークンファイルへの保存失敗: {}", e.getMessage());
        }
    }
}
