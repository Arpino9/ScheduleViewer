namespace ScheduleViewer.WPF.Helpers;

/// <summary>
/// ReactiveCollectionを扱うための拡張メソッドを提供します。
/// </summary>
public static class ReactiveCollectionUtils
{
    /// <summary>
    /// コレクションの要素で既存のReactiveCollectionを更新します。
    /// </summary>
    /// <typeparam name="T">要素の型。</typeparam>
    /// <param name="source">コピー元のコレクション。</param>
    /// <param name="target">更新するReactiveCollection。</param>
    /// <returns>更新したReactiveCollection。</returns>
    /// <remarks>
    /// ReactiveCollectionのインスタンスを維持したまま内容を入れ替えるため、
    /// 既存のバインディングへ変更が通知されます。
    /// </remarks>
    public static ReactiveCollection<T> ToReactiveCollection<T>(
        this IEnumerable<T> source,
        ReactiveCollection<T> target)
    {
        target.Clear();

        foreach (var item in source)
        {
            target.Add(item);
        }

        return target;
    }

    /// <summary>
    /// コレクションの要素を格納したReactiveCollectionを作成します。
    /// </summary>
    /// <typeparam name="T">要素の型。</typeparam>
    /// <param name="source">コピー元のコレクション。</param>
    /// <returns>作成したReactiveCollection。</returns>
    public static ReactiveCollection<T> ToReactiveCollection<T>(this IEnumerable<T> source)
    {
        return source.ToReactiveCollection(new ReactiveCollection<T>());
    }
}
