using ScheduleViewer.Domain.Entities;

namespace ScheduleViewerTest;

public sealed class CalendarEventsEntityTests
{
    [Fact]
    public void ConstructorPopulatesDtoAlignedProperties()
    {
        var startDate = new DateTime(2026, 8, 23, 0, 0, 0);
        var endDate = startDate.AddDays(1);
        var attachment = new CalendarAttachmentEntity(
            startDate,
            "資料",
            "https://example.com/file",
            "application/pdf");

        var calendarEvent = new CalendarEventsEntity(
            eventId: "event-123",
            title: "読書",
            startDate: startDate,
            endDate: endDate,
            isAllDay: true,
            description: "【出版社】出版社",
            achievementImageUrl: "https://example.com/achievement.png",
            attachments: [attachment]);

        Assert.Equal("event-123", calendarEvent.EventId);
        Assert.Equal("読書", calendarEvent.DisplayTitle);
        Assert.Equal(startDate, calendarEvent.ProgressingStartDate);
        Assert.Equal("https://example.com/achievement.png", calendarEvent.AchievementImageUrl);
        Assert.Same(attachment, Assert.Single(calendarEvent.Attachments));
        Assert.True(calendarEvent.IsBook);
        Assert.False(calendarEvent.IsProgram);
        Assert.False(calendarEvent.IsAllDayEvent);
    }

    [Fact]
    public void ConstructorCanSetProgressDisplayWithoutAnotherOverload()
    {
        var originalStartDate = new DateTime(2026, 8, 23, 10, 0, 0);
        var progressingStartDate = originalStartDate.AddMinutes(30);

        var calendarEvent = new CalendarEventsEntity(
            eventId: "event-456",
            title: "予定",
            startDate: progressingStartDate,
            endDate: originalStartDate.AddHours(1),
            displayTitle: "↓",
            progressingStartDate: originalStartDate);

        Assert.Equal("↓", calendarEvent.DisplayTitle);
        Assert.Equal(progressingStartDate, calendarEvent.StartDate);
        Assert.Equal(originalStartDate, calendarEvent.ProgressingStartDate);
    }
}
