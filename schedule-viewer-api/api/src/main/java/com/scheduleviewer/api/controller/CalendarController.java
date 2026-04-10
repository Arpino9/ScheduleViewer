package com.scheduleviewer.api.controller;

import com.scheduleviewer.domain.entity.CalendarEventsEntity;
import com.scheduleviewer.infrastructure.google.calendar.CalendarService;
import org.springframework.format.annotation.DateTimeFormat;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDate;
import java.util.List;

/**
 * Google Calendar REST コントローラー
 */
@RestController
@RequestMapping("/api/calendar")
public class CalendarController {

    private final CalendarService calendarService;

    public CalendarController(CalendarService calendarService) {
        this.calendarService = calendarService;
    }

    /** 読込状態を返す */
    @GetMapping("/status")
    public boolean isLoading() {
        return calendarService.isLoading();
    }

    /** カレンダーを再読み込みする */
    @PostMapping("/reload")
    public void reload() throws Exception {
        calendarService.load();
    }

    /** 日付でアニメイベント（【視聴先】を含む全日イベント）を取得する */
    @GetMapping("/anime")
    public List<CalendarEventsEntity> findAnimeByDate(
            @RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate date) {
        return calendarService.findAnimeByDate(date);
    }

    /** 日付でイベントを取得する */
    @GetMapping
    public List<CalendarEventsEntity> findByDate(
            @RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate date) {
        return calendarService.findByDate(date);
    }

    /** 日付範囲でイベントを取得する */
    @GetMapping("/range")
    public List<CalendarEventsEntity> findByDateRange(
            @RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate startDate,
            @RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate endDate) {
        return calendarService.findByDate(startDate, endDate);
    }

    /** タイトルで検索する */
    @GetMapping("/search/title")
    public List<CalendarEventsEntity> findByTitle(
            @RequestParam String title,
            @RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate startDate,
            @RequestParam(required = false) @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate endDate) {
        if (endDate != null) {
            return calendarService.findByTitle(title, startDate, endDate);
        }
        return calendarService.findByTitle(title, startDate);
    }

    /** 住所で検索する */
    @GetMapping("/search/address")
    public List<CalendarEventsEntity> findByAddress(
            @RequestParam String address,
            @RequestParam(required = false) @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate startDate,
            @RequestParam(required = false) @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate endDate) {
        if (startDate != null && endDate != null) {
            return calendarService.findByAddress(address, startDate, endDate);
        }
        return calendarService.findByAddress(address);
    }

    /** キーワード検索 (タイトル・場所・説明の部分一致、最大10件) */
    @GetMapping("/search")
    public List<CalendarEventsEntity> search(@RequestParam String q) {
        return calendarService.search(q);
    }

    /** 説明で検索する */
    @GetMapping("/search/description")
    public List<CalendarEventsEntity> findByDescription(
            @RequestParam String description,
            @RequestParam(required = false) @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate startDate,
            @RequestParam(required = false) @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate endDate) {
        if (startDate != null && endDate != null) {
            return calendarService.findByDescription(description, startDate, endDate);
        }
        return calendarService.findByDescription(description);
    }
}
