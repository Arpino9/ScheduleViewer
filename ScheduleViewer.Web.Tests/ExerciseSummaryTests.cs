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

public sealed class ExerciseSummaryTests
{
    public static ExerciseRecord Record => new("STRENGTH_TRAINING", "筋力トレーニング",
        DateTimeOffset.Parse("2026-10-02T12:45:00+09:00"), DateTimeOffset.Parse("2026-10-02T12:58:00+09:00"),
        780, true, "チェストプレス<br>30kg × 10回", "users/123/dataTypes/exercise/dataPoints/456", false, 53);
    public static ExerciseDetailRecord Sample(string status = "available") => new(Record, 128,
        [new("light", 0), new("moderate", 780), new("vigorous", 0), new("peak", 0)],
        [new(Record.StartTime, 120), new(Record.StartTime.AddMinutes(1), 140), new(Record.StartTime.AddMinutes(4), 128)], status);
    private sealed class Handler(ExerciseDetailRecord value) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Assert.Equal("/api/fitbit/exercises/detail?name=users%2F123%2FdataTypes%2Fexercise%2FdataPoints%2F456", request.RequestUri!.PathAndQuery);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(value) });
        }
    }
    [Fact]
    public async Task SummaryRendersMetricsLineGraphZoneBarsAndNotes()
    {
        var html = await Render(Sample());
        Assert.Contains("53 kcal", html);
        Assert.Contains("128 bpm", html);
        Assert.Contains("<svg", html);
        Assert.Contains("<path", html);
        Assert.Contains("<circle", html);
        Assert.Contains("各ゾーンの時間", WebUtility.HtmlDecode(html));
        Assert.Contains("100%", html);
        Assert.Contains("exercise-zone-fill moderate", html);
        Assert.Contains("<br", html);
        Assert.Contains("30kg", html);
    }
    [Theory]
    [InlineData("empty", "心拍データはありません")]
    [InlineData("permission_denied", "読み取り権限が不足")]
    [InlineData("failed", "取得できませんでした")]
    public async Task HeartFailuresKeepSummaryVisible(string status, string expected)
    {
        var html = await Render(Sample(status) with { HeartRate = [], Zones = [new("light", null)] });
        Assert.Contains(expected, WebUtility.HtmlDecode(html));
        Assert.Contains("53 kcal", html);
        Assert.Contains("未取得", WebUtility.HtmlDecode(html));
        Assert.DoesNotContain("<svg", html);
        Assert.DoesNotContain("100%", html);
    }
    [Fact]
    public void ChartKeepsActualTimesAndSeparatesGapsAndFiltersInvalidPoints()
    {
        var data = Sample();
        var chart = new ExerciseHeartChart(data.HeartRate.Concat([
            new ExerciseHeartPoint(Record.EndTime, 200), new ExerciseHeartPoint(Record.StartTime, double.NaN)]).ToArray(), Record.StartTime, Record.EndTime);
        Assert.Equal(2, chart.Paths.Count);
        Assert.Equal(3, chart.Dots.Count);
        Assert.DoesNotContain("NaN", string.Join(" ", chart.Paths));
        Assert.All(chart.Paths, p => Assert.StartsWith("M", p));
    }
    [Fact]
    public async Task CardOffersDetailsForSelectedRecord()
    {
        using var services = new ServiceCollection().AddLogging().BuildServiceProvider();
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        var html = await renderer.Dispatcher.InvokeAsync(async () => (await renderer.RenderComponentAsync<ExerciseSessionList>(
            ParameterView.FromDictionary(new Dictionary<string, object?> {
                ["Records"] = new[] { Record }, ["OnOpenDetails"] = EventCallback.Factory.Create<ExerciseRecord>(new object(), _ => { }) }))).ToHtmlString());
        Assert.Contains("詳細を開く", WebUtility.HtmlDecode(html));
        Assert.Contains("exercise-session", html);
    }
    private static async Task<string> Render(ExerciseDetailRecord data)
    {
        using var http = new HttpClient(new Handler(data)) { BaseAddress = new Uri("http://localhost/") };
        using var services = new ServiceCollection().AddLogging().AddSingleton(new ScheduleViewerApiClient(http)).BuildServiceProvider();
        await using var renderer = new HtmlRenderer(services, services.GetRequiredService<ILoggerFactory>());
        return await renderer.Dispatcher.InvokeAsync(async () => (await renderer.RenderComponentAsync<ExerciseSummary>(
            ParameterView.FromDictionary(new Dictionary<string, object?> { ["Record"] = Record }))).ToHtmlString());
    }
}
