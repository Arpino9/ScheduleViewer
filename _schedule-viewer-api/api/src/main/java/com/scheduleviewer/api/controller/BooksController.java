package com.scheduleviewer.api.controller;

import com.scheduleviewer.domain.entity.BookEntity;
import com.scheduleviewer.infrastructure.google.books.BooksService;
import org.springframework.format.annotation.DateTimeFormat;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDate;

/**
 * Google Books API コントローラー
 */
@RestController
@RequestMapping("/api/books")
public class BooksController {

    private final BooksService booksService;

    public BooksController(BooksService booksService) {
        this.booksService = booksService;
    }

    /**
     * タイトルで書籍を検索する
     *
     * @param title    書籍タイトル (部分一致)
     * @param readDate 読了日 (省略時は本日)
     * @return 書籍情報
     */
    @GetMapping
    public BookEntity search(
            @RequestParam String title,
            @RequestParam(required = false) @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate readDate)
            throws Exception {

        if (readDate != null) {
            return booksService.findByTitle(title, readDate);
        }
        return booksService.findByTitle(title);
    }
}
