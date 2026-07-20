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
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

/** Loads and caches Google Tasks for the daily schedule view. */
@Service
public class TasksService {

    private static final Logger log = LoggerFactory.getLogger(TasksService.class);
    private static final List<String> SCOPES = List.of(TasksScopes.TASKS);
    private static final DateTimeFormatter RFC3339 =
            DateTimeFormatter.ofPattern("yyyy-MM-dd'T'HH:mm:ss.SSS'Z'");

    private final GoogleAuthService authService;
    private final SpreadsheetService spreadsheetService;
    private final List<TaskEntity> entities = new ArrayList<>();

    public TasksService(GoogleAuthService authService, SpreadsheetService spreadsheetService) {
        this.authService = authService;
        this.spreadsheetService = spreadsheetService;
    }

    @PostConstruct
    public void initializeAsync() {
        if (!authService.hasToken("token_Tasks")) {
            log.info("Google Tasks token is not configured; startup loading was skipped");
            return;
        }

        Thread.ofVirtual().start(() -> {
            try {
                load();
            } catch (Exception e) {
                log.error("Google Tasks startup loading failed", e);
            }
        });
    }

    public String getAuthUrl() throws Exception {
        return authService.startAuthFlowAndGetUrl(SCOPES, "token_Tasks", () -> {
            try {
                load();
            } catch (Exception e) {
                log.error("Google Tasks reload after authentication failed", e);
            }
        });
    }

    /** Reload every dated task from all visible Google task lists. */
    public synchronized void load() throws Exception {
        var credential = authService.authorize(SCOPES, "token_Tasks");
        var service = new Tasks.Builder(
                authService.newTransport(),
                authService.getJsonFactory(),
                credential)
                .setApplicationName(authService.getApplicationName())
                .build();

        Map<String, String> taskLists = loadTaskLists(service);
        if (taskLists.isEmpty()) {
            log.warn("No Google Tasks task lists were available");
            return;
        }

        List<TaskEntity> loaded = new ArrayList<>();
        for (Map.Entry<String, String> taskList : taskLists.entrySet()) {
            String taskListId = taskList.getKey();
            String taskListName = taskList.getValue();

            List<Task> tasks;
            try {
                tasks = fetchAllTasks(service, taskListId);
            } catch (Exception e) {
                log.warn("Skipping Google task list '{}' ({}): {}",
                        taskListName, taskListId, e.getMessage());
                continue;
            }

            for (Task task : tasks) {
                if (task.getDue() == null) continue;

                loaded.add(new TaskEntity(
                        taskListName,
                        task.getTitle() != null ? task.getTitle() : "",
                        task.getNotes() != null ? task.getNotes() : "",
                        task.getCompleted() != null ? parseDateTime(task.getCompleted()) : null,
                        parseDateTime(task.getDue())));
            }
        }

        loaded.sort(Comparator.comparing(TaskEntity::getDueDate).reversed());
        entities.clear();
        entities.addAll(loaded);
        log.info("Google Tasks loading completed: {} dated tasks from {} lists",
                entities.size(), taskLists.size());
    }

    /**
     * Merge spreadsheet-configured lists with every list visible through the
     * Google Tasks API. Configured display names take precedence.
     */
    private Map<String, String> loadTaskLists(Tasks service) {
        Map<String, String> result = new LinkedHashMap<>();
        List<List<Object>> configuredLists = spreadsheetService.readTasks();

        if (!configuredLists.isEmpty() && !configuredLists.get(0).isEmpty()) {
            String headerLabel = configuredLists.get(0).get(0).toString();
            for (List<Object> row : configuredLists) {
                if (row.size() < 2 || row.get(0) == null || row.get(1) == null) continue;
                String name = row.get(0).toString();
                String id = row.get(1).toString();
                if (!name.equals(headerLabel) && !id.isBlank()) result.put(id, name);
            }
        }

        try {
            String pageToken = null;
            do {
                var response = service.tasklists().list()
                        .setMaxResults(100)
                        .setPageToken(pageToken)
                        .execute();
                if (response.getItems() != null) {
                    for (var taskList : response.getItems()) {
                        result.putIfAbsent(taskList.getId(), taskList.getTitle());
                    }
                }
                pageToken = response.getNextPageToken();
            } while (pageToken != null && !pageToken.isBlank());
        } catch (Exception e) {
            log.warn("Google Tasks task-list discovery failed: {}", e.getMessage());
        }

        return result;
    }

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
            if (response.getItems() != null) result.addAll(response.getItems());
            request.setPageToken(response.getNextPageToken());
        } while (request.getPageToken() != null);

        return result;
    }

    public List<TaskEntity> findByDate(LocalDate date) {
        return entities.stream()
                .filter(entity -> entity.getDueDate().toLocalDate().equals(date))
                .toList();
    }

    public List<TaskEntity> getAll() {
        return List.copyOf(entities);
    }

    private LocalDateTime parseDateTime(String rfc3339) {
        try {
            return LocalDateTime.parse(rfc3339, RFC3339);
        } catch (Exception e) {
            return LocalDate.parse(rfc3339.substring(0, 10)).atStartOfDay();
        }
    }
}
