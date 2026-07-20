package com.scheduleviewer.infrastructure.google.calendar;

import com.google.api.services.calendar.Calendar;
import com.google.api.services.calendar.CalendarScopes;
import com.google.api.services.calendar.model.Event;
import com.google.api.services.calendar.model.EventAttachment;
import com.google.api.services.calendar.model.EventDateTime;
import com.google.api.services.calendar.model.Events;
import com.scheduleviewer.domain.entity.AttachmentEntity;
import com.scheduleviewer.domain.entity.CalendarEventsEntity;
import com.scheduleviewer.infrastructure.config.AppProperties;
import com.scheduleviewer.infrastructure.google.GoogleAuthService;
import jakarta.annotation.PostConstruct;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Service;

import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.ZoneId;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.concurrent.atomic.AtomicBoolean;

/**
 * Google Calendar 読み込みサービス
 * <p>.NET版の CalendarReader に相当</p>
 */
@Service
public class CalendarService {

    private static final Logger log = LoggerFactory.getLogger(CalendarService.class);
    private static final List<String> SCOPES = List.of(CalendarScopes.CALENDAR); // 読み書き両方必要

    private final GoogleAuthService authService;
    private final AppProperties props;

    private volatile List<CalendarEventsEntity> calendarEvents = List.of();
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

	/** キャッシュ済みイベント件数を返す (データ取得確認用) */
    public int getEventCount() {
        return calendarEvents.size();
    }
	
    /**
     * Google Calendar API からイベントを全件取得してキャッシュする
     */
    public synchronized void load() throws Exception {
        loading.set(true);
        try {
            var credential = authService.authorize(SCOPES, "token_Calendar");
            var service = buildCalendarService(credential);

            String calendarId = props.getGoogle().getCalendarId();
            List<Event> allEvents = fetchAllEvents(service, calendarId);

            List<CalendarEventsEntity> temp = new ArrayList<>(allEvents.size());
            for (Event event : allEvents) {
                mapEventInto(event, temp);
            }
            calendarEvents = temp;

            log.info("カレンダー読み込み完了: {}件", calendarEvents.size());
        } finally {
            loading.set(false);
        }
    }

    private Calendar buildCalendarService(com.google.api.client.auth.oauth2.Credential credential) throws Exception {
        return new Calendar.Builder(
                authService.newTransport(),
                authService.getJsonFactory(),
                httpRequest -> {
                    credential.initialize(httpRequest);
                    httpRequest.setConnectTimeout(30_000);
                    httpRequest.setReadTimeout(120_000);
                })
                .setApplicationName(authService.getApplicationName())
                .build();
    }

    /** ページネーションを使って全イベントを取得する */
    private List<Event> fetchAllEvents(Calendar service, String calendarId) throws Exception {
    	var request = service.events().list(calendarId);
        request.setMaxResults(2500);
    	// 繰り返しイベントを個別インスタンスに展開する (orderBy=startTime に必須)
        request.setSingleEvents(true);
        request.setOrderBy("startTime");
        request.setShowDeleted(false);
        request.setPageToken(null);

        List<Event> result = new ArrayList<>();
        do {
            Events events = request.execute();
            if (events.getItems() != null) {
                result.addAll(events.getItems());
            }
            request.setPageToken(events.getNextPageToken());
        } while (request.getPageToken() != null);

        return result;
    }

    private void mapEventInto(Event event, List<CalendarEventsEntity> target) {
        var start = event.getStart();
        var end   = event.getEnd();

        CalendarEventsEntity entity;

        // 全日イベント: singleEvents=true の場合、全日イベントは必ず getDateTime()==null
        if (start.getDateTime() == null) {
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

        entity.setEventId(event.getId());

        // 添付ファイル
        if (event.getAttachments() != null) {
            var atts = event.getAttachments().stream()
                    .map(att -> new AttachmentEntity(entity.getStartDate(), att.getTitle(), att.getFileUrl(), att.getMimeType()))
                    .toList();
            entity.setAttachments(atts);
        }

        target.add(entity);
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
                .filter(e -> {
                    LocalDate start = e.getStartDate().toLocalDate();
                    LocalDate end   = e.getEndDate().toLocalDate();
                    // 単日イベント (timed含む) はstart==date、複数日全日イベントは start<=date<end
                    return start.equals(date) || (!start.isAfter(date) && end.isAfter(date));
                })
                .toList();
    }

    /** 日付でアニメイベント（【視聴先】を含む全日イベント）を検索 */
    public List<CalendarEventsEntity> findAnimeByDate(LocalDate date) {
        return calendarEvents.stream()
                .filter(e -> {
                    if (!e.isProgram()) return false;
                    LocalDate start = e.getStartDate().toLocalDate();
                    LocalDate end   = e.getEndDate().toLocalDate();
                    return start.equals(date) || (!start.isAfter(date) && end.isAfter(date));
                })
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

    /**
     * アニメ視聴記録を Google Calendar に全日イベントとして登録する
     * タイトル形式: "{seriesTitle} 第{episode}話"
     * 説明形式: "\n【サブタイトル】\n...\n\n【視聴先】\n...\n\n【概要】\n..."
     */
    public void createAnimeEvent(LocalDate date, String seriesTitle, int episode,
                                  String subtitle, String service, String summary) throws Exception {
        var credential = authService.authorize(SCOPES, "token_Calendar");
        var calService = buildCalendarService(credential);

        String eventTitle = seriesTitle + " 第" + episode + "話";
        String desc = "\n【サブタイトル】\n" + subtitle
                    + "\n\n【視聴先】\n" + service
                    + "\n\n【概要】\n" + summary;

        var event = new Event()
                .setSummary(eventTitle)
                .setDescription(desc)
                .setColorId("4") // Flamingo
                .setStart(new EventDateTime().setDate(new com.google.api.client.util.DateTime(date.toString())))
                .setEnd(new EventDateTime().setDate(new com.google.api.client.util.DateTime(date.plusDays(1).toString())));

        String calendarId = props.getGoogle().getCalendarId();
        calService.events().insert(calendarId, event).execute();
        log.info("カレンダーにアニメイベント登録: {} on {}", eventTitle, date);

        // インメモリキャッシュを更新
        Thread.ofVirtual().start(() -> {
            try { load(); } catch (Exception e) { log.error("Calendar reload after insert failed", e); }
        });
    }

    /**
     * 指定イベントに外部URL添付ファイルを追加する (Box等)
     */
    public void addPhotoUrl(String eventId, String photoUrl) throws Exception {
        var credential = authService.authorize(SCOPES, "token_Calendar");
        var calService = buildCalendarService(credential);

        String calendarId = props.getGoogle().getCalendarId();
        Event event = calService.events().get(calendarId, eventId).execute();

        String desc = event.getDescription() != null ? event.getDescription() : "";
        String newDesc;

        int photoIdx = desc.indexOf("【写真】");
        if (photoIdx >= 0) {
            int nextSection = desc.indexOf("\n【", photoIdx + 5);
            if (nextSection < 0) {
                newDesc = desc.stripTrailing() + "\n" + photoUrl;
            } else {
                newDesc = desc.substring(0, nextSection).stripTrailing() + "\n" + photoUrl
                        + desc.substring(nextSection);
            }
        } else {
            String photoSection = "【写真】\n" + photoUrl;
            newDesc = desc.isEmpty() ? photoSection : photoSection + "\n\n" + desc;
        }

        event.setDescription(newDesc);
        calService.events().update(calendarId, eventId, event)
                .setSupportsAttachments(true)
                .execute();
        log.info("写真URL追加: eventId={}, url={}", eventId, photoUrl);

        Thread.ofVirtual().start(() -> {
            try { load(); } catch (Exception e) { log.error("Calendar reload after photo attach failed", e); }
        });
    }

    public void attachBoxFile(String eventId, String fileUrl, String fileTitle) throws Exception {
        var credential = authService.authorize(SCOPES, "token_Calendar");
        var calService = buildCalendarService(credential);

        String calendarId = props.getGoogle().getCalendarId();
        Event event = calService.events().get(calendarId, eventId).execute();

        List<EventAttachment> attachments = event.getAttachments() != null
                ? new ArrayList<>(event.getAttachments())
                : new ArrayList<>();
        attachments.add(new EventAttachment().setFileUrl(fileUrl).setTitle(fileTitle));
        event.setAttachments(attachments);

        calService.events().update(calendarId, eventId, event)
                .setSupportsAttachments(true)
                .execute();
        log.info("添付ファイル追加: eventId={}, title={}", eventId, fileTitle);

        Thread.ofVirtual().start(() -> {
            try { load(); } catch (Exception e) { log.error("Calendar reload after attach failed", e); }
        });
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
