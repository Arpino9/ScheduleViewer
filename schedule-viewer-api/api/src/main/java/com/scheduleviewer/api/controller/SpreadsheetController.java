package com.scheduleviewer.api.controller;

import com.scheduleviewer.infrastructure.google.spreadsheet.SpreadsheetService;
import org.springframework.web.bind.annotation.*;

import java.util.Map;

/**
 * Google Spreadsheet REST コントローラー
 */
@RestController
@RequestMapping("/api/spreadsheet")
public class SpreadsheetController {

    private final SpreadsheetService spreadsheetService;

    public SpreadsheetController(SpreadsheetService spreadsheetService) {
        this.spreadsheetService = spreadsheetService;
    }

    /**
     * タイトルでサムネイル画像URLを取得する
     * @return {"url": "..."} または {}
     */
    @GetMapping("/thumbnail")
    public Map<String, String> findThumbnail(@RequestParam String title) {
        String url = spreadsheetService.findThumbnailByTitle(title);
        return url != null ? Map.of("url", url) : Map.of();
    }

    /** サムネイルキャッシュを再読み込みする */
    @PostMapping("/thumbnail/reload")
    public void reloadThumbnails() {
        spreadsheetService.reloadThumbnails();
    }

    /**
     * タイトルで概要を取得する (取得(番組)シートから)
     * @return {"caption": "..."} または {}
     */
    @GetMapping("/caption")
    public Map<String, String> findCaption(@RequestParam String title) {
        String caption = spreadsheetService.findCaptionByTitle(title);
        return caption != null ? Map.of("caption", caption) : Map.of();
    }

    /** 概要キャッシュを再読み込みする */
    @PostMapping("/caption/reload")
    public void reloadCaptions() {
        spreadsheetService.reloadCaptions();
    }

    /**
     * タイトルで各話サムネイルURLを取得する (サムネイル(アニメ各話)シートから)
     * @return {"url": "..."} または {}
     */
    @GetMapping("/episode-thumbnail")
    public Map<String, String> findEpisodeThumbnail(@RequestParam String title) {
        String url = spreadsheetService.findEpisodeThumbnailByTitle(title);
        return url != null ? Map.of("url", url) : Map.of();
    }

    /** 各話サムネイルキャッシュを再読み込みする */
    @PostMapping("/episode-thumbnail/reload")
    public void reloadEpisodeThumbnails() {
        spreadsheetService.reloadEpisodeThumbnails();
    }
}
