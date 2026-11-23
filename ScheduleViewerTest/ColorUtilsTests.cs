using ScheduleViewer.Domain.ValueObjects;
using ScheduleViewer.WPF.Helpers;

namespace ScheduleViewerTest;

public sealed class ColorUtilsTests
{
    [Fact]
    public void ToWpfBrushCopiesArgbComponents()
    {
        var color = new ArgbColorValue(128, 10, 20, 30);

        var brush = color.ToWpfBrush();

        Assert.Equal(128, brush.Color.A);
        Assert.Equal(10, brush.Color.R);
        Assert.Equal(20, brush.Color.G);
        Assert.Equal(30, brush.Color.B);
    }
}
