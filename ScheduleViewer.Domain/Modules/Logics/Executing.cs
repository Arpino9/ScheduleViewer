namespace ScheduleViewer.Domain.Modules.Logics;

/// <summary>
/// 実行中判定用
/// </summary>
public class Executing : IDisposable
{
    /// <summary> 値 </summary>
    /// <remarks> True : 実行中 / false : 実行前 or 実行後 </remarks>
    public bool Value { get; private set; }

    public Executing()
    {
        this.Value = true;
    }

    public void Dispose()
    {
        this.Value = false;
    }
}