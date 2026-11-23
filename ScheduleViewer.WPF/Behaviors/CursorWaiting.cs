namespace ScheduleViewer.WPF.Behaviors;

/// <summary>
/// 処理中だけカーソルを待機状態にします。
/// </summary>
/// <remarks>
/// 簡易的な実装のため、必要に応じてコンストラクターの排他制御を検討します。
/// </remarks>
public sealed class CursorWaiting : IDisposable
{
    /// <summary>
    /// カーソルが待機状態かどうかを取得します。
    /// </summary>
    public bool IsWaiting => Mouse.OverrideCursor == System.Windows.Input.Cursors.Wait;

    /// <summary>
    /// カーソルを待機状態にします。
    /// </summary>
    public CursorWaiting()
    {
        Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
    }

    /// <summary>
    /// カーソルを既定の状態に戻します。
    /// </summary>
    public void Dispose()
    {
        Mouse.OverrideCursor = null;
    }
}
