using System.Windows.Threading;

namespace ScheduleViewer.WPF;

/// <summary>
/// アプリケーションのエントリーポイントです。
/// </summary>
public partial class App : System.Windows.Application
{
    /// <inheritdoc />
    protected override void OnStartup(StartupEventArgs e)
    {
        DispatcherUnhandledException += OnDispatcherUnhandledException;
        base.OnStartup(e);
    }

    private static void OnDispatcherUnhandledException(
        object sender,
        DispatcherUnhandledExceptionEventArgs e)
    {
        ExceptionDialog.Show(e.Exception);
    }
}
