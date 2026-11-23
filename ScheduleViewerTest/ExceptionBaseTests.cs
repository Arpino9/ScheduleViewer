using ScheduleViewer.Domain.Exceptions;

namespace ScheduleViewerTest;

public class ExceptionBaseTests
{
    [Fact]
    public void Constructor_SetsMessageAndPresentationMetadata()
    {
        var exception = new FileReaderException("ファイルを読み込めませんでした。");

        Assert.Equal("ファイルを読み込めませんでした。", exception.Message);
        Assert.Equal(nameof(FileReaderException), exception.Title);
        Assert.Equal(ExceptionBase.LogType.Error, exception.Severity);
        Assert.Null(exception.InnerException);
    }

    [Fact]
    public void Constructor_WithInnerException_PreservesCauseAndSeverity()
    {
        var cause = new InvalidOperationException("接続に失敗しました。");

        var exception = new DatabaseException(
            "データベース処理に失敗しました。",
            cause,
            ExceptionBase.LogType.Fatal);

        Assert.Equal("データベース処理に失敗しました。", exception.Message);
        Assert.Equal(nameof(DatabaseException), exception.Title);
        Assert.Equal(ExceptionBase.LogType.Fatal, exception.Severity);
        Assert.Same(cause, exception.InnerException);
    }
}
