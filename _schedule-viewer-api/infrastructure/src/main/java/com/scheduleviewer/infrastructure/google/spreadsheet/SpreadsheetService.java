package com.scheduleviewer.infrastructure.google.spreadsheet;

import com.google.api.services.sheets.v4.Sheets;
import com.google.api.services.sheets.v4.SheetsScopes;
import com.scheduleviewer.infrastructure.google.GoogleAuthService;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Service;

import java.util.Collections;
import java.util.List;
import java.util.Map;
import java.util.concurrent.ConcurrentHashMap;

/**
 * Google Spreadsheet 読み込みサービス
 * <p>.NET版の SheetReader に相当</p>
 */
@Service
public class SpreadsheetService {

    private static final Logger log = LoggerFactory.getLogger(SpreadsheetService.class);
    private static final List<String> SCOPES = List.of(SheetsScopes.SPREADSHEETS);

    /** Tasks一覧スプレッドシートID */
    public static final String TASK_SHEET_ID  = "1tc5uFTh09PBVVnV2OYmGZ3svY6C-6SwCAF6KIUO8l9c";

    /** サムネイル一覧スプレッドシートID */
    public static final String THUMB_SHEET_ID = "191fTeVKET2K5yZ6trFewRV3_8GJ80s8qC92-NtgNvv0";

    private final GoogleAuthService authService;

    /** タイトル→サムネイルURLの遅延ロードキャッシュ */
    private volatile Map<String, String> thumbnailCache = null;

    /** タイトル→概要の遅延ロードキャッシュ */
    private volatile Map<String, String> captionCache = null;

    /** タイトル→各話サムネイルURLの遅延ロードキャッシュ */
    private volatile Map<String, String> episodeThumbnailCache = null;

    public SpreadsheetService(GoogleAuthService authService) {
        this.authService = authService;
    }

    /**
     * スプレッドシートの指定範囲を読み込む
     *
     * @param sheetId    スプレッドシートID
     * @param sheetRange 範囲 (例: "タスク一覧!A:B")
     * @return セルの値リスト
     */
    /** OAuth認証URLを取得する。認証済みの場合は null を返す。 */
    public String getAuthUrl() throws Exception {
        return authService.startAuthFlowAndGetUrl(SCOPES, "token_Sheets", null);
    }

    public List<List<Object>> read(String sheetId, String sheetRange) {
        try {
            var credential = authService.authorize(SCOPES, "token_Sheets");
            var service = new Sheets.Builder(
                    authService.newTransport(),
                    authService.getJsonFactory(),
                    credential)
                    .setApplicationName(authService.getApplicationName())
                    .build();

            var response = service.spreadsheets().values()
                    .get(sheetId, sheetRange)
                    .execute();

            var values = response.getValues();
            if (values == null) return Collections.emptyList();

            // List<List<Object>> に変換
            return values.stream()
                    .map(row -> (List<Object>) row)
                    .toList();

        } catch (Exception e) {
            log.error("スプレッドシート読み込み失敗: sheetId={}, range={}", sheetId, sheetRange, e);
            return Collections.emptyList();
        }
    }

    /** Tasks一覧を読み込む */
    public List<List<Object>> readTasks() {
        return read(TASK_SHEET_ID, "タスク一覧!A:B");
    }

    /** サムネイル一覧を読み込む (B=タイトル, C=画像URL) */
    public List<List<Object>> readThumbnails() {
        return read(THUMB_SHEET_ID, "サムネイル!B:C");
    }

    /**
     * タイトルでサムネイルURLを検索する (部分一致、遅延ロード)
     * @return 画像URL。見つからなければ null
     */
    public String findThumbnailByTitle(String title) {
        if (thumbnailCache == null) {
            synchronized (this) {
                if (thumbnailCache == null) {
                    Map<String, String> cache = new ConcurrentHashMap<>();
                    for (var row : readThumbnails()) {
                        if (row.size() >= 2 && row.get(0) != null && row.get(1) != null) {
                            cache.put(row.get(0).toString().trim(), row.get(1).toString().trim());
                        }
                    }
                    thumbnailCache = cache;
                    log.info("サムネイルキャッシュ構築: {}件", cache.size());
                }
            }
        }
        return searchByTitle(thumbnailCache, title);
    }

    /** サムネイルキャッシュを破棄して再読み込みさせる */
    public void reloadThumbnails() {
        thumbnailCache = null;
    }

    /** 取得(番組)シートを読み込む */
    public List<List<Object>> readAnimePrograms() {
        return read(THUMB_SHEET_ID, "'取得(番組)'!A:Z");
    }

    /**
     * タイトルで概要を検索する (ヘッダー行からタイトル列・概要列を自動検出、遅延ロード)
     * @return 概要テキスト。見つからなければ null
     */
    public String findCaptionByTitle(String title) {
        if (captionCache == null) {
            synchronized (this) {
                if (captionCache == null) {
                    captionCache = loadCaptionCache();
                }
            }
        }
        return searchByTitle(captionCache, title);
    }

    private Map<String, String> loadCaptionCache() {
        var rows = readAnimePrograms();
        if (rows.isEmpty()) return Collections.emptyMap();

        // 1行目をヘッダーとしてタイトル列・概要列のインデックスを特定
        var headers = rows.get(0);
        int titleIdx   = -1;
        int captionIdx = -1;
        for (int i = 0; i < headers.size(); i++) {
            String h = headers.get(i).toString().trim();
            if (h.equals("タイトル"))  titleIdx   = i;
            if (h.equals("概要"))      captionIdx = i;
        }
        if (titleIdx < 0 || captionIdx < 0) {
            log.warn("取得(番組)シートでタイトル列または概要列が見つかりません headers={}", headers);
            return Collections.emptyMap();
        }

        Map<String, String> cache = new ConcurrentHashMap<>();
        final int ti = titleIdx, ci = captionIdx;
        for (int r = 1; r < rows.size(); r++) {
            var row = rows.get(r);
            if (row.size() > ti && row.size() > ci
                    && row.get(ti) != null && row.get(ci) != null) {
                String t = row.get(ti).toString().trim();
                String c = row.get(ci).toString().trim();
                if (!t.isEmpty() && !c.isEmpty()) cache.put(t, c);
            }
        }
        log.info("概要キャッシュ構築: {}件", cache.size());
        return cache;
    }

    /** 概要キャッシュを破棄して再読み込みさせる */
    public void reloadCaptions() {
        captionCache = null;
    }

    /** 各話サムネイルシートを読み込む (A=名称, B=URL) */
    public List<List<Object>> readEpisodeThumbnails() {
        return read(THUMB_SHEET_ID, "'サムネイル(アニメ各話)'!A:B");
    }

    /**
     * タイトルで各話サムネイルURLを検索する (完全一致優先、遅延ロード)
     * @return 画像URL。見つからなければ null
     */
    public String findEpisodeThumbnailByTitle(String title) {
        if (episodeThumbnailCache == null) {
            synchronized (this) {
                if (episodeThumbnailCache == null) {
                    Map<String, String> cache = new ConcurrentHashMap<>();
                    var rows = readEpisodeThumbnails();
                    // 1行目はヘッダーなのでスキップ
                    for (int i = 1; i < rows.size(); i++) {
                        var row = rows.get(i);
                        if (row.size() >= 2 && row.get(0) != null && row.get(1) != null) {
                            String key = row.get(0).toString().trim();
                            String url = row.get(1).toString().trim();
                            if (!key.isEmpty() && !url.isEmpty()) cache.put(key, url);
                        }
                    }
                    episodeThumbnailCache = cache;
                    log.info("各話サムネイルキャッシュ構築: {}件", cache.size());
                }
            }
        }
        return searchByTitle(episodeThumbnailCache, title);
    }

    /** 各話サムネイルキャッシュを破棄して再読み込みさせる */
    public void reloadEpisodeThumbnails() {
        episodeThumbnailCache = null;
    }

    /** 完全一致 → 前方一致 → 部分一致の順で検索する共通ロジック。_ / 全角スペース / 半角スペースは同一視する */
    private String searchByTitle(Map<String, String> cache, String title) {
        String norm = normalizeTitle(title);
        return cache.entrySet().stream()
                .filter(e -> normalizeTitle(e.getKey()).equals(norm))
                .map(Map.Entry::getValue)
                .findFirst()
                .or(() -> cache.entrySet().stream()
                        .filter(e -> { String k = normalizeTitle(e.getKey());
                                       return k.startsWith(norm) || norm.startsWith(k); })
                        .map(Map.Entry::getValue)
                        .findFirst())
                .or(() -> cache.entrySet().stream()
                        .filter(e -> { String k = normalizeTitle(e.getKey());
                                       return k.contains(norm) || norm.contains(k); })
                        .map(Map.Entry::getValue)
                        .findFirst())
                .orElse(null);
    }

    /** _ と全角スペースを半角スペースに統一する */
    private static String normalizeTitle(String s) {
        return s.replace('_', ' ').replace('\u3000', ' ');
    }
}
