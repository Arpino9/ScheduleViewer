package com.scheduleviewer.infrastructure.fitbit;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.scheduleviewer.domain.entity.*;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Service;

import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.time.Duration;
import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.util.ArrayList;
import java.util.List;

/**
 * Fitbit API データ取得サービス
 * <p>.NET版の FitbitReader に相当</p>
 */
@Service
public class FitbitApiService {

    private static final Logger log = LoggerFactory.getLogger(FitbitApiService.class);
    private static final DateTimeFormatter DATE_FMT = DateTimeFormatter.ofPattern("yyyy-MM-dd");

    private static final String BASE = "https://api.fitbit.com";
    private static final String EP_PROFILE  = BASE + "/1/user/-/profile.json";
    private static final String EP_SLEEP    = BASE + "/1.2/user/-/sleep/date/%s.json";
    private static final String EP_ACTIVITY = BASE + "/1/user/-/activities/date/%s.json";
    private static final String EP_HEART    = BASE + "/1/user/-/activities/heart/date/%s/1d.json";
    private static final String EP_WEIGHT   = BASE + "/1/user/-/body/log/weight/date/%s.json";

    private final FitbitTokenStore tokenStore;
    private final FitbitAuthService authService;
    private final ObjectMapper mapper = new ObjectMapper();

    public FitbitApiService(FitbitTokenStore tokenStore, FitbitAuthService authService) {
        this.tokenStore  = tokenStore;
        this.authService = authService;
    }

    /** プロフィールを取得する */
    public FitbitProfileEntity getProfile() throws Exception {
        String json = fetch(EP_PROFILE);
        if (json.isEmpty()) return emptyProfile();

        JsonNode root = mapper.readTree(json);
        JsonNode user = root.get("user");

        List<FitbitProfileEntity.EarnedBadge> badges = new ArrayList<>();
        JsonNode topBadges = user.get("topBadges");
        if (topBadges != null) {
            for (JsonNode badge : topBadges) {
                badges.add(new FitbitProfileEntity.EarnedBadge(
                        badge.get("category").asText(),
                        LocalDateTime.parse(badge.get("dateTime").asText() + "T00:00:00"),
                        badge.get("description").asText()));
            }
        }

        return new FitbitProfileEntity(
                user.get("fullName").asText(),
                user.get("age").asInt(),
                user.get("gender").asText(),
                user.get("height").asDouble(),
                user.get("weight").asDouble(),
                badges);
    }

    /** 睡眠データを取得する */
    public FitbitSleepEntity getSleep(LocalDate date) throws Exception {
        String json = fetch(EP_SLEEP.formatted(date.format(DATE_FMT)));
        if (json.isEmpty()) return emptySleep();

        JsonNode root  = mapper.readTree(json);
        JsonNode sleep = null;
        JsonNode arr   = root.get("sleep");
        if (arr != null && arr.isArray() && arr.size() > 0) {
            sleep = arr.get(arr.size() - 1); // 最後の記録
        }
        if (sleep == null) return emptySleep();

        LocalDateTime startTime = LocalDateTime.parse(sleep.get("startTime").asText());
        LocalDateTime endTime   = LocalDateTime.parse(sleep.get("endTime").asText());

        JsonNode summary = sleep.path("levels").path("summary");
        Duration asleep   = minutesToDuration(summary, "asleep");
        Duration rem      = minutesToDuration(summary, "rem");
        Duration awake    = minutesToDuration(summary, "awake");
        Duration restless = minutesToDuration(summary, "restless");

        return new FitbitSleepEntity(startTime, endTime, awake, restless, rem, asleep);
    }

    /** アクティビティを取得する */
    public FitbitActivityEntity getActivity(LocalDate date) throws Exception {
        String json = fetch(EP_ACTIVITY.formatted(date.format(DATE_FMT)));
        if (json.isEmpty()) return new FitbitActivityEntity(0, 0, 0, 0);

        JsonNode summary = mapper.readTree(json).get("summary");
        return new FitbitActivityEntity(
                summary.get("steps").asDouble(),
                summary.get("caloriesOut").asDouble(),
                summary.get("elevation").asDouble(),
                summary.path("distances").path(0).path("distance").asDouble());
    }

    /** 心拍数を取得する */
    public FitbitHeartEntity getHeart(LocalDate date) throws Exception {
        String json = fetch(EP_HEART.formatted(date.format(DATE_FMT)));
        if (json.isEmpty()) return new FitbitHeartEntity(0);

        JsonNode value = mapper.readTree(json)
                .path("activities-heart").path(0).path("value").path("restingHeartRate");
        return new FitbitHeartEntity(value.isMissingNode() ? 0 : value.asDouble());
    }

    /** 体重を取得する */
    public FitbitWeightEntity getWeight(LocalDate date) throws Exception {
        String json = fetch(EP_WEIGHT.formatted(date.format(DATE_FMT)));
        if (json.isEmpty()) return new FitbitWeightEntity(0, 0);

        JsonNode w = mapper.readTree(json).path("weight").path(0);
        return new FitbitWeightEntity(w.get("bmi").asDouble(), w.get("weight").asDouble());
    }

    /** トークンを付けてAPIを呼び出す */
    private String fetch(String endpoint) throws Exception {
        if (!tokenStore.hasToken()) {
            log.error("アクセストークンが設定されていません");
            return "";
        }
        if (tokenStore.isExpired()) {
            log.info("アクセストークン期限切れ - リフレッシュ中");
            authService.refreshAccessToken();
            if (tokenStore.isExpired()) {
                log.error("トークンリフレッシュ失敗 - 再認証が必要です");
                tokenStore.clear();
                return "";
            }
        }

        var request = HttpRequest.newBuilder()
                .uri(URI.create(endpoint))
                .header("Authorization", "Bearer " + tokenStore.getAccessToken())
                .GET()
                .build();

        var response = HttpClient.newHttpClient().send(request, HttpResponse.BodyHandlers.ofString());
        if (response.statusCode() != 200) {
            log.error("Fitbit API エラー: {} {}", response.statusCode(), response.body());
            return "";
        }
        return response.body();
    }

    private Duration minutesToDuration(JsonNode summary, String key) {
        JsonNode node = summary.path(key).path("minutes");
        return node.isMissingNode() ? Duration.ZERO : Duration.ofMinutes(node.asLong());
    }

    private FitbitProfileEntity emptyProfile() {
        return new FitbitProfileEntity("", 0, "", 0, 0, List.of());
    }

    private FitbitSleepEntity emptySleep() {
        return new FitbitSleepEntity(
                LocalDateTime.MIN, LocalDateTime.MIN,
                Duration.ZERO, Duration.ZERO, Duration.ZERO, Duration.ZERO);
    }
}
