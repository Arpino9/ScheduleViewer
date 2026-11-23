package com.scheduleviewer.infrastructure.sqlite;

import com.scheduleviewer.domain.entity.WorkingPlaceEntity;
import com.scheduleviewer.domain.repository.IWorkingPlaceRepository;
import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.stereotype.Repository;

import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.List;

/**
 * SQLite - 就業場所リポジトリ
 */
@Repository
public class WorkingPlaceRepository implements IWorkingPlaceRepository {

    private static final String SELECT_COLUMNS = """
            SELECT ID, DispatchingCompany, DispatchedCompany,
                   WorkingPlace_Name, WorkingPlace_Address,
                   WorkingStart, WorkingEnd, IsWaiting, IsWorking,
                   WorkingStartTime_Hour,   WorkingStartTime_Minute,
                   WorkingEndTime_Hour,     WorkingEndTime_Minute,
                   LunchStartTime_Hour,     LunchStartTime_Minute,
                   LunchEndTime_Hour,       LunchEndTime_Minute,
                   BreakStartTime_Hour,     BreakStartTime_Minute,
                   BreakEndTime_Hour,       BreakEndTime_Minute,
                   Remarks
            """;

    private final JdbcTemplate jdbc;

    public WorkingPlaceRepository(JdbcTemplate jdbc) {
        this.jdbc = jdbc;
    }

    @Override
    public List<WorkingPlaceEntity> getEntities() {
        return jdbc.query(SELECT_COLUMNS + "FROM WorkingPlace", this::mapRow);
    }

    @Override
    public WorkingPlaceEntity getEntity(int id) {
        return jdbc.queryForObject(SELECT_COLUMNS + "FROM WorkingPlace WHERE ID = ?", this::mapRow, id);
    }

    @Override
    public void save(WorkingPlaceEntity entity) {
        String update = """
                UPDATE WorkingPlace SET
                    DispatchingCompany      = ?, DispatchedCompany       = ?,
                    WorkingPlace_Name       = ?, WorkingPlace_Address    = ?,
                    WorkingStart            = ?, WorkingEnd              = ?,
                    IsWaiting               = ?, IsWorking               = ?,
                    WorkingStartTime_Hour   = ?, WorkingStartTime_Minute = ?,
                    WorkingEndTime_Hour     = ?, WorkingEndTime_Minute   = ?,
                    LunchStartTime_Hour     = ?, LunchStartTime_Minute   = ?,
                    LunchEndTime_Hour       = ?, LunchEndTime_Minute     = ?,
                    BreakStartTime_Hour     = ?, BreakStartTime_Minute   = ?,
                    BreakEndTime_Hour       = ?, BreakEndTime_Minute     = ?,
                    Remarks                 = ?
                WHERE ID = ?
                """;
        int updated = jdbc.update(update, buildParams(entity));
        if (updated == 0) {
            String insert = """
                    INSERT INTO WorkingPlace (
                        ID, DispatchingCompany, DispatchedCompany,
                        WorkingPlace_Name, WorkingPlace_Address,
                        WorkingStart, WorkingEnd, IsWaiting, IsWorking,
                        WorkingStartTime_Hour, WorkingStartTime_Minute,
                        WorkingEndTime_Hour,   WorkingEndTime_Minute,
                        LunchStartTime_Hour,   LunchStartTime_Minute,
                        LunchEndTime_Hour,     LunchEndTime_Minute,
                        BreakStartTime_Hour,   BreakStartTime_Minute,
                        BreakEndTime_Hour,     BreakEndTime_Minute,
                        Remarks)
                    VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)
                    """;
            jdbc.update(insert, buildInsertParams(entity));
        }
    }

    @Override
    public void delete(int id) {
        jdbc.update("DELETE FROM WorkingPlace WHERE ID = ?", id);
    }

    private WorkingPlaceEntity mapRow(ResultSet rs, int rowNum) throws SQLException {
        return new WorkingPlaceEntity(
                rs.getInt("ID"),
                rs.getString("DispatchingCompany"),
                rs.getString("DispatchedCompany"),
                rs.getString("WorkingPlace_Name"),
                rs.getString("WorkingPlace_Address"),
                rs.getDate("WorkingStart").toLocalDate(),
                rs.getDate("WorkingEnd").toLocalDate(),
                rs.getBoolean("IsWaiting"),
                rs.getBoolean("IsWorking"),
                rs.getInt("WorkingStartTime_Hour"),  rs.getInt("WorkingStartTime_Minute"),
                rs.getInt("WorkingEndTime_Hour"),    rs.getInt("WorkingEndTime_Minute"),
                rs.getInt("LunchStartTime_Hour"),    rs.getInt("LunchStartTime_Minute"),
                rs.getInt("LunchEndTime_Hour"),      rs.getInt("LunchEndTime_Minute"),
                rs.getInt("BreakStartTime_Hour"),    rs.getInt("BreakStartTime_Minute"),
                rs.getInt("BreakEndTime_Hour"),      rs.getInt("BreakEndTime_Minute"),
                rs.getString("Remarks"));
    }

    private Object[] buildParams(WorkingPlaceEntity e) {
        var wt = e.getWorkingTime();
        var lt = e.getLunchTime();
        var bt = e.getBreakTime();
        return new Object[]{
                e.getDispatchingCompany().text(), e.getDispatchedCompany().text(),
                e.getWorkingPlaceName().text(), e.getWorkingPlaceAddress(),
                e.getWorkingStart().toString(), e.getWorkingEnd().toString(),
                e.isWaiting(), e.isWorking(),
                (int) wt.start().toHours(), (int) wt.start().toMinutesPart(),
                (int) wt.end().toHours(),   (int) wt.end().toMinutesPart(),
                (int) lt.start().toHours(), (int) lt.start().toMinutesPart(),
                (int) lt.end().toHours(),   (int) lt.end().toMinutesPart(),
                (int) bt.start().toHours(), (int) bt.start().toMinutesPart(),
                (int) bt.end().toHours(),   (int) bt.end().toMinutesPart(),
                e.getRemarks(), e.getId()
        };
    }

    private Object[] buildInsertParams(WorkingPlaceEntity e) {
        var wt = e.getWorkingTime();
        var lt = e.getLunchTime();
        var bt = e.getBreakTime();
        return new Object[]{
                e.getId(),
                e.getDispatchingCompany().text(), e.getDispatchedCompany().text(),
                e.getWorkingPlaceName().text(), e.getWorkingPlaceAddress(),
                e.getWorkingStart().toString(), e.getWorkingEnd().toString(),
                e.isWaiting(), e.isWorking(),
                (int) wt.start().toHours(), (int) wt.start().toMinutesPart(),
                (int) wt.end().toHours(),   (int) wt.end().toMinutesPart(),
                (int) lt.start().toHours(), (int) lt.start().toMinutesPart(),
                (int) lt.end().toHours(),   (int) lt.end().toMinutesPart(),
                (int) bt.start().toHours(), (int) bt.start().toMinutesPart(),
                (int) bt.end().toHours(),   (int) bt.end().toMinutesPart(),
                e.getRemarks()
        };
    }
}
