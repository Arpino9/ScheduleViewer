package com.scheduleviewer.api.controller;

import com.scheduleviewer.domain.entity.*;
import com.scheduleviewer.infrastructure.fitbit.FitbitApiService;
import com.scheduleviewer.infrastructure.fitbit.FitbitAuthService;
import org.springframework.format.annotation.DateTimeFormat;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDate;

/**
 * Fitbit REST コントローラー
 */
@RestController
@RequestMapping("/api/fitbit")
public class FitbitController {

    private final FitbitApiService apiService;
    private final FitbitAuthService authService;

    public FitbitController(FitbitApiService apiService, FitbitAuthService authService) {
        this.apiService  = apiService;
        this.authService = authService;
    }

    /** OAuth2 PKCE 認証を開始し、認証URLを返す */
    @PostMapping("/auth")
    public java.util.Map<String, String> authorize() throws Exception {
        String url = authService.initialize();
        return java.util.Map.of("status", "pending", "url", url);
    }

    /** プロフィールを取得する */
    @GetMapping("/profile")
    public FitbitProfileEntity getProfile() throws Exception {
        return apiService.getProfile();
    }

    /** 睡眠データを取得する */
    @GetMapping("/sleep")
    public FitbitSleepEntity getSleep(
            @RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate date) throws Exception {
        return apiService.getSleep(date);
    }

    /** アクティビティを取得する */
    @GetMapping("/activity")
    public FitbitActivityEntity getActivity(
            @RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate date) throws Exception {
        return apiService.getActivity(date);
    }

    /** 心拍数を取得する */
    @GetMapping("/heart")
    public FitbitHeartEntity getHeart(
            @RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate date) throws Exception {
        return apiService.getHeart(date);
    }

    /** 体重を取得する */
    @GetMapping("/weight")
    public FitbitWeightEntity getWeight(
            @RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate date) throws Exception {
        return apiService.getWeight(date);
    }
}
