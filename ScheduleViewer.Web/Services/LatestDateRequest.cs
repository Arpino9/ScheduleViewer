namespace ScheduleViewer.Web.Services;

/// <summary>日付単位の非同期読込で、最新の要求だけを画面状態へ反映させます。</summary>
public sealed class LatestDateRequest : IDisposable
{
    private CancellationTokenSource? current;

    public Request Begin(DateOnly date)
    {
        current?.Cancel();
        current?.Dispose();
        current = new CancellationTokenSource();
        return new Request(date, current.Token);
    }

    public bool IsCurrent(Request request)
        => current is not null &&
           current.Token == request.CancellationToken &&
           !request.CancellationToken.IsCancellationRequested;

    public void Dispose()
    {
        current?.Cancel();
        current?.Dispose();
        current = null;
    }

    public readonly record struct Request(DateOnly Date, CancellationToken CancellationToken);
}
