package com.scheduleviewer.infrastructure.local;

import com.scheduleviewer.domain.entity.PhotoEntity;
import com.scheduleviewer.infrastructure.config.AppProperties;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Service;

import java.io.IOException;
import java.net.URLEncoder;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.util.List;
import java.util.Set;

/**
 * ローカルファイルシステムの写真を提供するサービス
 * <p>対象フォルダ構造: {basePath}/{yyyy}年/{m}月/{yyyymmdd}/</p>
 */
@Service
public class LocalPhotoService {

    private static final Logger log = LoggerFactory.getLogger(LocalPhotoService.class);

    private static final Set<String> IMAGE_EXTENSIONS = Set.of(
            "jpg", "jpeg", "png", "gif", "webp", "bmp", "heic", "heif", "tiff", "tif"
    );

    private static final DateTimeFormatter DATE_FOLDER_FORMAT = DateTimeFormatter.ofPattern("yyyyMMdd");

    private final AppProperties props;

    public LocalPhotoService(AppProperties props) {
        this.props = props;
    }

    /**
     * 指定日の写真一覧を返す
     *
     * @param date 対象日付
     * @return 写真エンティティのリスト
     */
    public List<PhotoEntity> findByDate(LocalDate date) {
        String basePath = props.getLocalPhotoBasePath();
        if (basePath == null || basePath.isBlank()) {
            log.warn("local-photo-base-path が未設定です");
            return List.of();
        }

        String yearFolder  = date.getYear() + "年";
        String monthFolder = date.getMonthValue() + "月";
        String dateFolder  = date.format(DATE_FOLDER_FORMAT);

        Path dir = Paths.get(basePath, yearFolder, monthFolder, dateFolder);
        if (!Files.isDirectory(dir)) {
            log.debug("写真フォルダが存在しません: {}", dir);
            return List.of();
        }

        try (var stream = Files.list(dir)) {
            return stream
                    .filter(p -> !Files.isDirectory(p))
                    .filter(p -> isImage(p.getFileName().toString()))
                    .sorted()
                    .map(p -> toEntity(p, yearFolder, monthFolder, dateFolder, date))
                    .toList();
        } catch (IOException e) {
            log.error("写真フォルダの読み込みに失敗しました: {}", dir, e);
            return List.of();
        }
    }

    private boolean isImage(String name) {
        int dot = name.lastIndexOf('.');
        if (dot < 0) return false;
        return IMAGE_EXTENSIONS.contains(name.substring(dot + 1).toLowerCase());
    }

    private PhotoEntity toEntity(Path filePath, String yearFolder, String monthFolder,
                                 String dateFolder, LocalDate date) {
        String fileName = filePath.getFileName().toString();
        String url = "/local-photos/"
                + encode(yearFolder) + "/"
                + encode(monthFolder) + "/"
                + dateFolder + "/"
                + encode(fileName);
        return new PhotoEntity(
                fileName,
                date.atStartOfDay(),
                fileName,
                "",
                url,
                guessMime(fileName),
                0,
                0
        );
    }

    private static String encode(String s) {
        return URLEncoder.encode(s, StandardCharsets.UTF_8).replace("+", "%20");
    }

    private static String guessMime(String fileName) {
        int dot = fileName.lastIndexOf('.');
        if (dot < 0) return "image/jpeg";
        return switch (fileName.substring(dot + 1).toLowerCase()) {
            case "png"  -> "image/png";
            case "gif"  -> "image/gif";
            case "webp" -> "image/webp";
            case "bmp"  -> "image/bmp";
            default     -> "image/jpeg";
        };
    }
}
