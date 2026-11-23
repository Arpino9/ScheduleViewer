namespace ScheduleViewer.Web.Models;

/// <summary>外部サービスの認証開始APIが返す情報。</summary>
public sealed class AuthorizationResponseDto
{
    /// <summary>認証状態。認証待ちまたは認証済みを表す。</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>ユーザーが開くOAuth認証ページのURL。</summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>APIが返した説明メッセージ。</summary>
    public string Message { get; set; } = string.Empty;
}
