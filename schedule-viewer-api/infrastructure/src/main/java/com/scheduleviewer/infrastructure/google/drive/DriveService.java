package com.scheduleviewer.infrastructure.google.drive;

import com.google.api.services.drive.Drive;
import com.google.api.services.drive.DriveScopes;
import com.google.api.services.drive.model.File;
import com.scheduleviewer.domain.entity.ExpenditureEntity;
import com.scheduleviewer.infrastructure.config.AppProperties;
import com.scheduleviewer.infrastructure.google.GoogleAuthService;
import jakarta.annotation.PostConstruct;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Service;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.nio.charset.StandardCharsets;
import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

/**
 * Google Drive サービス
 * <p>.NET版の DriveReader に相当</p>
 * <p>指定フォルダ内のCSVファイルを読み込み、家計簿データを取得する</p>
 */
@Service
public class DriveService {

    private static final Logger log = LoggerFactory.getLogger(DriveService.class);
    private static final List<String> SCOPES = List.of(DriveScopes.DRIVE_READONLY);

    private final GoogleAuthService authService;
    private final AppProperties props;

    private final List<ExpenditureEntity> expenditures = new ArrayList<>();

    public DriveService(GoogleAuthService authService, AppProperties props) {
        this.authService = authService;
        this.props       = props;
    }

    /** 起動時に非同期で家計簿データを読み込む (トークンが存在する場合のみ) */
    @PostConstruct
    public void initializeAsync() {
        if (!authService.hasToken("token_Drive")) {
            log.info("Google Drive トークンが未設定のため起動時読み込みをスキップします");
            return;
        }
        Thread.ofVirtual().start(() -> {
            try {
                load();
            } catch (Exception e) {
                log.error("Google Driveの読み込みに失敗しました", e);
            }
        });
    }

    /** OAuth認証URLを取得する。認証完了後に自動でデータを読み込む。認証済みの場合は null を返す。 */
    public String getAuthUrl() throws Exception {
        return authService.startAuthFlowAndGetUrl(SCOPES, "token_Drive", () -> {
            try { load(); } catch (Exception e) { log.error("Drive reload after auth failed", e); }
        });
    }

    /** CSVファイルを全件読み込んでキャッシュする */
    public synchronized void load() throws Exception {
        expenditures.clear();

        var credential = authService.authorize(SCOPES, "token_Drive");
        var service = new Drive.Builder(
                authService.newTransport(),
                authService.getJsonFactory(),
                credential)
                .setApplicationName(authService.getApplicationName())
                .build();

        String folderId = props.getGoogle().getDriveFolderId();
        List<File> files = listFilesInFolder(service, folderId);

        log.info("Google Drive CSVファイル数: {}", files.stream().filter(f -> f.getName().endsWith(".csv")).count());

        Set<String> seenKeys = new HashSet<>();
        for (File file : files) {
            if (!file.getName().endsWith(".csv")) continue;

            String content = downloadFileContent(service, file.getId());
            String[] lines = content.split("\n");
            int added = 0, skipped = 0;

            // 1行目はヘッダーなのでスキップ
            for (int i = 1; i < lines.length; i++) {
                String line = lines[i].trim();
                if (line.isEmpty()) continue;
                try {
                    ExpenditureEntity entity = parseCsvLine(line);
                    if (entity == null) continue;
                    // IDで重複排除 (複数CSVファイルに同一エントリが含まれる場合に対応)
                    if (!seenKeys.add(entity.getId())) {
                        skipped++;
                        continue;
                    }
                    expenditures.add(entity);
                    added++;
                } catch (Exception e) {
                    log.warn("CSV行の解析失敗 (line={}): {}", i, line, e);
                }
            }
            log.info("  {} → 追加={}, スキップ={}", file.getName(), added, skipped);
        }

        log.info("Google Drive読み込み完了: {}件", expenditures.size());
    }

    /** 指定日の家計簿データを取得する */
    public List<ExpenditureEntity> getExpenditure(LocalDate date) {
        return expenditures.stream()
                .filter(e -> e.getDate().toLocalDate().equals(date))
                .toList();
    }

    /** 全家計簿データを取得する */
    public List<ExpenditureEntity> getAll() {
        return List.copyOf(expenditures);
    }

    /** フォルダ内のファイル一覧を取得する */
    private List<File> listFilesInFolder(Drive service, String folderId) throws IOException {
        var request = service.files().list()
                .setQ("'" + folderId + "' in parents and trashed=false")
                .setFields("nextPageToken, files(id, name)");

        var response = request.execute();
        if (response.getFiles() == null || response.getFiles().isEmpty()) {
            log.warn("Google Driveのフォルダが空です: folderId={}", folderId);
            return List.of();
        }
        return response.getFiles();
    }

    /** ファイルの内容をダウンロードして文字列で返す */
    private String downloadFileContent(Drive service, String fileId) throws IOException {
        var outputStream = new ByteArrayOutputStream();
        service.files().get(fileId).executeMediaAndDownloadTo(outputStream);
        return outputStream.toString(StandardCharsets.UTF_8);
    }

    /**
     * CSV行をパースして ExpenditureEntity を生成する
     * <p>フォーマット: canCalc,date,itemName,price,financialInstitutions,
     *                  categoryLarge,categoryMiddle,memo,change,id</p>
     */
    private ExpenditureEntity parseCsvLine(String line) {
        // クォートを考慮した簡易CSVパース
        String[] cols = splitCsv(line);
        if (cols.length < 10) {
            log.debug("カラム数不足: {}", line);
            return null;
        }

        boolean canCalc    = "1".equals(cols[0].trim());
        LocalDateTime date = parseDate(cols[1].trim());
        String itemName    = cols[2].trim();
        long price         = parseLong(cols[3].trim());
        String fi          = cols[4].trim();
        String catLarge    = cols[5].trim();
        String catMiddle   = cols[6].trim();
        String memo        = cols[7].trim();
        boolean change     = "1".equals(cols[8].trim());
        String id          = cols[9].trim();

        return new ExpenditureEntity(id, canCalc, date, itemName, price, fi, catLarge, catMiddle, memo, change);
    }

    /** 簡易CSVスプリット (ダブルクォート対応) */
    private String[] splitCsv(String line) {
        List<String> result = new ArrayList<>();
        StringBuilder sb = new StringBuilder();
        boolean inQuotes = false;

        for (char c : line.toCharArray()) {
            if (c == '"') {
                inQuotes = !inQuotes;
            } else if (c == ',' && !inQuotes) {
                result.add(sb.toString());
                sb.setLength(0);
            } else {
                sb.append(c);
            }
        }
        result.add(sb.toString());
        return result.toArray(new String[0]);
    }

    private LocalDateTime parseDate(String s) {
        try {
            return LocalDate.parse(s, DateTimeFormatter.ofPattern("yyyy/M/d")).atStartOfDay();
        } catch (Exception e) {
            try {
                return LocalDateTime.parse(s);
            } catch (Exception e2) {
                return LocalDateTime.now();
            }
        }
    }

    private long parseLong(String s) {
        try {
            return Long.parseLong(s.replace(",", ""));
        } catch (NumberFormatException e) {
            return 0L;
        }
    }
}
