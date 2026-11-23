namespace ScheduleViewer.Web.Models;

/// <summary>Google Fit Takeoutインポートの処理結果。</summary>
public sealed class GoogleFitImportResponseDto
{
    /// <summary>ファイルごとの処理結果。</summary>
    public List<GoogleFitImportFileDto> Files { get; set; } = [];
}
