namespace ScheduleViewer.WPF.Services;

/// <summary>
/// 未処理例外をWPFのダイアログで通知します。
/// </summary>
public static class ExceptionDialog
{
    /// <summary>
    /// デバッグビルドで例外の内容を表示します。
    /// </summary>
    /// <param name="exception">表示する例外。</param>
    [System.Diagnostics.Conditional("DEBUG")]
    public static void Show(Exception exception)
    {
        var title = exception is ExceptionBase applicationException
            ? applicationException.Title
            : exception.GetType().Name;
        var message = exception.InnerException?.ToString() ?? exception.Message;

        System.Windows.MessageBox.Show(
            message,
            title,
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }
}
