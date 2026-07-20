package com.scheduleviewer.api.controller;

import com.scheduleviewer.domain.entity.PhotoEntity;
import com.scheduleviewer.infrastructure.google.photo.PhotoService;
import org.springframework.format.annotation.DateTimeFormat;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDate;
import java.util.List;

/**
 * Google Photos コントローラー
 *
 * @deprecated Google Photos APIの仕様変更により機能が制限されている
 */
@Deprecated
@RestController
@RequestMapping("/api/photos")
public class PhotoController {

    private final PhotoService photoService;

    public PhotoController(PhotoService photoService) {
        this.photoService = photoService;
    }

    /**
     * 全写真を取得する
     */
    @GetMapping
    public List<PhotoEntity> getAll() {
        return photoService.getAll();
    }

    /**
     * 指定日の写真を取得する
     *
     * @param date 日付 (yyyy-MM-dd)
     */
    @GetMapping("/date/{date}")
    public List<PhotoEntity> getByDate(
            @PathVariable @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate date) {
        return photoService.findByDate(date);
    }

    /**
     * 写真キャッシュを再読み込みする
     */
    @PostMapping("/reload")
    public void reload() throws Exception {
        photoService.load();
    }
}
