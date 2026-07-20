package com.scheduleviewer.infrastructure.google.photo;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.google.api.client.auth.oauth2.Credential;
import com.scheduleviewer.domain.entity.PhotoEntity;
import com.scheduleviewer.infrastructure.google.GoogleAuthService;
import jakarta.annotation.PostConstruct;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.http.*;
import org.springframework.stereotype.Service;
import org.springframework.web.client.RestTemplate;

import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.Map;

/**
 * Google Photos サービス
 * <p>.NET版の PhotoReader に相当</p>
 *
 * <p><b>注意:</b> Google Photos Library APIは2025/04/01の変更により制限が大幅に強化された。
 * 公式Javaクライアントが提供されていないため、REST APIを直接呼び出す実装を採用している。
 * 最新のAPIドキュメントを参照してスコープ・エンドポイントを確認すること。</p>
 *
 * @deprecated Google Photos APIの仕様変更により機能が制限されている
 */
@Deprecated
@Service
public class PhotoService {

    private static final Logger log = LoggerFactory.getLogger(PhotoService.class);

    private static final String PHOTOS_SCOPE     = "https://www.googleapis.com/auth/photoslibrary.readonly";
    private static final String MEDIA_ITEMS_URL  = "https://photoslibrary.googleapis.com/v1/mediaItems:search";

    private final GoogleAuthService authService;
    private final RestTemplate restTemplate;
    private final ObjectMapper mapper = new ObjectMapper();

    private final List<PhotoEntity> photos = new ArrayList<>();

    public PhotoService(GoogleAuthService authService, RestTemplate restTemplate) {
        this.authService  = authService;
        this.restTemplate = restTemplate;
    }

    /** 起動時に非同期で写真を読み込む (トークンが存在する場合のみ) */
    @PostConstruct
    public void initializeAsync() {
        if (!authService.hasToken("token_Photos")) {
            log.info("Google Photos トークンが未設定のため起動時読み込みをスキップします");
            return;
        }
        Thread.ofVirtual().start(() -> {
            try {
                load();
            } catch (Exception e) {
                log.error("Google Photosの読み込みに失敗しました", e);
            }
        });
    }

    /** OAuth認証URLを取得する。認証完了後に自動でデータを読み込む。認証済みの場合は null を返す。 */
    public String getAuthUrl() throws Exception {
        return authService.startAuthFlowAndGetUrl(List.of(PHOTOS_SCOPE), "token_Photos", () -> {
            try { load(); } catch (Exception e) { log.error("Photos reload after auth failed", e); }
        });
    }

    /** 写真を全件取得してキャッシュする */
    public synchronized void load() throws Exception {
        photos.clear();

        Credential credential = authService.authorize(List.of(PHOTOS_SCOPE), "token_Photos");
        String accessToken = credential.getAccessToken();

        String nextPageToken = null;
        do {
            var result = fetchMediaItems(accessToken, nextPageToken);
            if (result == null) break;

            JsonNode items = result.get("mediaItems");
            if (items == null || !items.isArray()) break;

            for (JsonNode item : items) {
                PhotoEntity photo = mapPhoto(item);
                if (photo != null) photos.add(photo);
            }

            JsonNode token = result.get("nextPageToken");
            nextPageToken = (token != null) ? token.asText(null) : null;

        } while (nextPageToken != null);

        log.info("Google Photos読み込み完了: {}件", photos.size());
    }

    /** 日付で写真を検索する */
    public List<PhotoEntity> findByDate(LocalDate date) {
        return photos.stream()
                .filter(p -> p.getDate().toLocalDate().equals(date))
                .toList();
    }

    /** 全写真を返す */
    public List<PhotoEntity> getAll() {
        return Collections.unmodifiableList(photos);
    }

    /** Google Photos REST API からメディアアイテムを取得する */
    private JsonNode fetchMediaItems(String accessToken, String pageToken) {
        HttpHeaders headers = new HttpHeaders();
        headers.setBearerAuth(accessToken);
        headers.setContentType(MediaType.APPLICATION_JSON);

        Map<String, Object> body = new java.util.LinkedHashMap<>();
        body.put("pageSize", 100);
        if (pageToken != null) {
            body.put("pageToken", pageToken);
        }

        HttpEntity<Map<String, Object>> request = new HttpEntity<>(body, headers);

        try {
            ResponseEntity<String> response = restTemplate.exchange(
                    MEDIA_ITEMS_URL, HttpMethod.POST, request, String.class);

            if (response.getStatusCode().is2xxSuccessful() && response.getBody() != null) {
                return mapper.readTree(response.getBody());
            }
            log.warn("Photos API レスポンスエラー: {}", response.getStatusCode());
        } catch (Exception e) {
            log.error("Photos APIの呼び出しに失敗しました", e);
        }
        return null;
    }

    private PhotoEntity mapPhoto(JsonNode item) {
        try {
            String id       = item.path("id").asText("");
            String fileName = item.path("filename").asText("");
            String desc     = item.path("description").asText("");
            String url      = item.path("productUrl").asText("");
            String mime     = item.path("mimeType").asText("");

            JsonNode meta   = item.path("mediaMetadata");
            long height     = meta.path("height").asLong(0);
            long width      = meta.path("width").asLong(0);

            // 撮影日時 (RFC3339)
            String creationTime = meta.path("creationTime").asText("");
            LocalDateTime dateTime = parseDateTime(creationTime);

            return new PhotoEntity(id, dateTime, fileName, desc, url, mime, height, width);
        } catch (Exception e) {
            log.warn("写真データのマッピングに失敗しました: {}", item, e);
            return null;
        }
    }

    private LocalDateTime parseDateTime(String rfc3339) {
        if (rfc3339 == null || rfc3339.isEmpty()) return LocalDateTime.MIN;
        try {
            return LocalDateTime.parse(rfc3339, DateTimeFormatter.ofPattern("yyyy-MM-dd'T'HH:mm:ss'Z'"));
        } catch (Exception e) {
            try {
                return LocalDateTime.parse(rfc3339, DateTimeFormatter.ISO_DATE_TIME);
            } catch (Exception e2) {
                return LocalDateTime.MIN;
            }
        }
    }
}
