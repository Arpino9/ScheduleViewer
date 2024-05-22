namespace ScheduleViewer.Domain.Repositories;

/// <summary>
/// Repository - 就業場所
/// </summary>
public interface IWorkingPlaceRepository
{
    /// <summary> 
    /// Get - 就業場所
    /// </summary>
    /// <returns>Entity - 就業場所</returns>
    IReadOnlyList<WorkingPlaceEntity> GetEntities();

    /// <summary>
    /// Get - 就業場所
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>就業場所</returns>
    WorkingPlaceEntity GetEntity(int id);

    /// <summary>
    /// 保存
    /// </summary>
    /// <param name="entity">就業場所</param>
    public void Save(WorkingPlaceEntity entity);

    /// <summary>
    /// 削除
    /// </summary>
    /// <param name="id">ID</param>
    public void Delete(int id);
}
