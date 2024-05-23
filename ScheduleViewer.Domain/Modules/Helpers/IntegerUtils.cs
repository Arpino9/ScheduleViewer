namespace ScheduleViewer.Domain.Modules.Helpers;

/// <summary>
/// 拡張クラス - Integer
/// </summary>
public static class IntegerUtils
{
    /// <summary>
    /// コレクションが選択されていないか
    /// </summary>
    /// <param name="selectedIndex">コレクションのインデックス</param>
    /// <returns>
    /// True : コレクション未選択 / False: コレクション選択済
    /// </returns>
    public static bool IsUnSelected(this int selectedIndex)
        => (selectedIndex == -1);
}
