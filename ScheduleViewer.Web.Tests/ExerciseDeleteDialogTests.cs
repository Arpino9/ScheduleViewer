using System.Net;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ScheduleViewer.Web.Components;
using ScheduleViewer.Web.Models;
using ScheduleViewer.Web.Services;

namespace ScheduleViewer.Web.Tests;

public sealed class ExerciseDeleteDialogTests
{
    [Fact]
    public async Task SameTypeSessionsHaveSeparateIdsAndTokyoTimesAndNoAutomaticDeletion()
    {
        var start = DateTimeOffset.Parse("2026-10-17T03:00:00Z");
        ExerciseRecord[] records = [
            new("STRENGTH_TRAINING", "筋力トレーニング", start, start.AddMinutes(30), 1800, true,
                "first", "users/123/dataTypes/exercise/dataPoints/111", true),
            new("STRENGTH_TRAINING", "筋力トレーニング", start.AddHours(1), start.AddHours(1.5), 1800, true,
                "second", "users/123/dataTypes/exercise/dataPoints/222", true),
            new("WALKING", "他アプリ", start, start.AddHours(1), 3600, true,
                "foreign", "users/123/dataTypes/exercise/dataPoints/333", false)];
        var html = WebUtility.HtmlDecode(await Render(records));
        Assert.Contains("value=\"users/123/dataTypes/exercise/dataPoints/111\"", html);
        Assert.Contains("value=\"users/123/dataTypes/exercise/dataPoints/222\"", html);
        Assert.Contains("2026/10/17 12:00:00", html);
        Assert.Contains("2026/10/17 13:00:00", html);
        Assert.DoesNotContain("dataPoints/333", html);
        Assert.Contains("記録を選択してください", html);
        Assert.Matches("<button[^>]*disabled[^>]*>選択した記録を削除</button>", html);
    }

    [Fact]
    public async Task NoOwnedRecordsShowsEmptyState()
    {
        var html = WebUtility.HtmlDecode(await Render([]));
        Assert.Contains("この日に削除できる記録はありません", html);
        Assert.Contains("元のアプリで削除してください", html);
    }

    private static async Task<string> Render(ExerciseRecord[] records)
    {
        using var http = new HttpClient { BaseAddress = new Uri("http://localhost/") };
        using var services = new ServiceCollection().AddLogging().AddSingleton(new ScheduleViewerApiClient(http)).BuildServiceProvider();
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        return await renderer.Dispatcher.InvokeAsync(async () =>
        {
            var root = await renderer.RenderComponentAsync<ExerciseDeleteDialog>(ParameterView.FromDictionary(
                new Dictionary<string, object?> { ["Records"] = records }));
            return root.ToHtmlString();
        });
    }
}
