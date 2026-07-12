package com.scheduleviewer.infrastructure.google.books;

import com.google.api.services.books.v1.Books;
import com.google.api.services.books.v1.BooksScopes;
import com.google.api.services.books.v1.model.Volume;
import com.google.api.services.books.v1.model.Volumes;
import com.scheduleviewer.domain.entity.BookEntity;
import com.scheduleviewer.infrastructure.google.GoogleAuthService;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Service;

import java.time.LocalDate;
import java.util.List;

/**
 * Google Books API サービス
 * <p>.NET版の BooksReader に相当</p>
 */
@Service
public class BooksService {

    private static final Logger log = LoggerFactory.getLogger(BooksService.class);
    private static final List<String> SCOPES = List.of(BooksScopes.BOOKS);

    private final GoogleAuthService authService;

    public BooksService(GoogleAuthService authService) {
        this.authService = authService;
    }

    /**
     * タイトルで書籍を検索する (読了日 = 本日)
     */
    public BookEntity findByTitle(String title) throws Exception {
        return findByTitle(title, LocalDate.now());
    }

    /**
     * タイトルで書籍を検索する
     *
     * @param title    タイトル
     * @param readDate 読了日
     */
    public BookEntity findByTitle(String title, LocalDate readDate) throws Exception {
        var credential = authService.authorize(SCOPES, "token_Books");
        var service = new Books.Builder(
                authService.newTransport(),
                authService.getJsonFactory(),
                credential)
                .setApplicationName(authService.getApplicationName())
                .build();

        Volumes volumes = service.volumes().list(title).execute();

        if (volumes.getItems() == null || volumes.getItems().isEmpty()) {
            log.info("書籍が見つかりません: title={}", title);
            return BookEntity.EMPTY;
        }

        Volume book = volumes.getItems().get(0);
        Volume.VolumeInfo info = book.getVolumeInfo();

        String author       = info.getAuthors()     != null && !info.getAuthors().isEmpty()
                              ? info.getAuthors().get(0) : "";
        String publisher    = info.getPublisher()   != null ? info.getPublisher()   : "";
        String releasedDate = info.getPublishedDate() != null ? info.getPublishedDate() : "";
        String type         = info.getCategories()  != null && !info.getCategories().isEmpty()
                              ? info.getCategories().get(0) : "";
        String caption      = info.getDescription() != null ? info.getDescription() : "";
        String rating       = info.getRatingsCount() != null ? info.getRatingsCount().toString() : "";
        String thumbnail    = info.getImageLinks()  != null ? info.getImageLinks().getThumbnail() : "";

        String isbn13 = getIsbn(service, book.getId(), "ISBN_13");
        String isbn10 = getIsbn(service, book.getId(), "ISBN_10");

        return new BookEntity(title, readDate, author, publisher, releasedDate,
                type, isbn10, isbn13, caption, thumbnail, rating);
    }

    /** 指定した種類のISBNコードを取得する */
    private String getIsbn(Books service, String volumeId, String isbnType) {
        try {
            var detail = service.volumes().get(volumeId).execute();
            if (detail.getVolumeInfo().getIndustryIdentifiers() == null) return "";
            return detail.getVolumeInfo().getIndustryIdentifiers().stream()
                    .filter(id -> isbnType.equals(id.getType()))
                    .map(Volume.VolumeInfo.IndustryIdentifiers::getIdentifier)
                    .findFirst()
                    .orElse("");
        } catch (Exception e) {
            log.warn("ISBN取得失敗: volumeId={}, type={}", volumeId, isbnType, e);
            return "";
        }
    }
}
