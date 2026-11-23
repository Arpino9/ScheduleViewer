namespace ScheduleViewer.Web.Models;

/// <summary>APIが返すエラーメッセージ。</summary>
public sealed class ApiMessageDto
{
    /// <summary>画面へ表示するエラー内容。</summary>
    public string Message { get; set; } = string.Empty;
}
