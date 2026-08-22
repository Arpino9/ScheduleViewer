package com.scheduleviewer.api.controller;

import com.scheduleviewer.domain.entity.*;
import com.scheduleviewer.infrastructure.google.health.GoogleHealthApiService;
import com.scheduleviewer.infrastructure.google.health.GoogleHealthAuthService;
import com.scheduleviewer.infrastructure.google.health.GoogleFitImportService;
import org.springframework.format.annotation.DateTimeFormat;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.multipart.MultipartFile;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDate;

/**
 * Google Health REST コントローラー。
 * 既存クライアントとの互換性を保つため、URLは /api/fitbit のまま提供する。
 */
@RestController
@RequestMapping("/api/fitbit")
public class FitbitController {

    private final GoogleHealthApiService apiService;
    private final GoogleHealthAuthService authService;
    private final GoogleFitImportService importService;

    public FitbitController(GoogleHealthApiService apiService, GoogleHealthAuthService authService,
                            GoogleFitImportService importService) {
        this.apiService  = apiService;
        this.authService = authService;
        this.importService = importService;
    }

    /** OAuth2 PKCE 認証を開始し、認証URLを返す */
    @PostMapping("/auth")
    public java.util.Map<String, String> authorize() throws Exception {
        String url = authService.initialize();
        if (url == null) {
            return java.util.Map.of("status", "already_authorized");
        }
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

    /** Google Healthへのインポート用認証状態を返す。 */
    @GetMapping("/import/status")
    public java.util.Map<String, Boolean> importStatus() {
        return java.util.Map.of("authorized", authService.hasImportToken());
    }

    /** Google Healthへの書き込み認証を開始する。 */
    @PostMapping("/import/auth")
    public java.util.Map<String, Object> authorizeImport(
            @RequestParam(defaultValue = "false") boolean force) throws Exception {
        String url = force ? authService.reauthorizeImport() : authService.initializeImport();
        return url == null
                ? java.util.Map.of("status", "already_authorized")
                : java.util.Map.of("status", "pending", "url", url);
    }

    /** Google Fit TakeoutのJSONファイルをGoogle Healthへ取り込む。 */
    @PostMapping(value = "/import/google-fit", consumes = "multipart/form-data")
    public ResponseEntity<?> importGoogleFit(@RequestParam("files") MultipartFile[] files) {
        if (files.length == 0) {
            return ResponseEntity.badRequest().body(java.util.Map.of("message", "JSON ファイルを選択してください。"));
        }
        if (files.length > 100) {
            return ResponseEntity.badRequest().body(java.util.Map.of("message", "一度に選択できるのは100ファイルまでです。"));
        }
        for (MultipartFile file : files) {
            if (file.isEmpty() || file.getOriginalFilename() == null
                    || !file.getOriginalFilename().toLowerCase(java.util.Locale.ROOT).endsWith(".json")) {
                return ResponseEntity.badRequest().body(java.util.Map.of("message", "JSON ファイルのみ選択できます。"));
            }
        }

        var results = new java.util.ArrayList<GoogleFitImportService.ImportResult>();
        try {
            for (MultipartFile file : files) {
                try {
                    results.add(importService.importJson(file.getOriginalFilename(), file.getInputStream()));
                } catch (GoogleFitImportService.UnsupportedFormatException e) {
                    results.add(new GoogleFitImportService.ImportResult(
                            file.getOriginalFilename(), 0, 0, 0, java.util.List.of(e.getMessage())));
                }
            }
            return ResponseEntity.ok(java.util.Map.of("files", results));
        } catch (GoogleFitImportService.ImportAuthorizationException e) {
            return ResponseEntity.status(HttpStatus.UNAUTHORIZED).body(java.util.Map.of("message", e.getMessage()));
        } catch (Exception e) {
            String message = e.getMessage() == null ? "インポート中に予期しないエラーが発生しました。" : e.getMessage();
            return ResponseEntity.internalServerError().body(java.util.Map.of("message", message));
        }
    }
}
