package com.scheduleviewer.domain.repository;

import com.scheduleviewer.domain.entity.WorkingPlaceEntity;

import java.util.List;

/**
 * Repository - 就業場所
 */
public interface IWorkingPlaceRepository {

    /** 就業場所一覧を取得する */
    List<WorkingPlaceEntity> getEntities();

    /** 就業場所を取得する */
    WorkingPlaceEntity getEntity(int id);

    /** 保存する */
    void save(WorkingPlaceEntity entity);

    /** 削除する */
    void delete(int id);
}
