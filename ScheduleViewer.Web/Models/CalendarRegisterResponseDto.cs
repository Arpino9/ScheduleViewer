namespace ScheduleViewer.Web.Models;

/// <summary>Google Calendar予定登録APIの処理結果。</summary>
internal sealed class CalendarRegisterResponseDto
{
    /// <summary>APIの処理状態。</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>画面へ表示する結果メッセージ。</summary>
    public string Message { get; set; } = string.Empty;
}
