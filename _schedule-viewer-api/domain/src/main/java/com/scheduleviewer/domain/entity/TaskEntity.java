package com.scheduleviewer.domain.entity;

import java.time.LocalDateTime;

/**
 * Entity - ToDoリスト
 */
public final class TaskEntity {

    private final String taskListName;
    private final String taskName;
    private final String details;
    private final LocalDateTime completed;
    private final LocalDateTime dueDate;

    public TaskEntity(
            String taskListName,
            String taskName,
            String details,
            LocalDateTime completed,
            LocalDateTime dueDate) {
        this.taskListName = taskListName;
        this.taskName     = taskName;
        this.details      = details;
        this.completed    = completed;
        this.dueDate      = dueDate;
    }

    /** ToDoリスト名 */
    public String getTaskListName() { return taskListName; }

    /** タスク名 */
    public String getTaskName() { return taskName; }

    /** 詳細 */
    public String getDetails() { return details; }

    /** 完了日時 */
    public LocalDateTime getCompleted() { return completed; }

    /** 期日 */
    public LocalDateTime getDueDate() { return dueDate; }
}
