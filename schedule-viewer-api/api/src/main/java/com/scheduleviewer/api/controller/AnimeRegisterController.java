package com.scheduleviewer.api.controller;

import com.scheduleviewer.infrastructure.google.calendar.CalendarService;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDate;
import java.util.Map;

/**
 * アニメ視聴記録の登録コントローラー
 * Google Calendar への全日イベント登録のみ行う
 */
@RestController
@RequestMapping("/api/anime")
public class AnimeRegisterController {

    private static final Logger log = LoggerFactory.getLogger(AnimeRegisterController.class);

    private final CalendarService calendarService;

    public AnimeRegisterController(CalendarService calendarService) {
        this.calendarService = calendarService;
    }

    @PostMapping("/register")
    public Map<String, Object> register(@RequestBody AnimeRegisterRequest req) {
        try {
            LocalDate date = LocalDate.parse(req.date());

            calendarService.createAnimeEvent(
                    date, req.title(), req.episode(),
                    req.subtitle(), req.service(), req.summary());

            String label = req.title() + " 第" + req.episode() + "話";
            log.info("アニメ視聴登録完了: {}", label);
            return Map.of("status", "ok", "message", label + " を登録しました");

        } catch (Exception e) {
            log.error("アニメ視聴登録エラー", e);
            String msg = e.getMessage() != null ? e.getMessage() : "登録に失敗しました";
            return Map.of("status", "error", "message", msg);
        }
    }

    public record AnimeRegisterRequest(
            String date,
            String title,
            int episode,
            String subtitle,
            String service,
            String summary
    ) {}
}
