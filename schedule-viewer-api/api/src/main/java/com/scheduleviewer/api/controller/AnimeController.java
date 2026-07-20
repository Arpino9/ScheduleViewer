package com.scheduleviewer.api.controller;

import com.scheduleviewer.domain.entity.AnimeEntity;
import com.scheduleviewer.infrastructure.annict.AnnictService;
import org.springframework.web.bind.annotation.*;

import java.util.List;

/**
 * アニメ情報 REST コントローラー
 */
@RestController
@RequestMapping("/api/anime")
public class AnimeController {

    private final AnnictService annictService;

    public AnimeController(AnnictService annictService) {
        this.annictService = annictService;
    }

    /**
     * タイトルでアニメ情報を検索する
     *
     * @param title     タイトル
     * @param first     取得件数 (デフォルト: 5)
     * @param castFirst キャスト取得件数 (デフォルト: 10)
     */
    @GetMapping
    public List<AnimeEntity> search(
            @RequestParam String title,
            @RequestParam(defaultValue = "5")  int first,
            @RequestParam(defaultValue = "10") int castFirst) throws Exception {
        return annictService.fetch(title, first, castFirst);
    }
}
