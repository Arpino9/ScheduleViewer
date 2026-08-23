namespace ScheduleViewer.Web.Models;

/// <summary>Google Healthインポート用の認証状態。</summary>
public sealed class GoogleHealthImportStatusDto
{
    /// <summary>書き込み認証が完了しているかどうか。</summary>
    public bool Authorized { get; set; }
}
