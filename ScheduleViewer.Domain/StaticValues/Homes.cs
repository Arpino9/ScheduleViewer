namespace ScheduleViewer.Domain.StaticValues;

/// <summary>
/// Static Values - 自宅
/// </summary>
public static class Homes
{
    private static List<HomeEntity> _entities = new List<HomeEntity>();

    /// <summary>
    /// テーブル取得
    /// </summary>
    /// <param name="repository">Repository</param>
    /// <remarks>
    /// 競合防止のためlockをかけており、常に最新の情報が取得できる。
    /// </remarks>
    public static void Create(IHomeRepository repository)
    {
        lock (((ICollection)_entities).SyncRoot)
        {
            _entities.Clear();
            _entities.AddRange(repository.GetEntities());
        }
    }

    /// <summary>
    /// 会社を取得
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>職歴</returns>
    public static HomeEntity Fetch(int id)
        => _entities?.Find(x => x.ID == id);

    /// <summary>
    /// 昇順で取得する
    /// </summary>
    /// <returns>職歴</returns>
    public static IReadOnlyList<HomeEntity> FetchByAscending()
        => _entities?.OrderBy(x => x.ID).ToList().AsReadOnly();

    /// <summary>
    /// 降順で取得する
    /// </summary>
    /// <returns>職歴</returns>
    public static IReadOnlyList<HomeEntity> FetchByDescending()
        => _entities?.OrderByDescending(x => x.ID).ToList().AsReadOnly();

    /// <summary>
    /// 日付から在住場所を取得
    /// </summary>
    /// <param name="date">日付</param>
    /// <returns>就業場所</returns>
    public static HomeEntity FetchByDate(DateOnly date)
        => _entities.Where(x => x.LivingStart <= date && date <= x.LivingEnd).FirstOrDefault();
}