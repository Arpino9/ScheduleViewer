namespace ScheduleViewer.Web.Services;

/// <summary>
/// 日付単位の非同期読込を管理し、最新の要求だけを画面状態へ反映できるようにします。
/// </summary>
public sealed class LatestDateRequest : IDisposable
{
    private CancellationTokenSource? current;

    /// <summary>指定日の新しい読込を開始し、直前の読込をキャンセルします。</summary>
    /// <param name="date">読込対象の日付。</param>
    /// <returns>対象日とキャンセルトークンを保持する要求。</returns>
    public Request Begin(DateOnly date)
    {
        current?.Cancel();
        current?.Dispose();
        current = new CancellationTokenSource();
        return new Request(date, current.Token);
    }

    /// <summary>要求が現在の最新要求で、キャンセルされていないかを判定します。</summary>
    /// <param name="request">判定する要求。</param>
    /// <returns>画面状態へ反映できる最新要求である場合は<see langword="true"/>。</returns>
    public bool IsCurrent(Request request)
        => current is not null &&
           current.Token == request.CancellationToken &&
           !request.CancellationToken.IsCancellationRequested;

    /// <summary>実行中の要求をキャンセルし、関連リソースを解放します。</summary>
    public void Dispose()
    {
        current?.Cancel();
        current?.Dispose();
        current = null;
    }

    /// <summary>日付単位の読込要求。</summary>
    /// <param name="Date">読込対象の日付。</param>
    /// <param name="CancellationToken">要求のキャンセルトークン。</param>
    public readonly record struct Request(DateOnly Date, CancellationToken CancellationToken);
}
