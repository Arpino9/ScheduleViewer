package com.scheduleviewer.api;

import com.google.api.services.calendar.CalendarScopes;
import com.google.api.services.drive.DriveScopes;
import com.google.api.services.sheets.v4.SheetsScopes;
import com.google.api.services.tasks.TasksScopes;
import com.scheduleviewer.infrastructure.google.GoogleAuthService;
import com.scheduleviewer.infrastructure.google.calendar.CalendarService;
import com.scheduleviewer.infrastructure.google.drive.DriveService;
import com.scheduleviewer.infrastructure.google.tasks.TasksService;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.boot.ApplicationArguments;
import org.springframework.boot.ApplicationRunner;
import org.springframework.stereotype.Component;

import java.awt.*;
import java.net.URI;
import java.util.List;

/**
 * 起動時 Google 一括認証 + ブラウザ自動オープン
 * <p>
 * トークン未設定のサービスがある場合、ブラウザを開いて OAuth 認証を行ってから
 * データを読み込む。全サービス認証済みの場合は即座にブラウザを開く。
 * </p>
 */
@Component
public class StartupInitializer implements ApplicationRunner {

    private static final Logger log = LoggerFactory.getLogger(StartupInitializer.class);
    private static final String APP_URL = "http://localhost:9080/";

    private final GoogleAuthService authService;
    private final CalendarService   calendarService;
    private final TasksService      tasksService;
    private final DriveService      driveService;

    public StartupInitializer(
            GoogleAuthService authService,
            CalendarService   calendarService,
            TasksService      tasksService,
            DriveService      driveService) {
        this.authService     = authService;
        this.calendarService = calendarService;
        this.tasksService    = tasksService;
        this.driveService    = driveService;
    }

    @Override
    public void run(ApplicationArguments args) {
        Thread.ofVirtual().start(this::initAll);
    }

    private void initAll() {
        boolean calMissing    = !authService.hasToken("token_Calendar");
        boolean sheetsMissing = !authService.hasToken("token_Sheets");
        boolean tasksMissing  = !authService.hasToken("token_Tasks");
        boolean driveMissing  = !authService.hasToken("token_Drive");

        if (!calMissing && !sheetsMissing && !tasksMissing && !driveMissing) {
            // 全サービス認証済み → @PostConstruct がデータ読み込みを担当
            log.info("全 Google サービス認証済み。ブラウザを開きます");
            openBrowser(APP_URL);
            return;
        }

        log.info("=== 未認証の Google サービスを検出 → 一括認証を開始します ===");

        // Calendar 認証
        boolean calOk = !calMissing || ensureAuth(List.of(CalendarScopes.CALENDAR), "token_Calendar");

        // Sheets 認証 (Tasks が依存するため先に実行)
        boolean sheetsOk = !sheetsMissing || ensureAuth(List.of(SheetsScopes.SPREADSHEETS), "token_Sheets");

        // Tasks 認証
        boolean tasksOk = !tasksMissing || ensureAuth(List.of(TasksScopes.TASKS), "token_Tasks");

        // Drive 認証
        boolean driveOk = !driveMissing || ensureAuth(List.of(DriveScopes.DRIVE_READONLY), "token_Drive");

        // 認証が完了したサービスのデータを読み込む
        if (calMissing && calOk) {
            try {
                log.info("[Calendar] データ読み込み開始");
                calendarService.load();
            } catch (Exception e) {
                log.error("[Calendar] データ読み込み失敗", e);
            }
        }

        // Sheets か Tasks の一方でも認証が必要だった場合は Tasks を再読み込み
        if ((sheetsMissing || tasksMissing) && sheetsOk && tasksOk) {
            try {
                log.info("[Tasks] データ読み込み開始");
                tasksService.load();
            } catch (Exception e) {
                log.error("[Tasks] データ読み込み失敗", e);
            }
        }

        if (driveMissing && driveOk) {
            try {
                log.info("[Drive] データ読み込み開始");
                driveService.load();
            } catch (Exception e) {
                log.error("[Drive] データ読み込み失敗", e);
            }
        }

        log.info("=== 起動時一括認証・初期化完了 ===");
        openBrowser(APP_URL);
    }

    /** 指定サービスの OAuth 認証を実行する (既にトークンがあれば即時返却) */
    private boolean ensureAuth(List<String> scopes, String tokenFolder) {
        log.info("[{}] 認証を開始します (ブラウザが開きます)", tokenFolder);
        try {
            authService.authorize(scopes, tokenFolder);
            log.info("[{}] 認証完了", tokenFolder);
            return true;
        } catch (Exception e) {
            log.error("[{}] 認証失敗: {}", tokenFolder, e.getMessage());
            return false;
        }
    }

    private void openBrowser(String url) {
	    try {
	        if (Desktop.isDesktopSupported() && Desktop.getDesktop().isSupported(Desktop.Action.BROWSE)) {
	            Desktop.getDesktop().browse(new URI(url));
	            log.info("ブラウザを開きました: {}", url);
	        } else {
	            // headless環境でも動作するフォールバック (Windows)
	            Runtime.getRuntime().exec(new String[]{"cmd", "/c", "start", url});
	            log.info("ブラウザを開きました (fallback): {}", url);
	        }
	    } catch (Exception e) {
	        log.warn("ブラウザを自動で開けませんでした: {}", e.getMessage());
	    }
	}
}
