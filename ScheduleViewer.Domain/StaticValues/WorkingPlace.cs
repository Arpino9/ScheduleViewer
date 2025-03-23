namespace ScheduleViewer.Domain.StaticValues;

/// <summary>
/// Static Values - 就業場所
/// </summary>
public static class WorkingPlace
{
    private static List<WorkingPlaceEntity> _entities = new List<WorkingPlaceEntity>();

    /// <summary>
    /// テーブル取得
    /// </summary>
    /// <param name="repository">Repository</param>
    /// <remarks>
    /// 競合防止のためlockをかけており、常に最新の情報が取得できる。
    /// </remarks>
    public static void Create(IWorkingPlaceRepository repository)
    {
        lock (((ICollection)_entities).SyncRoot)
        {
            _entities.Clear();
            _entities.AddRange(repository.GetEntities());
        }
    }

    /// <summary>
    /// IDから就業場所を取得
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>就業場所</returns>
    public static WorkingPlaceEntity FetchByID(int id)
        => _entities.Find(x => x.ID == id);

    /// <summary>
    /// 日付から就業場所を取得
    /// </summary>
    /// <param name="date">日付</param>
    /// <returns>就業場所</returns>
    public static IReadOnlyList<WorkingPlaceEntity> FetchByDate(DateOnly date)
        => _entities.Where(x => x.WorkingStart <= date && date <= x.WorkingEnd).ToList().AsReadOnly();

    /// <summary>
    /// 会社名から就業場所を取得
    /// </summary>
    /// <param name="companyName">ID</param>
    /// <returns>就業場所</returns>
    public static WorkingPlaceEntity FetchByCompany(string companyName)
        => _entities.Find(x => x.WorkingPlace_Name.Text.Contains(companyName));

    /// <summary>
    /// 昇順で取得する
    /// </summary>
    /// <returns>就業場所</returns>
    public static IReadOnlyList<WorkingPlaceEntity> FetchByAscending()
        => _entities.OrderBy(x => x.ID).ToList().AsReadOnly();

    /// <summary>
    /// 降順で取得する
    /// </summary>
    /// <returns>就業場所</returns>
    public static IReadOnlyList<WorkingPlaceEntity> FetchByDescending()
        => _entities.OrderByDescending(x => x.ID).ToList().AsReadOnly();
}
