namespace ScheduleViewer.WPF.Interface;

/// <summary>
/// Interface - 閲覧用
/// </summary>
/// <remarks>
/// 機能がInfrastructureからの読込のみ
/// </remarks>
internal interface IViewer
{
    public void ListView_SelectionChanged();
}
