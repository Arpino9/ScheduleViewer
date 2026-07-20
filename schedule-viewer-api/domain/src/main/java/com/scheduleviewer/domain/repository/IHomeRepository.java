package com.scheduleviewer.domain.repository;

import com.scheduleviewer.domain.entity.HomeEntity;

import java.util.List;

/**
 * Repository - 自宅
 */
public interface IHomeRepository {

    /** 自宅一覧を取得する */
    List<HomeEntity> getEntities();

    /** 自宅を取得する */
    HomeEntity getEntity(int id);

    /** 保存する */
    void save(HomeEntity entity);

    /** 削除する */
    void delete(int id);
}
