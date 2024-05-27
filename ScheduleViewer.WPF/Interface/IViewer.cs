namespace ScheduleViewer.WPF.Interface;

/// <summary>
/// Interface - 閲覧用
/// </summary>
/// <remarks>
/// 機能がInfrastructureからの読込のみ
/// </remarks>
internal interface IViewer
{
    /// <summary>
    /// 初期化
    /// </summary>
    void Initialize();

    /// <summary>
    /// ListView - SelectionChanged
    /// </summary>
    void ListView_SelectionChanged();

    /// <summary>
    /// Clear - 閲覧項目
    /// </summary>
    void Clear_ViewForm();
}
