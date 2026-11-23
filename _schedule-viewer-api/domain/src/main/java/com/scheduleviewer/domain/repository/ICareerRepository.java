package com.scheduleviewer.domain.repository;

import com.scheduleviewer.domain.entity.CareerEntity;

import java.util.List;

/**
 * Repository - 職歴
 */
public interface ICareerRepository {

    /** 職歴一覧を取得する */
    List<CareerEntity> getEntities();

    /** 職歴を取得する */
    CareerEntity getEntity(int id);

    /** 保存する */
    void save(CareerEntity entity);

    /** 削除する */
    void delete(int id);
}
