package com.scheduleviewer.api.controller;

import com.scheduleviewer.domain.entity.PhotoEntity;
import com.scheduleviewer.infrastructure.google.photo.PhotoService;
import com.scheduleviewer.infrastructure.local.LocalPhotoService;
import org.springframework.format.annotation.DateTimeFormat;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDate;
import java.util.List;

/**
 * 写真コントローラー
 * <p>Google Photos API版は仕様変更により非推奨。ローカルフォルダ版を使用する。</p>
 */
@RestController
@RequestMapping("/api/photos")
public class PhotoController {

    private final PhotoService photoService;
    private final LocalPhotoService localPhotoService;

    public PhotoController(PhotoService photoService, LocalPhotoService localPhotoService) {
        this.photoService = photoService;
        this.localPhotoService = localPhotoService;
    }

    /**
     * 全写真を取得する (Google Photos API版 - 非推奨)
     *
     * @deprecated Google Photos APIの仕様変更により機能が制限されている
     */
    @Deprecated
    @GetMapping
    public List<PhotoEntity> getAll() {
        return photoService.getAll();
    }

    /**
     * 指定日の写真を取得する (Google Photos API版 - 非推奨)
     *
     * @deprecated ローカル写真 /api/photos/local/date/{date} を使用すること
     */
    @Deprecated
    @GetMapping("/date/{date}")
    public List<PhotoEntity> getByDate(
            @PathVariable @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate date) {
        return photoService.findByDate(date);
    }

    /**
     * 写真キャッシュを再読み込みする (Google Photos API版 - 非推奨)
     */
    @Deprecated
    @PostMapping("/reload")
    public void reload() throws Exception {
        photoService.load();
    }

    /**
     * 指定日のローカル写真を取得する
     * <p>対象: C:/Users/okaji/Desktop/Google Photo/{yyyy}年/{m}月/{yyyymmdd}/</p>
     *
     * @param date 日付 (yyyy-MM-dd)
     */
    @GetMapping("/local/date/{date}")
    public List<PhotoEntity> getLocalByDate(
            @PathVariable @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate date) {
        return localPhotoService.findByDate(date);
    }
}
