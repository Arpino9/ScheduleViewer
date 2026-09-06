using System.Windows.Media;
using ScheduleViewer.WPF.Models.Presentation;

namespace ScheduleViewerTest;

public sealed class ScheduleItemTests
{
    [Fact]
    public void ConstructorFormatsCalendarDisplayValues()
    {
        var item = new ScheduleItem(
            Brushes.Black,
            Brushes.White,
            new DateOnly(2026, 9, 6),
            "祝日",
            ["予定1", "予定2", "予定3", "予定4", "予定5", "表示しない予定"]);

        Assert.Equal("6", item.Day_Text);
        Assert.Equal("★祝日", item.AllDayEvent_Text);
        Assert.Equal("予定1", item.DailyEvent1_Text);
        Assert.Equal("予定5", item.DailyEvent5_Text);
    }

    [Fact]
    public void EmptyContainsNoDisplayValues()
    {
        Assert.Equal(default, ScheduleItem.Empty.Date);
        Assert.Equal(string.Empty, ScheduleItem.Empty.Day_Text);
        Assert.Null(ScheduleItem.Empty.AllDayEvent_Text);
        Assert.Null(ScheduleItem.Empty.DailyEvent1_Text);
    }
}
