namespace ScheduleViewer.Domain.Modules.Helpers;

/// <summary>
/// 拡張クラス - List
/// </summary>
public static class ListUtils
{
    /// <summary>
    /// Observable Collectionに変換する
    /// </summary>
    /// <typeparam name="T">型パラメータ</typeparam>
    /// <param name="list">リスト</param>
    /// <returns>ObservableCollection</returns>
    [Obsolete("一応残すが、ToReactiveCollection()メソッドを使うこと")]
    public static ObservableCollection<T> ToObservableCollection<T>(ICollection<T> list)
        => new ObservableCollection<T>(list as List<T>);

    /// <summary>
    /// コレクションが空かどうか調べる
    /// </summary>
    /// <typeparam name="T">型パラメータ</typeparam>
    /// <param name="list">リスト</param>
    /// <returns>
    /// True : コレクションが空である / False: コレクションが空でない
    /// </returns>
    public static bool IsEmpty<T>(this IEnumerable<T> list)
        => (list.Any() == false);

    /// <summary>
    /// 並べ替え
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="keySelector"></param>
    /// <returns></returns>
    public static bool IsSortedAscending<T>(IEnumerable<T> list, Func<T, IComparable> keySelector)
    {
        var listArray = list.ToArray();

        for (int i = 1; i < listArray.Length; i++)
        {
            if (keySelector(listArray[i - 1]).CompareTo(keySelector(listArray[i])) > 0)
            {
                return false;
            }
        }

        return true;
    }
}
