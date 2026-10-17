using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ScheduleViewer.Web.Components;
using ScheduleViewer.Web.Models;

namespace ScheduleViewer.Web.Tests;

public sealed class ExerciseRenderingTests
{
    [Fact]
    public async Task NotesAreEncodedAndOvernightTimesAreRenderedInTokyo()
    {
        using var services = new ServiceCollection().AddLogging().BuildServiceProvider();
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        var start = DateTimeOffset.Parse("2026-04-11T14:30:00Z");
        ExerciseRecord[] records = [new("STRENGTH_TRAINING", "Strength", start, start.AddHours(1),
            1800.5, true, "40kg × 10\n<script>alert(1)</script> & <b>notes</b>")];
        var html = await renderer.Dispatcher.InvokeAsync(async () =>
        {
            var component = await renderer.RenderComponentAsync<ExerciseSessionList>(
                ParameterView.FromDictionary(new Dictionary<string, object?> { ["Records"] = records }));
            return component.ToHtmlString();
        });
        var decoded = System.Net.WebUtility.HtmlDecode(html);
        Assert.Contains("23:30～00:30", decoded);
        Assert.Contains("💪", decoded);
        Assert.Contains("筋力トレーニング", decoded);
        Assert.DoesNotContain("STRENGTH_TRAINING", decoded);
        Assert.DoesNotContain("日本時間", decoded);
        Assert.DoesNotContain("2026/", decoded);
        var notes = System.Text.RegularExpressions.Regex.Match(html,
            "<div class=\"exercise-notes\"[^>]*>(.*?)</div>", System.Text.RegularExpressions.RegexOptions.Singleline);
        Assert.True(notes.Success);
        Assert.Equal(records[0].Notes, System.Net.WebUtility.HtmlDecode(notes.Groups[1].Value));
        Assert.DoesNotContain("<script>", html);
        Assert.Contains("exercise-notes", html);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0d)]
    [InlineData(123.4d)]
    [InlineData(-1d)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public async Task CaloriesShowValueOrUnavailableBelowDuration(double? calories)
    {
        using var services = new ServiceCollection().AddLogging().BuildServiceProvider();
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        var start = DateTimeOffset.Parse("2026-10-17T00:44:39Z");
        ExerciseRecord[] records = [new("WALKING", "Walking", start,
            DateTimeOffset.Parse("2026-10-17T01:02:34Z"), 1075, true, "", CaloriesKcal: calories)];
        var html = await renderer.Dispatcher.InvokeAsync(async () =>
        {
            var root = await renderer.RenderComponentAsync<ExerciseSessionList>(ParameterView.FromDictionary(
                new Dictionary<string, object?> { ["Records"] = records }));
            return System.Net.WebUtility.HtmlDecode(root.ToHtmlString());
        });
        Assert.Contains("09:44～10:02", html);
        Assert.Contains("🚶", html);
        Assert.Contains("ウォーキング", html);
        Assert.DoesNotContain("WALKING", html);
        Assert.True(html.IndexOf("消費カロリー", StringComparison.Ordinal) > html.IndexOf("運動時間", StringComparison.Ordinal));
        if (calories is null || !double.IsFinite(calories.Value) || calories < 0)
            Assert.Contains("消費カロリー：未算出", html);
        else
        {
            Assert.Contains($"消費カロリー：{calories.Value:0.#} kcal", html);
            Assert.DoesNotContain("未算出", html);
        }
    }
}
