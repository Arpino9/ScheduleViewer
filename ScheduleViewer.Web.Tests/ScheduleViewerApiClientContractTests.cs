using System.Net;
using System.Text;
using System.Text.Json;
using ScheduleViewer.Web.Services;

namespace ScheduleViewer.Web.Tests;

public sealed class ScheduleViewerApiClientContractTests
{
    [Fact]
    public async Task CalendarContractMapsEventId()
    {
        var handler = new StubHttpMessageHandler(request =>
        {
            Assert.Equal("api/calendar?date=2026-08-10", request.RequestUri!.PathAndQuery.TrimStart('/'));
            return Json("[{\"eventId\":\"event-123\",\"title\":\"予定\",\"displayTitle\":\"予定\",\"startDate\":\"2026-08-10T10:00:00\",\"endDate\":\"2026-08-10T11:00:00\"}]");
        });
        var client = CreateClient(handler);

        var schedules = await client.GetSchedulesAsync(new DateOnly(2026, 8, 10));

        Assert.Equal("event-123", Assert.Single(schedules).EventId);
    }

    [Fact]
    public async Task PhotoContractPostsExpectedRouteAndBody()
    {
        var handler = new StubHttpMessageHandler(async request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("api/calendar/events/event-123/photo", request.RequestUri!.PathAndQuery.TrimStart('/'));
            Assert.Equal("{\"photoUrl\":\"https://photos.google.com/share/example\"}", await request.Content!.ReadAsStringAsync());
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        });
        var client = CreateClient(handler);

        await client.AddPhotoUrlAsync("event-123", "https://photos.google.com/share/example");
    }

    [Fact]
    public async Task BoxContractPostsExpectedMetadataWithoutUploadingAFile()
    {
        var handler = new StubHttpMessageHandler(async request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("api/calendar/events/event-123/attachments", request.RequestUri!.PathAndQuery.TrimStart('/'));
            Assert.Equal("{\"fileUrl\":\"https://app.box.com/s/example\",\"fileTitle\":\"contract.pdf\"}", await request.Content!.ReadAsStringAsync());
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        });
        var client = CreateClient(handler);

        await client.AttachBoxFileAsync(
            "event-123", "https://app.box.com/s/example", "contract.pdf");
    }

    [Fact]
    public async Task CalendarRegistrationPostsTimedEventContract()
    {
        var handler = new StubHttpMessageHandler(async request =>
        {
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("api/calendar/events", request.RequestUri!.PathAndQuery.TrimStart('/'));
            using var body = JsonDocument.Parse(await request.Content!.ReadAsStringAsync());
            Assert.Equal("歯医者", body.RootElement.GetProperty("title").GetString());
            Assert.Equal("2026-08-16", body.RootElement.GetProperty("date").GetString());
            Assert.False(body.RootElement.GetProperty("allDay").GetBoolean());
            Assert.Equal("11:30", body.RootElement.GetProperty("startTime").GetString());
            Assert.Equal("12:30", body.RootElement.GetProperty("endTime").GetString());
            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent(
                    "{\"status\":\"ok\",\"message\":\"歯医者 を登録しました\"}",
                    Encoding.UTF8,
                    "application/json")
            };
        });
        var client = CreateClient(handler);

        var message = await client.RegisterCalendarEventAsync(
            "歯医者", new DateOnly(2026, 8, 16), false,
            new TimeOnly(11, 30), new TimeOnly(12, 30), "東京", "定期検診");

        Assert.Equal("歯医者 を登録しました", message);
    }

    [Fact]
    public async Task AuthenticationContractUsesExistingStatusAndAuthorizationRoutes()
    {
        var requestNumber = 0;
        var handler = new StubHttpMessageHandler(request =>
        {
            requestNumber++;
            if (requestNumber == 1)
            {
                Assert.Equal(HttpMethod.Get, request.Method);
                Assert.Equal("api/auth/status", request.RequestUri!.PathAndQuery.TrimStart('/'));
                return Json("{\"calendar\":true,\"fitbit\":false}");
            }

            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("api/auth/google/fitbit", request.RequestUri!.PathAndQuery.TrimStart('/'));
            return Json("{\"status\":\"pending\",\"url\":\"https://www.fitbit.com/oauth2/authorize\"}");
        });
        var client = CreateClient(handler);

        var status = await client.GetAuthStatusAsync();
        var authorization = await client.AuthorizeServiceAsync("fitbit");

        Assert.True(status["calendar"]);
        Assert.False(status["fitbit"]);
        Assert.Equal("pending", authorization.Status);
        Assert.Equal("https://www.fitbit.com/oauth2/authorize", authorization.Url);
    }

    [Fact]
    public async Task GoogleHealthImportAuthenticationUsesDedicatedRoutes()
    {
        var requestNumber = 0;
        var handler = new StubHttpMessageHandler(request =>
        {
            requestNumber++;
            if (requestNumber == 1)
            {
                Assert.Equal(HttpMethod.Get, request.Method);
                Assert.Equal("api/fitbit/import/status", request.RequestUri!.PathAndQuery.TrimStart('/'));
                return Json("{\"authorized\":true}");
            }

            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("api/fitbit/import/auth?force=true", request.RequestUri!.PathAndQuery.TrimStart('/'));
            return Json("{\"status\":\"pending\",\"url\":\"https://accounts.google.com/o/oauth2/auth\"}");
        });
        var client = CreateClient(handler);

        var authorized = await client.GetGoogleHealthImportStatusAsync();
        var authorization = await client.AuthorizeGoogleHealthImportAsync(force: true);

        Assert.True(authorized);
        Assert.Equal("pending", authorization.Status);
        Assert.Equal("https://accounts.google.com/o/oauth2/auth", authorization.Url);
    }

    private static ScheduleViewerApiClient CreateClient(HttpMessageHandler handler)
        => new(new HttpClient(handler) { BaseAddress = new Uri("http://localhost:9080/") });

    private static HttpResponseMessage Json(string body)
        => new(HttpStatusCode.OK)
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json")
        };

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> asyncHandler;

        public StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
            => asyncHandler = request => Task.FromResult(handler(request));

        public StubHttpMessageHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler)
            => asyncHandler = handler;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => asyncHandler(request);
    }
}
