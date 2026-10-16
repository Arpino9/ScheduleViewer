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
        Assert.Contains("2026/04/11 23:30:00", html);
        Assert.Contains("2026/04/12 00:30:00", html);
        var notes = System.Text.RegularExpressions.Regex.Match(html,
            "<div class=\"exercise-notes\"[^>]*>(.*?)</div>", System.Text.RegularExpressions.RegexOptions.Singleline);
        Assert.True(notes.Success);
        Assert.Equal(records[0].Notes, System.Net.WebUtility.HtmlDecode(notes.Groups[1].Value));
        Assert.DoesNotContain("<script>", html);
        Assert.Contains("exercise-notes", html);
    }
}
