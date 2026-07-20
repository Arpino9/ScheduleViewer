package com.scheduleviewer.api.controller;

import com.scheduleviewer.domain.entity.TaskEntity;
import com.scheduleviewer.infrastructure.google.tasks.TasksService;
import org.springframework.format.annotation.DateTimeFormat;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDate;
import java.util.List;

/**
 * Google Tasks コントローラー
 */
@RestController
@RequestMapping("/api/tasks")
public class TaskController {

    private final TasksService tasksService;

    public TaskController(TasksService tasksService) {
        this.tasksService = tasksService;
    }

    /**
     * 全タスクを取得する (期日の降順)
     */
    @GetMapping
    public List<TaskEntity> getAll() {
        return tasksService.getAll();
    }

    /**
     * 指定日のタスクを取得する
     *
     * @param date 日付 (yyyy-MM-dd)
     */
    @GetMapping("/date/{date}")
    public List<TaskEntity> getByDate(
            @PathVariable @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate date) {
        return tasksService.findByDate(date);
    }

    /**
     * タスクキャッシュを再読み込みする
     */
    @PostMapping("/reload")
    public void reload() throws Exception {
        tasksService.load();
    }
}
