using System.Windows.Media;
using ScheduleViewer.WPF.Models.Presentation;

namespace ScheduleViewerTest;

public sealed class WorkScheduleItemTests
{
    [Fact]
    public void ConstructorUsesEmptyDisplayValuesByDefault()
    {
        var item = new WorkScheduleItem("9/6(日)", Brushes.White);

        Assert.Equal("9/6(日)", item.Day);
        Assert.Same(Brushes.White, item.Background);
        Assert.Equal(string.Empty, item.StartTime);
        Assert.Equal(string.Empty, item.Notification);
        Assert.Equal(string.Empty, item.Remarks);
    }

    [Fact]
    public void ConstructorPopulatesWorkScheduleDisplayValues()
    {
        var item = new WorkScheduleItem(
            "9/7(月)",
            Brushes.LightYellow,
            startTime: "09:00",
            endTime: "18:00",
            lunchTime: "01:00",
            notification: "在宅",
            workingTime: "08:00",
            overtime: "00:00",
            midnightTime: "00:00",
            absentedTime: "00:00",
            remarks: "終日リモート");

        Assert.Equal("09:00", item.StartTime);
        Assert.Equal("18:00", item.EndTime);
        Assert.Equal("在宅", item.Notification);
        Assert.Equal("08:00", item.WorkingTime);
        Assert.Equal("終日リモート", item.Remarks);
    }
}
