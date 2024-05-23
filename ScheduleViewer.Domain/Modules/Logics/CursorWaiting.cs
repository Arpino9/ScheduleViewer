namespace ScheduleViewer.Domain.Modules.Logics;

/// <summary>
/// カーソル待ち
/// </summary>
/// <remarks>
/// 簡易的な実装なので、必要があればコンストラクタのlock化を検討。
/// </remarks>
public class CursorWaiting : IDisposable
{
    /// <summary> 待機中か </summary>
    public bool IsWaiting => (Cursor.Current == Cursors.WaitCursor);

    public CursorWaiting()
    {
        Cursor.Current = Cursors.WaitCursor;
    }

    public void Dispose()
    {
        Cursor.Current = Cursors.Default;
    }
}
