using Reactive.Bindings;
using ScheduleViewer.WPF.Helpers;

namespace ScheduleViewerTest;

public sealed class ReactiveCollectionUtilsTests
{
    [Fact]
    public void ToReactiveCollectionCreatesCollectionWithSourceItems()
    {
        var source = new[] { 1, 2, 3 };

        var result = source.ToReactiveCollection();

        Assert.Equal(source, result);
    }

    [Fact]
    public void ToReactiveCollectionUpdatesExistingInstance()
    {
        var target = new ReactiveCollection<int> { 9, 8 };

        var result = new[] { 1, 2 }.ToReactiveCollection(target);

        Assert.Same(target, result);
        Assert.Equal(new[] { 1, 2 }, result);
    }

    [Fact]
    public void ToReactiveCollectionClearsExistingInstanceForEmptySource()
    {
        var target = new ReactiveCollection<int> { 9, 8 };

        var result = Array.Empty<int>().ToReactiveCollection(target);

        Assert.Same(target, result);
        Assert.Empty(result);
    }
}
