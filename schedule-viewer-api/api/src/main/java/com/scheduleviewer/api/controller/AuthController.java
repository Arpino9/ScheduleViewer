package com.scheduleviewer.api.controller;

import com.scheduleviewer.infrastructure.google.GoogleAuthService;
import com.scheduleviewer.infrastructure.google.calendar.CalendarService;
import com.scheduleviewer.infrastructure.google.drive.DriveService;
import com.scheduleviewer.infrastructure.google.photo.PhotoService;
import com.scheduleviewer.infrastructure.google.spreadsheet.SpreadsheetService;
import com.scheduleviewer.infrastructure.google.tasks.TasksService;
import com.scheduleviewer.infrastructure.google.health.GoogleHealthAuthService;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.LinkedHashMap;
import java.util.Map;

/**
 * Google OAuth 認証コントローラー
 * <p>各サービスのトークン有無確認・認証トリガーを提供する</p>
 */
@RestController
@RequestMapping("/api/auth")
public class AuthController {

    private static final Logger log = LoggerFactory.getLogger(AuthController.class);
    private static final MediaType HTML_UTF8 =
            MediaType.parseMediaType("text/html;charset=UTF-8");

    private final GoogleAuthService authService;
    private final CalendarService calendarService;
    private final TasksService tasksService;
    private final DriveService driveService;
    private final PhotoService photoService;
    private final SpreadsheetService spreadsheetService;
    private final GoogleHealthAuthService healthAuthService;

    public AuthController(
            GoogleAuthService authService,
            CalendarService calendarService,
            TasksService tasksService,
            DriveService driveService,
            PhotoService photoService,
            SpreadsheetService spreadsheetService,
            GoogleHealthAuthService healthAuthService) {
        this.authService = authService;
        this.calendarService = calendarService;
        this.tasksService = tasksService;
        this.driveService = driveService;
        this.photoService = photoService;
        this.spreadsheetService = spreadsheetService;
        this.healthAuthService = healthAuthService;
    }

    /**
     * 各サービスの認証状態を返す
     * <p>true = トークンあり (認証済み)</p>
     */
    @GetMapping("/status")
    public Map<String, Boolean> status() {
        Map<String, Boolean> result = new LinkedHashMap<>();
        result.put("calendar", authService.hasToken("token_Calendar"));
        result.put("tasks", authService.hasToken("token_Tasks"));
        result.put("drive", authService.hasToken("token_Drive"));
        result.put("photos", authService.hasToken("token_Photos"));
        result.put("sheets", authService.hasToken("token_Sheets"));
        result.put("fitbit", healthAuthService.hasToken());
        return result;
    }

    /**
     * 指定サービスのGoogle OAuthを開始し、認証URLを返す。
     *
     * @param service calendar | tasks | drive | photos | sheets
     */
    @PostMapping("/google/{service}")
    public Map<String, Object> authorizeGoogle(
            @PathVariable String service,
            @RequestParam(defaultValue = "false") boolean force) throws Exception {
        String url = switch (service) {
            case "calendar" -> calendarService.getAuthUrl(force);
            case "tasks" -> tasksService.getAuthUrl(force);
            case "drive" -> driveService.getAuthUrl(force);
            case "photos" -> photoService.getAuthUrl(force);
            case "sheets" -> spreadsheetService.getAuthUrl(force);
            case "fitbit" -> force
                    ? healthAuthService.reauthorize()
                    : healthAuthService.initialize();
            default -> {
                log.warn("不明なサービス: {}", service);
                yield null;
            }
        };

        if (url == null) {
            return Map.of(
                    "status", "already_authorized",
                    "message", service + " はすでに認証済みです。");
        }

        log.info("認証URL取得: {}", service);
        return Map.of(
                "status", "pending",
                "url", url,
                "message", service + " の認証URLを取得しました。");
    }

    /**
     * Googleからの固定HTTPSコールバックを処理する。
     * stateを検証後、認可コードをトークンへ交換して永続化する。
     */
    @GetMapping("/google/callback")
    public ResponseEntity<String> googleCallback(
            @RequestParam(required = false) String code,
            @RequestParam(required = false) String state,
            @RequestParam(required = false) String error) {
        if (error != null) {
            log.warn("Google OAuthが拒否されました: {}", error);
            return ResponseEntity.badRequest()
                    .contentType(HTML_UTF8)
                    .body(htmlPage(
                            "Google認証を完了できませんでした",
                            "認証がキャンセルされたか、Googleから拒否されました。"));
        }

        try {
            authService.completeWebAuthorization(code, state);
            return ResponseEntity.ok()
                    .contentType(HTML_UTF8)
                    .body(htmlPage(
                            "Google認証が完了しました",
                            "このタブを閉じてScheduleViewerへ戻り、「状態を更新」を押してください。"));
        } catch (IllegalArgumentException e) {
            log.warn("Google OAuthコールバックが無効です: {}", e.getMessage());
            return ResponseEntity.badRequest()
                    .contentType(HTML_UTF8)
                    .body(htmlPage(
                            "Google認証を完了できませんでした",
                            "認証の有効期限が切れています。ScheduleViewerからもう一度お試しください。"));
        } catch (Exception e) {
            log.error("Google OAuthコールバック処理に失敗しました", e);
            return ResponseEntity.internalServerError()
                    .contentType(HTML_UTF8)
                    .body(htmlPage(
                            "Google認証を完了できませんでした",
                            "サーバーでエラーが発生しました。ログを確認してください。"));
        }
    }

    /** 全サービスを一括認証し、各URLを返す。 */
    @PostMapping("/google/all")
    public Map<String, Object> authorizeAll() throws Exception {
        Map<String, Object> result = new LinkedHashMap<>();
        for (String service : new String[]{"calendar", "tasks", "drive", "photos"}) {
            result.put(service, authorizeGoogle(service, false));
        }
        return result;
    }

    private static String htmlPage(String title, String message) {
        return """
                <!doctype html>
                <html lang="ja">
                <head>
                  <meta charset="utf-8">
                  <meta name="viewport" content="width=device-width,initial-scale=1">
                  <title>%s</title>
                  <style>
                    body { font-family: sans-serif; max-width: 640px; margin: 64px auto; padding: 0 24px; }
                    h1 { font-size: 1.5rem; }
                    p { line-height: 1.7; }
                  </style>
                </head>
                <body>
                  <h1>%s</h1>
                  <p>%s</p>
                </body>
                </html>
                """.formatted(title, title, message);
    }
}
