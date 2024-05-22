namespace ScheduleViewer.Domain.Repositories;

/// <summary>
/// Repository - 職歴
/// </summary>
public interface ICareerRepository
{
    /// <summary> 
    /// Get - 職歴
    /// </summary>
    /// <returns>Entity - 支給額</returns>
    IReadOnlyList<CareerEntity> GetEntities();

    /// <summary>
    /// Get - 職歴
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>職歴</returns>
    CareerEntity GetEntity(int id);

    /// <summary>
    /// 保存
    /// </summary>
    /// <param name="entity">職歴</param>
    public void Save(CareerEntity entity);

    /// <summary>
    /// 削除
    /// </summary>
    /// <param name="id">ID</param>
    public void Delete(int id);
}
