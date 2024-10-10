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
    /// List → Reactive Collection
    /// </summary>
    /// <typeparam name="T">型パラメータ</typeparam>
    /// <param name="list">コレクション</param>
    /// <param name="reactiveCollection">Reactive Collection</param>
    /// <returns>Reactive Collection</returns>
    /// <remarks>
    /// 別のコレクションのSelectedItemなどでReactiveCollectionの中身を入れ替える場合、
    /// こちらのメソッドで引数に更新するReactiveCollectionを指定する。
    /// ※引数なしの方はViewへの通知がされないため
    /// </remarks>
    public static ReactiveCollection<T> ToReactiveCollection<T>(
        this IEnumerable<T> list,
        ReactiveCollection<T> reactiveCollection)
    {
        if (list.IsEmpty())
        {
            return new ReactiveCollection<T>();
        }

        reactiveCollection.Clear();

        foreach (var item in list)
        {
            reactiveCollection.Add(item);
        }

        return reactiveCollection;
    }

    /// <summary>
    /// List → Reactive Collection
    /// </summary>
    /// <typeparam name="T">型パラメータ</typeparam>
    /// <param name="list">コレクション</param>
    /// <returns>Reactive Collection</returns>
    /// <remarks>
    /// 通常のList → ReactiveCollection変換。
    /// </remarks>
    public static ReactiveCollection<T> ToReactiveCollection<T>(this IEnumerable<T> list)
    {
        var reactiveCollection = new ReactiveCollection<T>();

        if (list.IsEmpty())
        {
            return new ReactiveCollection<T>();
        }

        foreach (var item in list)
        {
            reactiveCollection.Add(item);
        }

        return reactiveCollection;
    }

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