package com.scheduleviewer.infrastructure.google.tasks;

import com.google.api.services.tasks.Tasks;
import com.google.api.services.tasks.TasksScopes;
import com.google.api.services.tasks.model.Task;
import com.scheduleviewer.domain.entity.TaskEntity;
import com.scheduleviewer.infrastructure.google.GoogleAuthService;
import com.scheduleviewer.infrastructure.google.spreadsheet.SpreadsheetService;
import jakarta.annotation.PostConstruct;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Service;

import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.util.ArrayList;
import java.util.Comparator;
import java.util.List;

/**
 * Google Tasks サービス
 * <p>.NET版の TaskReader に相当</p>
 * <p>Spreadsheetからタスクリスト一覧を読み込み、各リストのタスクを取得する</p>
 */
@Service
public class TasksService {

    private static final Logger log = LoggerFactory.getLogger(TasksService.class);
    private static final List<String> SCOPES = List.of(TasksScopes.TASKS);
    private static final DateTimeFormatter RFC3339 = DateTimeFormatter.ofPattern("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'");

    private final GoogleAuthService authService;
    private final SpreadsheetService spreadsheetService;

    private final List<TaskEntity> entities = new ArrayList<>();

    public TasksService(GoogleAuthService authService, SpreadsheetService spreadsheetService) {
        this.authService       = authService;
        this.spreadsheetService = spreadsheetService;
    }

    /** 起動時に非同期でタスクを読み込む (トークンが存在する場合のみ) */
    @PostConstruct
    public void initializeAsync() {
        if (!authService.hasToken("token_Tasks")) {
            log.info("Google Tasks トークンが未設定のため起動時読み込みをスキップします");
            return;
        }
        Thread.ofVirtual().start(() -> {
            try {
                load();
            } catch (Exception e) {
                log.error("タスクの読み込みに失敗しました", e);
            }
        });
    }

    /** OAuth認証URLを取得する。認証完了後に自動でデータを読み込む。認証済みの場合は null を返す。 */
    public String getAuthUrl() throws Exception {
        return authService.startAuthFlowAndGetUrl(SCOPES, "token_Tasks", () -> {
            try { load(); } catch (Exception e) { log.error("Tasks reload after auth failed", e); }
        });
    }

    /** タスクを全件取得してキャッシュする */
    public synchronized void load() throws Exception {
        entities.clear();

        var credential = authService.authorize(SCOPES, "token_Tasks");
        var service = new Tasks.Builder(
                authService.newTransport(),
                authService.getJsonFactory(),
                credential)
                .setApplicationName(authService.getApplicationName())
                .build();

        // Spreadsheetからタスクリスト名とIDを取得
        List<List<Object>> taskLists = spreadsheetService.readTasks();

        if (taskLists.isEmpty()) {
            log.warn("タスクリストが取得できませんでした");
            return;
        }

        // 1行目はヘッダー行なのでスキップ
        String headerLabel = taskLists.get(0).get(0).toString();

        for (List<Object> row : taskLists) {
            if (row.get(0).toString().equals(headerLabel)) continue;
            if (row.size() < 2) continue;

            String taskListName = row.get(0).toString();
            String taskListId   = row.get(1).toString();

            List<Task> tasks;
            try {
                tasks = fetchAllTasks(service, taskListId);
            } catch (Exception e) {
                log.warn("タスクリスト '{}' ({}) の取得をスキップ: {}", taskListName, taskListId, e.getMessage());
                continue;
            }

            for (Task task : tasks) {
                if (task.getCompleted() == null || task.getDue() == null) continue;

                entities.add(new TaskEntity(
                        taskListName,
                        task.getTitle(),
                        task.getNotes() != null ? task.getNotes() : "",
                        parseDateTime(task.getCompleted()),
                        parseDateTime(task.getDue())));
            }
        }

        entities.sort(Comparator.comparing(TaskEntity::getDueDate).reversed());
        log.info("タスク読み込み完了: {}件", entities.size());
    }

    /** ページネーションで全タスクを取得する */
    private List<Task> fetchAllTasks(Tasks service, String taskListId) throws Exception {
        var request = service.tasks().list(taskListId);
        request.setMaxResults(100);
        request.setShowCompleted(true);
        request.setShowDeleted(false);
        request.setShowHidden(true);
        request.setPageToken(null);

        List<Task> result = new ArrayList<>();
        do {
            var response = request.execute();
            if (response.getItems() != null) {
                result.addAll(response.getItems());
            }
            request.setPageToken(response.getNextPageToken());
        } while (request.getPageToken() != null);

        return result;
    }

    /** 日付でタスクを検索する */
    public List<TaskEntity> findByDate(LocalDate date) {
        return entities.stream()
                .filter(e -> e.getDueDate().toLocalDate().equals(date))
                .toList();
    }

    /** 全タスクを返す */
    public List<TaskEntity> getAll() {
        return List.copyOf(entities);
    }

    private LocalDateTime parseDateTime(String rfc3339) {
        if (rfc3339 == null) return LocalDateTime.MIN;
        try {
            return LocalDateTime.parse(rfc3339, RFC3339);
        } catch (Exception e) {
            // RFC3339 の形式が "yyyy-MM-dd" の場合
            return LocalDate.parse(rfc3339.substring(0, 10)).atStartOfDay();
        }
    }
}
