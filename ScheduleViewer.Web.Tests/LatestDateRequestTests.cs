using ScheduleViewer.Web.Services;

namespace ScheduleViewer.Web.Tests;

public sealed class LatestDateRequestTests
{
    [Fact]
    public async Task OlderDateCannotCommitAfterNewerDateStarts()
    {
        using var requests = new LatestDateRequest();
        var displayedDate = default(DateOnly);
        var firstCanFinish = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        var firstRequest = requests.Begin(new DateOnly(2026, 8, 10));
        var firstLoad = Task.Run(async () =>
        {
            await firstCanFinish.Task;
            if (requests.IsCurrent(firstRequest)) displayedDate = firstRequest.Date;
        });

        var secondRequest = requests.Begin(new DateOnly(2026, 8, 11));
        if (requests.IsCurrent(secondRequest)) displayedDate = secondRequest.Date;
        firstCanFinish.SetResult();
        await firstLoad;

        Assert.True(firstRequest.CancellationToken.IsCancellationRequested);
        Assert.False(requests.IsCurrent(firstRequest));
        Assert.Equal(secondRequest.Date, displayedDate);
    }
}
