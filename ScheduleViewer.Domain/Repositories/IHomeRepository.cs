namespace ScheduleViewer.Domain.Repositories;

/// <summary>
/// Repository - 自宅
/// </summary>
public interface IHomeRepository
{
    /// <summary> 
    /// Get - 自宅
    /// </summary>
    /// <returns>Entity - 自宅</returns>
    IReadOnlyList<HomeEntity> GetEntities();

    /// <summary>
    /// Get - 自宅
    /// </summary>
    /// <returns>自宅</returns>
    HomeEntity GetEntity(int id);

    /// <summary>
    /// 保存
    /// </summary>
    /// <param name="entity">エンティティ</param>
    public void Save(HomeEntity entity);

    /// <summary>
    /// 削除
    /// </summary>
    /// <param name="id">ID</param>
    public void Delete(int id);
}