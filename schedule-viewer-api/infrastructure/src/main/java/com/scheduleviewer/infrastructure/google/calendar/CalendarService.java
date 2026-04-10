package com.scheduleviewer.infrastructure.google.calendar;

import com.google.api.services.calendar.Calendar;
import com.google.api.services.calendar.CalendarScopes;
import com.google.api.services.calendar.model.Event;
import com.google.api.services.calendar.model.Events;
import com.scheduleviewer.domain.entity.AttachmentEntity;
import com.scheduleviewer.domain.entity.CalendarEventsEntity;
import com.scheduleviewer.infrastructure.config.AppProperties;
import com.scheduleviewer.infrastructure.google.GoogleAuthService;
import jakarta.annotation.PostConstruct;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Service;

import java.time.Duration;
import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.ZoneId;
import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.atomic.AtomicBoolean;

/**
 * Google Calendar 読み込みサービス
 * <p>.NET版の CalendarReader に相当</p>
 */
@Service
public class CalendarService {

    private static final Logger log = LoggerFactory.getLogger(CalendarService.class);
    private static final List<String> SCOPES = List.of(CalendarScopes.CALENDAR_READONLY);

    private final GoogleAuthService authService;
    private final AppProperties props;

    private final List<CalendarEventsEntity> calendarEvents = new ArrayList<>();
    private final AtomicBoolean loading = new AtomicBoolean(false);

    public CalendarService(GoogleAuthService authService, AppProperties props) {
        this.authService = authService;
        this.props = props;
    }

    /** 起動時に非同期でカレンダーを読み込む (トークンが存在する場合のみ) */
    @PostConstruct
    public void initializeAsync() {
        if (!authService.hasToken("token_Calendar")) {
            log.info("Google Calendar トークンが未設定のため起動時読み込みをスキップします");
            return;
        }
        Thread.ofVirtual().start(() -> {
            try {
                load();
            } catch (Exception e) {
                log.error("カレンダーの読み込みに失敗しました", e);
            }
        });
    }

    /** OAuth認証URLを取得する。認証完了後に自動でデータを読み込む。認証済みの場合は null を返す。 */
    public String getAuthUrl() throws Exception {
        return authService.startAuthFlowAndGetUrl(SCOPES, "token_Calendar", () -> {
            try { load(); } catch (Exception e) { log.error("Calendar reload after auth failed", e); }
        });
    }

    /** カレンダーが読込中か */
    public boolean isLoading() {
        return loading.get();
    }

    /**
     * Google Calendar API からイベントを全件取得してキャッシュする
     */
    public synchronized void load() throws Exception {
        loading.set(true);
        try {
            calendarEvents.clear();

            var credential = authService.authorize(SCOPES, "token_Calendar");
            var service = new Calendar.Builder(
                    authService.newTransport(),
                    authService.getJsonFactory(),
                    credential)
                    .setApplicationName(authService.getApplicationName())
                    .build();

            String calendarId = props.getGoogle().getCalendarId();
            List<Event> allEvents = fetchAllEvents(service, calendarId);

            for (Event event : allEvents) {
                mapEvent(event);
            }

            log.info("カレンダー読み込み完了: {}件", calendarEvents.size());
        } finally {
            loading.set(false);
        }
    }

    /** ページネーションを使って全イベントを取得する */
    private List<Event> fetchAllEvents(Calendar service, String calendarId) throws Exception {
        var request = service.events().list(calendarId);
        request.setMaxResults(2500);
        request.setPageToken(null);

        List<Event> result = new ArrayList<>();
        do {
            Events events = request.execute();
            if (events.getItems() != null) {
                result.addAll(events.getItems());
            }
            request.setPageToken(events.getNextPageToken());
        } while (request.getPageToken() != null);

        result.sort((a, b) -> {
            var sa = a.getStart().getDateTime();
            var sb = b.getStart().getDateTime();
            // null (全日イベント) は Long.MIN_VALUE として先頭に並べる
            long va = (sa != null) ? sa.getValue() : Long.MIN_VALUE;
            long vb = (sb != null) ? sb.getValue() : Long.MIN_VALUE;
            return Long.compare(va, vb);
        });
        return result;
    }

    private void mapEvent(Event event) {
        var start = event.getStart();
        var end   = event.getEnd();

        CalendarEventsEntity entity;

        // 全日イベント (dateのみ、dateTimeなし)
        if (start.getDateTime() == null || isAllDayTime(start.getDateTime())) {
            LocalDateTime startDt = parseDate(start.getDate() != null ? start.getDate().toString() : null);
            LocalDateTime endDt   = parseDate(end.getDate()   != null ? end.getDate().toString()   : null);
            entity = new CalendarEventsEntity(
                    event.getSummary(), startDt, endDt,
                    event.getDescription() != null ? event.getDescription() : "");
        } else {
            if (event.getSummary() == null) return;

            LocalDateTime startDt = toLocalDateTime(start.getDateTime().getValue());
            LocalDateTime endDt   = toLocalDateTime(end.getDateTime().getValue());
            entity = new CalendarEventsEntity(
                    event.getSummary(), startDt, endDt,
                    event.getLocation() != null ? event.getLocation() : "",
                    event.getDescription() != null ? event.getDescription() : "");
        }

        // 添付ファイル
        if (event.getAttachments() != null) {
            var atts = event.getAttachments().stream()
                    .map(att -> new AttachmentEntity(entity.getStartDate(), att.getTitle(), att.getFileUrl(), att.getMimeType()))
                    .toList();
            entity.setAttachments(atts);
        }

        calendarEvents.add(entity);
    }

    private boolean isAllDayTime(com.google.api.client.util.DateTime dt) {
        var ldt = toLocalDateTime(dt.getValue());
        return ldt.getHour() == 0 && ldt.getMinute() == 0 && ldt.getSecond() == 0;
    }

    private LocalDateTime toLocalDateTime(long epochMillis) {
        return LocalDateTime.ofInstant(
                java.time.Instant.ofEpochMilli(epochMillis), ZoneId.systemDefault());
    }

    private LocalDateTime parseDate(String dateStr) {
        if (dateStr == null) return LocalDateTime.MIN;
        return LocalDate.parse(dateStr).atStartOfDay();
    }

    // ── フィルタリングメソッド ────────────────────────────────────────────

    /** 日付で検索 */
    public List<CalendarEventsEntity> findByDate(LocalDate date) {
        return calendarEvents.stream()
                .filter(e -> e.getStartDate().toLocalDate().equals(date))
                .toList();
    }

    /** 日付でアニメイベント（【視聴先】を含む全日イベント）を検索 */
    public List<CalendarEventsEntity> findAnimeByDate(LocalDate date) {
        return calendarEvents.stream()
                .filter(e -> e.isProgram() && e.getStartDate().toLocalDate().equals(date))
                .toList();
    }

    /** 開始日〜終了日で検索 */
    public List<CalendarEventsEntity> findByDate(LocalDate startDate, LocalDate endDate) {
        return calendarEvents.stream()
                .filter(e -> !e.getStartDate().toLocalDate().isBefore(startDate) &&
                             !e.getEndDate().toLocalDate().isAfter(endDate))
                .toList();
    }

    /** 開始日〜終了日 + 開始時刻以降で検索 */
    public List<CalendarEventsEntity> findByDate(LocalDate startDate, LocalDate endDate, java.time.LocalTime startTime) {
        return calendarEvents.stream()
                .filter(e -> !e.getStartDate().toLocalDate().isBefore(startDate) &&
                             !e.getEndDate().toLocalDate().isAfter(endDate) &&
                             !e.getStartDate().toLocalTime().isBefore(startTime))
                .toList();
    }

    /** タイトルで検索 */
    public List<CalendarEventsEntity> findByTitle(String title, LocalDate startDate) {
        return calendarEvents.stream()
                .filter(e -> e.getTitle() != null && e.getTitle().contains(title) &&
                             e.getStartDate().toLocalDate().equals(startDate))
                .toList();
    }

    /** タイトル + 日付範囲で検索 */
    public List<CalendarEventsEntity> findByTitle(String title, LocalDate startDate, LocalDate endDate) {
        return calendarEvents.stream()
                .filter(e -> e.getTitle() != null && e.getTitle().contains(title) &&
                             !e.getStartDate().toLocalDate().isBefore(startDate) &&
                             !e.getEndDate().toLocalDate().isAfter(endDate))
                .toList();
    }

    /** 場所で検索 */
    public List<CalendarEventsEntity> findByAddress(String address) {
        return calendarEvents.stream()
                .filter(e -> e.getPlace() != null && e.getPlace().contains(address))
                .toList();
    }

    /** 場所 + 日付範囲で検索 */
    public List<CalendarEventsEntity> findByAddress(String address, LocalDate startDate, LocalDate endDate) {
        return calendarEvents.stream()
                .filter(e -> e.getPlace() != null && e.getPlace().contains(address) &&
                             !e.getStartDate().toLocalDate().isBefore(startDate) &&
                             !e.getEndDate().toLocalDate().isAfter(endDate))
                .toList();
    }

    /** 説明で検索 */
    public List<CalendarEventsEntity> findByDescription(String description) {
        return calendarEvents.stream()
                .filter(e -> e.getDescription() != null && e.getDescription().contains(description))
                .toList();
    }

    /** 説明 + 日付範囲で検索 */
    public List<CalendarEventsEntity> findByDescription(String description, LocalDate startDate, LocalDate endDate) {
        return calendarEvents.stream()
                .filter(e -> e.getDescription() != null && e.getDescription().contains(description) &&
                             !e.getStartDate().toLocalDate().isBefore(startDate) &&
                             !e.getEndDate().toLocalDate().isAfter(endDate))
                .toList();
    }

    /** タイトル・場所・説明でキーワード検索 (最大10件) */
    public List<CalendarEventsEntity> search(String q) {
        String lower = q.toLowerCase();
        return calendarEvents.stream()
                .filter(e -> (e.getTitle() != null && e.getTitle().toLowerCase().contains(lower)) ||
                             (e.getPlace() != null && e.getPlace().toLowerCase().contains(lower)) ||
                             (e.getDescription() != null && e.getDescription().toLowerCase().contains(lower)))
                .limit(10)
                .toList();
    }
}
