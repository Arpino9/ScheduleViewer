package com.scheduleviewer.api.controller;

import com.scheduleviewer.infrastructure.fitbit.FitbitAuthService;
import com.scheduleviewer.infrastructure.fitbit.FitbitTokenStore;
import com.scheduleviewer.infrastructure.google.GoogleAuthService;
import com.scheduleviewer.infrastructure.google.calendar.CalendarService;
import com.scheduleviewer.infrastructure.google.drive.DriveService;
import com.scheduleviewer.infrastructure.google.photo.PhotoService;
import com.scheduleviewer.infrastructure.google.spreadsheet.SpreadsheetService;
import com.scheduleviewer.infrastructure.google.tasks.TasksService;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
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

    private final GoogleAuthService   authService;
    private final CalendarService     calendarService;
    private final TasksService        tasksService;
    private final DriveService        driveService;
    private final PhotoService        photoService;
    private final SpreadsheetService  spreadsheetService;
    private final FitbitAuthService   fitbitAuthService;
    private final FitbitTokenStore    fitbitTokenStore;

    public AuthController(
            GoogleAuthService   authService,
            CalendarService     calendarService,
            TasksService        tasksService,
            DriveService        driveService,
            PhotoService        photoService,
            SpreadsheetService  spreadsheetService,
            FitbitAuthService   fitbitAuthService,
            FitbitTokenStore    fitbitTokenStore) {
        this.authService        = authService;
        this.calendarService    = calendarService;
        this.tasksService       = tasksService;
        this.driveService       = driveService;
        this.photoService       = photoService;
        this.spreadsheetService = spreadsheetService;
        this.fitbitAuthService  = fitbitAuthService;
        this.fitbitTokenStore   = fitbitTokenStore;
    }

    /**
     * 各サービスの認証状態を返す
     * <p>true = トークンあり (認証済み)</p>
     */
    @GetMapping("/status")
    public Map<String, Boolean> status() {
        Map<String, Boolean> result = new LinkedHashMap<>();
        result.put("calendar", authService.hasToken("token_Calendar"));
        result.put("tasks",    authService.hasToken("token_Tasks"));
        result.put("drive",    authService.hasToken("token_Drive"));
        result.put("photos",   authService.hasToken("token_Photos"));
        result.put("sheets",   authService.hasToken("token_Sheets"));
        result.put("fitbit",   fitbitTokenStore.hasToken() && !fitbitTokenStore.isExpired());
        return result;
    }

    /**
     * 指定サービスの Google OAuth を実行し、認証後にデータを読み込む
     * <p>ブラウザが開くので、同一マシンで実行すること</p>
     *
     * @param service calendar | tasks | drive | photos
     */
    @PostMapping("/google/{service}")
    public Map<String, Object> authorizeGoogle(@PathVariable String service) throws Exception {
        String url = switch (service) {
            case "calendar" -> calendarService.getAuthUrl();
            case "tasks"    -> tasksService.getAuthUrl();
            case "drive"    -> driveService.getAuthUrl();
            case "photos"   -> photoService.getAuthUrl();
            case "sheets"   -> spreadsheetService.getAuthUrl();
            case "fitbit"   -> fitbitAuthService.initialize();
            default -> { log.warn("不明なサービス: {}", service); yield null; }
        };

        if (url == null) {
            return Map.of("status", "already_authorized",
                          "message", service + " はすでに認証済みです。");
        }
        log.info("認証URL取得: {} -> {}", service, url);
        return Map.of("status", "pending", "url", url,
                      "message", service + " の認証URLを取得しました。");
    }

    /**
     * 全サービスを一括認証 (各URLを返す)
     */
    @PostMapping("/google/all")
    public Map<String, Object> authorizeAll() throws Exception {
        Map<String, Object> result = new java.util.LinkedHashMap<>();
        for (String svc : new String[]{"calendar", "tasks", "drive", "photos"}) {
            result.put(svc, authorizeGoogle(svc));
        }
        return result;
    }
}
