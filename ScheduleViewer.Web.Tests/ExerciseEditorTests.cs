using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ScheduleViewer.Web.Components;
using ScheduleViewer.Web.Models;
using ScheduleViewer.Web.Services;

namespace ScheduleViewer.Web.Tests;

public sealed class ExerciseEditorTests
{
    private sealed class TypesHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            { Content = JsonContent.Create(new[] { new ExerciseTypeOption("STRENGTH_TRAINING", "筋力トレーニング") }) });
    }

    [Fact]
    public async Task AddFormHasTypeTokyoDatesAndNotes()
    {
        var html = WebUtility.HtmlDecode(await Render(false, []));
        Assert.Contains("運動種別", html);
        Assert.Contains("開始日時（日本時間）", html);
        Assert.Contains("終了日時（日本時間）", html);
        Assert.Contains("Notes（詳細）", html);
        Assert.Contains("2026-04-11T12:00:00", html);
        Assert.Contains("追加する", html);
    }

    [Fact]
    public async Task EditFormSelectsOwnedRecordAndPrefillsTokyoTimesAndEncodedNotes()
    {
        var start = DateTimeOffset.Parse("2026-04-11T14:30:00Z");
        var record = new ExerciseRecord("STRENGTH_TRAINING", "Strength", start, start.AddHours(1), 1800, true,
            "30kg\n<script>private</script>", "users/123/dataTypes/exercise/dataPoints/456", true);
        var html = await Render(true, [record]);
        Assert.Contains("2026-04-11T23:30:00", html);
        Assert.Contains("2026-04-12T00:30:00", html);
        Assert.Contains("30kg\n<script>private</script>", WebUtility.HtmlDecode(html));
        Assert.DoesNotContain("<script>", html);
        Assert.Contains("更新する", WebUtility.HtmlDecode(html));
    }

    [Fact]
    public async Task ForeignRecordsAreNotOfferedAsEditable()
    {
        var start = DateTimeOffset.Parse("2026-04-11T14:30:00Z");
        var record = new ExerciseRecord("STRENGTH_TRAINING", "Foreign", start, start.AddHours(1), 1800, true,
            "foreign notes", "users/123/dataTypes/exercise/dataPoints/999", false);
        var html = WebUtility.HtmlDecode(await Render(true, [record]));
        Assert.Contains("この日に編集できる記録はありません", html);
        Assert.DoesNotContain("foreign notes", html);
        Assert.DoesNotContain("value=\"users/123/dataTypes/exercise/dataPoints/999\"", html);
    }

    private static async Task<string> Render(bool edit, ExerciseRecord[] records)
    {
        using var http = new HttpClient(new TypesHandler()) { BaseAddress = new Uri("http://localhost/") };
        using var services = new ServiceCollection().AddLogging().AddSingleton(new ScheduleViewerApiClient(http)).BuildServiceProvider();
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        return await renderer.Dispatcher.InvokeAsync(async () =>
        {
            var root = await renderer.RenderComponentAsync<ExerciseEditor>(ParameterView.FromDictionary(
                new Dictionary<string, object?> { ["Edit"] = edit, ["Date"] = new DateOnly(2026, 4, 11), ["Records"] = records }));
            return root.ToHtmlString();
        });
    }
}
