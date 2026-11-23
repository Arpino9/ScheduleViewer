namespace ScheduleViewer.Web.Models;

/// <summary>アニメ視聴登録APIの処理結果。</summary>
internal sealed class AnimeRegisterResponseDto
{
    /// <summary>APIの処理状態。</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>画面へ表示する結果メッセージ。</summary>
    public string Message { get; set; } = string.Empty;
}
