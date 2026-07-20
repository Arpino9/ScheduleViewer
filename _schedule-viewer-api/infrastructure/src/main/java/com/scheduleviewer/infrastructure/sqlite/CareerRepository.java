package com.scheduleviewer.infrastructure.sqlite;

import com.scheduleviewer.domain.entity.AllowanceExistenceEntity;
import com.scheduleviewer.domain.entity.CareerEntity;
import com.scheduleviewer.domain.repository.ICareerRepository;
import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.stereotype.Repository;

import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.List;

/**
 * SQLite - 職歴リポジトリ
 */
@Repository
public class CareerRepository implements ICareerRepository {

    private static final String SELECT_COLUMNS = """
            SELECT ID, CompanyName, EmployeeNumber, WorkingStatus,
                   WorkingStartDate, WorkingEndDate,
                   PerfectAttendance, Education, Electricity, Certification,
                   Overtime, Travel, Housing, Food, LateNight, Area,
                   Commution, PrepaidRetirement, Dependency, Executive,
                   Special, Remarks
            """;

    private final JdbcTemplate jdbc;

    public CareerRepository(JdbcTemplate jdbc) {
        this.jdbc = jdbc;
    }

    @Override
    public List<CareerEntity> getEntities() {
        String sql = SELECT_COLUMNS + "FROM Career";
        return jdbc.query(sql, this::mapRow);
    }

    @Override
    public CareerEntity getEntity(int id) {
        String sql = SELECT_COLUMNS + "FROM Career WHERE ID = ?";
        return jdbc.queryForObject(sql, this::mapRow, id);
    }

    @Override
    public void save(CareerEntity entity) {
        String update = """
                UPDATE Career SET
                    CompanyName       = ?, EmployeeNumber    = ?,
                    WorkingStatus     = ?, WorkingStartDate  = ?,
                    WorkingEndDate    = ?, PerfectAttendance = ?,
                    Education         = ?, Electricity       = ?,
                    Certification     = ?, Overtime          = ?,
                    Travel            = ?, Housing           = ?,
                    Food              = ?, LateNight         = ?,
                    Area              = ?, Commution         = ?,
                    PrepaidRetirement = ?, Dependency        = ?,
                    Executive         = ?, Special           = ?,
                    Remarks           = ?
                WHERE ID = ?
                """;
        int updated = jdbc.update(update, buildParams(entity));
        if (updated == 0) {
            String insert = """
                    INSERT INTO Career (
                        ID, CompanyName, EmployeeNumber, WorkingStatus,
                        WorkingStartDate, WorkingEndDate,
                        PerfectAttendance, Education, Electricity, Certification,
                        Overtime, Travel, Housing, Food, LateNight, Area,
                        Commution, PrepaidRetirement, Dependency, Executive,
                        Special, Remarks)
                    VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)
                    """;
            jdbc.update(insert, buildInsertParams(entity));
        }
    }

    @Override
    public void delete(int id) {
        jdbc.update("DELETE FROM Career WHERE ID = ?", id);
    }

    private CareerEntity mapRow(ResultSet rs, int rowNum) throws SQLException {
        AllowanceExistenceEntity allowance = new AllowanceExistenceEntity(
                rs.getBoolean("PerfectAttendance"),
                rs.getBoolean("Education"),
                rs.getBoolean("Electricity"),
                rs.getBoolean("Certification"),
                rs.getBoolean("Overtime"),
                rs.getBoolean("Travel"),
                rs.getBoolean("Housing"),
                rs.getBoolean("Food"),
                rs.getBoolean("LateNight"),
                rs.getBoolean("Area"),
                rs.getBoolean("Commution"),
                rs.getBoolean("PrepaidRetirement"),
                rs.getBoolean("Dependency"),
                rs.getBoolean("Executive"),
                rs.getBoolean("Special"));
        return new CareerEntity(
                rs.getInt("ID"),
                rs.getString("WorkingStatus"),
                rs.getString("CompanyName"),
                rs.getString("EmployeeNumber"),
                rs.getDate("WorkingStartDate").toLocalDate(),
                rs.getDate("WorkingEndDate").toLocalDate(),
                allowance,
                rs.getString("Remarks"));
    }

    private Object[] buildParams(CareerEntity e) {
        var a = e.getAllowanceExistence();
        return new Object[]{
                e.getCompanyName().text(), e.getEmployeeNumber(),
                e.getWorkingStatus(), e.getWorkingStartDate().value().toString(),
                e.getWorkingEndDate().value().toString(),
                a.getPerfectAttendance().value(), a.getEducation().value(),
                a.getElectricity().value(), a.getCertification().value(),
                a.getOvertime().value(), a.getTravel().value(),
                a.getHousing().value(), a.getFood().value(),
                a.getLateNight().value(), a.getArea().value(),
                a.getCommution().value(), a.getPrepaidRetirement().value(),
                a.getDependency().value(), a.getExecutive().value(),
                a.getSpecial().value(), e.getRemarks(), e.getId()
        };
    }

    private Object[] buildInsertParams(CareerEntity e) {
        var a = e.getAllowanceExistence();
        return new Object[]{
                e.getId(), e.getCompanyName().text(), e.getEmployeeNumber(),
                e.getWorkingStatus(), e.getWorkingStartDate().value().toString(),
                e.getWorkingEndDate().value().toString(),
                a.getPerfectAttendance().value(), a.getEducation().value(),
                a.getElectricity().value(), a.getCertification().value(),
                a.getOvertime().value(), a.getTravel().value(),
                a.getHousing().value(), a.getFood().value(),
                a.getLateNight().value(), a.getArea().value(),
                a.getCommution().value(), a.getPrepaidRetirement().value(),
                a.getDependency().value(), a.getExecutive().value(),
                a.getSpecial().value(), e.getRemarks()
        };
    }
}
