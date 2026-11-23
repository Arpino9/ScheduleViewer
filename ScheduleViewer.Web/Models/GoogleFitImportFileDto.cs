namespace ScheduleViewer.Web.Models;

/// <summary>Google Fit Takeoutファイル1件の処理結果。</summary>
public sealed class GoogleFitImportFileDto
{
    /// <summary>処理対象のファイル名。</summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>検出した対応データ件数。</summary>
    public int Detected { get; set; }

    /// <summary>登録できた件数。</summary>
    public int Imported { get; set; }

    /// <summary>既に登録済みだった件数。</summary>
    public int Duplicates { get; set; }

    /// <summary>登録できなかったデータのエラー。</summary>
    public List<string> Errors { get; set; } = [];
}
