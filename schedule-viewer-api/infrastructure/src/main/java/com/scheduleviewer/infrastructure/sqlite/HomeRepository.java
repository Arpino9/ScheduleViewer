package com.scheduleviewer.infrastructure.sqlite;

import com.scheduleviewer.domain.entity.HomeEntity;
import com.scheduleviewer.domain.repository.IHomeRepository;
import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.stereotype.Repository;

import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.List;

/**
 * SQLite - 自宅リポジトリ
 */
@Repository
public class HomeRepository implements IHomeRepository {

    private static final String SELECT_COLUMNS = """
            SELECT ID, DisplayName, LivingStart, LivingEnd, IsLiving,
                   PostCode, Address, Address_Google, Remarks
            """;

    private final JdbcTemplate jdbc;

    public HomeRepository(JdbcTemplate jdbc) {
        this.jdbc = jdbc;
    }

    @Override
    public List<HomeEntity> getEntities() {
        return jdbc.query(SELECT_COLUMNS + "FROM Home", this::mapRow);
    }

    @Override
    public HomeEntity getEntity(int id) {
        return jdbc.queryForObject(SELECT_COLUMNS + "FROM Home WHERE ID = ?", this::mapRow, id);
    }

    @Override
    public void save(HomeEntity entity) {
        String update = """
                UPDATE Home SET
                    DisplayName    = ?, LivingStart    = ?,
                    LivingEnd      = ?, IsLiving       = ?,
                    PostCode       = ?, Address        = ?,
                    Address_Google = ?, Remarks        = ?
                WHERE ID = ?
                """;
        int updated = jdbc.update(update,
                entity.getDisplayName(), entity.getLivingStart().toString(),
                entity.getLivingEnd().toString(), entity.isLiving(),
                entity.getPostCode(), entity.getAddress(),
                entity.getAddressGoogle(), entity.getRemarks(),
                entity.getId());
        if (updated == 0) {
            String insert = """
                    INSERT INTO Home (ID, DisplayName, LivingStart, LivingEnd, IsLiving,
                                     PostCode, Address, Address_Google, Remarks)
                    VALUES (?,?,?,?,?,?,?,?,?)
                    """;
            jdbc.update(insert,
                    entity.getId(), entity.getDisplayName(),
                    entity.getLivingStart().toString(), entity.getLivingEnd().toString(),
                    entity.isLiving(), entity.getPostCode(), entity.getAddress(),
                    entity.getAddressGoogle(), entity.getRemarks());
        }
    }

    @Override
    public void delete(int id) {
        jdbc.update("DELETE FROM Home WHERE ID = ?", id);
    }

    private HomeEntity mapRow(ResultSet rs, int rowNum) throws SQLException {
        return new HomeEntity(
                rs.getInt("ID"),
                rs.getString("DisplayName"),
                rs.getDate("LivingStart").toLocalDate(),
                rs.getDate("LivingEnd").toLocalDate(),
                rs.getBoolean("IsLiving"),
                rs.getString("PostCode"),
                rs.getString("Address"),
                rs.getString("Address_Google"),
                rs.getString("Remarks"));
    }
}
