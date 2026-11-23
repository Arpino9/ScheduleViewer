package com.scheduleviewer.api.controller;

import com.scheduleviewer.domain.entity.ExpenditureEntity;
import com.scheduleviewer.infrastructure.google.drive.DriveService;
import org.springframework.format.annotation.DateTimeFormat;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDate;
import java.util.List;

/**
 * Google Drive (家計簿) コントローラー
 */
@RestController
@RequestMapping("/api/drive")
public class DriveController {

    private final DriveService driveService;

    public DriveController(DriveService driveService) {
        this.driveService = driveService;
    }

    /**
     * 全家計簿データを取得する
     */
    @GetMapping("/expenditure")
    public List<ExpenditureEntity> getAll() {
        return driveService.getAll();
    }

    /**
     * 指定日の家計簿データを取得する
     *
     * @param date 日付 (yyyy-MM-dd)
     */
    @GetMapping("/expenditure/date/{date}")
    public List<ExpenditureEntity> getByDate(
            @PathVariable @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate date) {
        return driveService.getExpenditure(date);
    }

    /**
     * 家計簿データを再読み込みする
     */
    @PostMapping("/expenditure/reload")
    public void reload() throws Exception {
        driveService.load();
    }
}
