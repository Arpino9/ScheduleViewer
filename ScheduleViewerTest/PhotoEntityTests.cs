using ScheduleViewer.Domain.Entities;

namespace ScheduleViewerTest;

public sealed class PhotoEntityTests
{
    [Fact]
    public void ConstructorStoresImageUrlWithoutWpfImageType()
    {
        var date = new DateTime(2026, 9, 6, 12, 0, 0);

        var photo = new PhotoEntity(
            "photo-1",
            date,
            "photo.jpg",
            "説明",
            "https://example.com/image.jpg",
            "https://example.com/photo/1",
            "image/jpeg",
            1080,
            1920);

        Assert.Equal("photo-1", photo.ID);
        Assert.Equal(date, photo.Date);
        Assert.Equal("https://example.com/image.jpg", photo.ImageUrl);
        Assert.Equal("https://example.com/photo/1", photo.URL);
        Assert.Equal(1080, photo.Height);
        Assert.Equal(1920, photo.Width);
    }
}
