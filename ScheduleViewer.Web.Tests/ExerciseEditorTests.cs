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
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Exercise the initial render before the options arrive.
            await Task.Yield();
            return new HttpResponseMessage(HttpStatusCode.OK)
            { Content = JsonContent.Create(new[] {
                new ExerciseTypeOption("BIKING", "サイクリング"),
                new ExerciseTypeOption("STRENGTH_TRAINING", "筋力トレーニング") }) };
        }
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
        AssertSelectedType(html, "STRENGTH_TRAINING");
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
        AssertSelectedType(html, "STRENGTH_TRAINING");
    }

    [Fact]
    public async Task EditFormSelectsStoredTypeInsteadOfDefault()
    {
        var start = DateTimeOffset.Parse("2026-04-11T14:30:00Z");
        var record = new ExerciseRecord("BIKING", "サイクリング", start, start.AddHours(1), 1800, true,
            "", "users/123/dataTypes/exercise/dataPoints/456", true);
        AssertSelectedType(await Render(true, [record]), "BIKING");
    }

    private static void AssertSelectedType(string html, string type)
    {
        var select = System.Text.RegularExpressions.Regex.Match(html,
            "<select id=\"exercise-type\"[^>]*>(.*?)</select>", System.Text.RegularExpressions.RegexOptions.Singleline);
        Assert.True(select.Success);
        var options = System.Text.RegularExpressions.Regex.Matches(select.Groups[1].Value, "<option\\b[^>]*>");
        var selected = Assert.Single(options.Cast<System.Text.RegularExpressions.Match>(),
            x => System.Text.RegularExpressions.Regex.IsMatch(x.Value, "\\bselected(?:[\\s=>])"));
        Assert.Contains($"value=\"{type}\"", selected.Value);
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
