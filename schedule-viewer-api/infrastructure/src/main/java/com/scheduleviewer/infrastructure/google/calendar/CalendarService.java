package com.scheduleviewer.infrastructure.google.calendar;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.google.api.client.util.DateTime;
import com.google.api.services.calendar.Calendar;
import com.google.api.services.calendar.CalendarScopes;
import com.google.api.services.calendar.model.Event;
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

import java.security.MessageDigest;
import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.LocalTime;
import java.time.ZoneId;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.HexFormat;
import java.util.List;
import java.util.Map;
import java.util.concurrent.atomic.AtomicBoolean;
import java.util.regex.Pattern;

/**
 * Google Calendar 読み込みサービス
 * <p>.NET版の CalendarReader に相当</p>
 */
@Service
public class CalendarService {

    private static final Logger log = LoggerFactory.getLogger(CalendarService.class);
    private static final Pattern SECTION_PATTERN = Pattern.compile("(?m)^【[^】]+】");
    private static final String BOX_PROPERTY_PREFIX = "scheduleviewer.box.";
    private static final String BOX_LINK_MIME_TYPE = "application/vnd.scheduleviewer.box-link";
    private static final ObjectMapper OBJECT_MAPPER = new ObjectMapper();
    private static final List<String> SCOPES = List.of(CalendarScopes.CALENDAR); // 読み書き両方必要

    private final GoogleAuthService authService;
    private final AppProperties props;

    private volatile List<CalendarEventsEntity> calendarEvents = List.of();
    private final AtomicBoolean loading = new AtomicBoolean(false);

    public CalendarService(GoogleAuthService authService, AppProperties props) {
        this.authService = authService;
        this.props = props;
    }

    /** 起動時に非同期でカレンダーを読み込む (トークンが存在する場合のみ)。失敗時は最大3回リトライする */
	@PostConstruct
	public void initializeAsync() {
	    if (!authService.hasToken("token_Calendar")) {
	        log.info("Google Calendar トークンが未設定のため起動時読み込みをスキップします");
	        return;
	    }
	    Thread.ofVirtual().start(() -> {
	        long[] retryDelaysMs = {30_000L, 60_000L, 120_000L};
	        for (int attempt = 0; attempt <= retryDelaysMs.length; attempt++) {
	            try {
	                load();
	                return;
	            } catch (Exception e) {
	                if (attempt < retryDelaysMs.length) {
	                    log.warn("カレンダーの読み込みに失敗しました (試行 {}/{}), {}秒後にリトライします: {}",
	                            attempt + 1, retryDelaysMs.length + 1, retryDelaysMs[attempt] / 1000, e.getMessage());
	                    try { Thread.sleep(retryDelaysMs[attempt]); } catch (InterruptedException ie) { return; }
	                } else {
	                    log.error("カレンダーの読み込みに全試行失敗しました", e);
	                }
	            }
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
        long nowMs = System.currentTimeMillis();
    	var request = service.events().list(calendarId);
        request.setMaxResults(2500);
    	// 繰り返しイベントを個別インスタンスに展開する
    	request.setSingleEvents(true);
    	request.setOrderBy("startTime");
    	request.setShowDeleted(false);
    	// 10年前から取得
	    long tenYearsAgoMs = java.time.Instant.now()
	            .minus(java.time.Duration.ofDays(365 * 10))
	            .toEpochMilli();
	    request.setTimeMin(new DateTime(tenYearsAgoMs));    	
        request.setPageToken(null);

        List<Event> result = new ArrayList<>();
        do {
            Events events = request.execute();
            if (events.getItems() != null) {
                result.addAll(events.getItems());
            }
            request.setPageToken(events.getNextPageToken());
        } while (request.getPageToken() != null);

        //result.sort((a, b) -> {
        //    var sa = a.getStart().getDateTime();
        //    var sb = b.getStart().getDateTime();
        //    // null (全日イベント) は Long.MIN_VALUE として先頭に並べる
        //    long va = (sa != null) ? sa.getValue() : Long.MIN_VALUE;
        //    long vb = (sb != null) ? sb.getValue() : Long.MIN_VALUE;
        //    return Long.compare(va, vb);
        //});
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
        var attachments = new ArrayList<AttachmentEntity>();
        if (event.getAttachments() != null) {
            attachments.addAll(event.getAttachments().stream()
                    .map(att -> new AttachmentEntity(entity.getStartDate(), att.getTitle(), att.getFileUrl(), att.getMimeType()))
                    .toList());
        }
        if (event.getExtendedProperties() != null && event.getExtendedProperties().getPrivate() != null) {
            attachments.addAll(readBoxAttachments(entity.getStartDate(), event.getExtendedProperties().getPrivate()));
        }
        entity.setAttachments(attachments);

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
        var calService = createCalendarClient();

        String eventTitle = seriesTitle + " 第" + episode + "話";
        String desc = "\n【サブタイトル】\n" + subtitle
                    + "\n\n【視聴先】\n" + service
                    + "\n\n【概要】\n" + summary;

        var event = new Event()
                .setSummary(eventTitle)
                .setDescription(desc)
                .setColorId("4") // Flamingo
                .setStart(new EventDateTime().setDate(new DateTime(date.toString())))
                .setEnd(new EventDateTime().setDate(new DateTime(date.plusDays(1).toString())));

        String calendarId = props.getGoogle().getCalendarId();
        calService.events().insert(calendarId, event).execute();
        log.info("カレンダーにアニメイベント登録: {} on {}", eventTitle, date);

        // インメモリキャッシュを更新
        reloadAsync("Calendar reload after insert failed");
    }

    /** 通常の予定をGoogle Calendarへ登録し、表示用キャッシュへ即時反映する。 */
    public synchronized void createEvent(String title, LocalDate date, boolean allDay,
                                         LocalTime startTime, LocalTime endTime,
                                         String location, String description) throws Exception {
        Event event = buildEvent(title, date, allDay, startTime, endTime, location, description);
        Event inserted = createCalendarClient().events()
                .insert(props.getGoogle().getCalendarId(), event)
                .execute();

        var updated = new ArrayList<CalendarEventsEntity>(calendarEvents.size() + 1);
        updated.addAll(calendarEvents);
        mapEventInto(inserted, updated);
        calendarEvents = List.copyOf(updated);
        log.info("カレンダーに予定登録: {} on {}", title, date);
    }

    /** 入力値からGoogle Calendar APIへ送信するイベントを構築する。 */
    static Event buildEvent(String title, LocalDate date, boolean allDay,
                            LocalTime startTime, LocalTime endTime,
                            String location, String description) {
        if (title == null || title.isBlank()) throw new IllegalArgumentException("タイトルを入力してください");
        if (date == null) throw new IllegalArgumentException("日付を入力してください");

        var event = new Event()
                .setSummary(title.trim())
                .setLocation(blankToNull(location))
                .setDescription(blankToNull(description));

        if (allDay) {
            event.setStart(new EventDateTime().setDate(new DateTime(date.toString())));
            event.setEnd(new EventDateTime().setDate(new DateTime(date.plusDays(1).toString())));
            return event;
        }

        if (startTime == null || endTime == null) {
            throw new IllegalArgumentException("開始時刻と終了時刻を入力してください");
        }
        LocalDateTime start = date.atTime(startTime);
        LocalDateTime end = date.atTime(endTime);
        if (!end.isAfter(start)) throw new IllegalArgumentException("終了時刻は開始時刻より後にしてください");

        ZoneId zone = ZoneId.systemDefault();
        event.setStart(new EventDateTime()
                .setDateTime(new DateTime(start.atZone(zone).toInstant().toEpochMilli()))
                .setTimeZone(zone.getId()));
        event.setEnd(new EventDateTime()
                .setDateTime(new DateTime(end.atZone(zone).toInstant().toEpochMilli()))
                .setTimeZone(zone.getId()));
        return event;
    }

    private static String blankToNull(String value) {
        return value == null || value.isBlank() ? null : value.trim();
    }

    /** Google Photosの共有URLをイベント説明の写真セクションへ追加する。 */
    public void addPhotoUrl(String eventId, String photoUrl) throws Exception {
        var calService = createCalendarClient();

        String calendarId = props.getGoogle().getCalendarId();
        Event event = calService.events().get(calendarId, eventId).execute();
        event.setDescription(appendSectionValue(event.getDescription(), "写真", photoUrl));
        calService.events().update(calendarId, eventId, event)
                .setSupportsAttachments(true)
                .execute();
        log.info("写真URL追加: eventId={}, url={}", eventId, photoUrl);
        reloadAsync("Calendar reload after photo attach failed");
    }

    /** Boxの共有リンクをイベントの非公開メタデータへ保存する。 */
    public void attachBoxFile(String eventId, String fileUrl, String fileTitle) throws Exception {
        var calService = createCalendarClient();

        String calendarId = props.getGoogle().getCalendarId();
        Event event = calService.events().get(calendarId, eventId).execute();
        Map<String, String> existing = event.getExtendedProperties() != null
                ? event.getExtendedProperties().getPrivate()
                : null;
        Map<String, String> privateProperties = addBoxLink(existing, fileUrl, fileTitle);

        calService.events().patch(calendarId, eventId, createBoxMetadataPatch(privateProperties)).execute();
        log.info("添付ファイル追加: eventId={}, title={}", eventId, fileTitle);
        reloadAsync("Calendar reload after attach failed");
    }

    static Map<String, String> addBoxLink(Map<String, String> existing, String fileUrl, String fileTitle)
            throws Exception {
        var properties = existing == null ? new HashMap<String, String>() : new HashMap<>(existing);
        String value = OBJECT_MAPPER.writeValueAsString(new BoxLink(fileTitle, fileUrl));
        if (value.length() > 1024) {
            throw new IllegalArgumentException("Box link metadata exceeds the Google Calendar property limit");
        }
        properties.put(boxPropertyKey(fileUrl), value);
        return properties;
    }

    static List<AttachmentEntity> readBoxAttachments(LocalDateTime date, Map<String, String> properties) {
        var attachments = new ArrayList<AttachmentEntity>();
        if (properties == null) return attachments;

        properties.entrySet().stream()
                .filter(entry -> entry.getKey().startsWith(BOX_PROPERTY_PREFIX))
                .sorted(Map.Entry.comparingByKey())
                .forEach(entry -> {
                    try {
                        BoxLink link = OBJECT_MAPPER.readValue(entry.getValue(), BoxLink.class);
                        if (link.title() != null && link.url() != null) {
                            attachments.add(new AttachmentEntity(date, link.title(), link.url(), BOX_LINK_MIME_TYPE));
                        }
                    } catch (JsonProcessingException e) {
                        log.warn("Invalid Box link metadata ignored: {}", entry.getKey());
                    }
                });
        return attachments;
    }

    static Event createBoxMetadataPatch(Map<String, String> privateProperties) {
        return new Event().setExtendedProperties(
                new Event.ExtendedProperties().setPrivate(privateProperties));
    }

    static String appendSectionValue(String description, String sectionName, String value) {
        String normalized = description == null ? "" : description.replace("\r\n", "\n").replace('\r', '\n');
        String marker = "【" + sectionName + "】";
        int markerIndex = normalized.indexOf(marker);

        if (markerIndex < 0) {
            String separator = normalized.isBlank() ? "" : "\n\n";
            return normalized + separator + marker + "\n" + value;
        }

        int sectionContentStart = markerIndex + marker.length();
        var nextSection = SECTION_PATTERN.matcher(normalized);
        nextSection.region(sectionContentStart, normalized.length());
        int insertAt = nextSection.find() ? nextSection.start() : normalized.length();
        String before = normalized.substring(0, insertAt).stripTrailing();
        String after = normalized.substring(insertAt);

        if (before.lines().anyMatch(value::equals)) return normalized;
        String suffix = after.isEmpty() ? "" : "\n" + after;
        return before + "\n" + value + suffix;
    }

    private static String boxPropertyKey(String fileUrl) throws Exception {
        byte[] digest = MessageDigest.getInstance("SHA-256")
                .digest(fileUrl.getBytes(java.nio.charset.StandardCharsets.UTF_8));
        return BOX_PROPERTY_PREFIX + HexFormat.of().formatHex(digest, 0, 12);
    }

    private Calendar createCalendarClient() throws Exception {
        var credential = authService.authorize(SCOPES, "token_Calendar");
        return buildCalendarService(credential);
    }

    private void reloadAsync(String errorMessage) {
        Thread.ofVirtual().start(() -> {
            try { load(); } catch (Exception e) { log.error(errorMessage, e); }
        });
    }

    private record BoxLink(String title, String url) {}

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
