namespace ScheduleViewer.Domain.ValueObjects;

/// <summary>
/// Value Object - 会社
/// </summary>
public sealed record class CompanyNameValue
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="value">値</param>
    public CompanyNameValue(string value)
    {
        this.Text = value;
    }

    /// <summary> Text </summary>
    public string Text { get; }

    /// <summary> 未登録 </summary>
    public static CompanyNameValue Undefined = new CompanyNameValue(string.Empty);

    /// <summary> 株式会社か </summary>
    public bool IsInc => (this.Text.Contains("株式会社") ||
                          this.Text.Contains("(株)") ||
                          this.Text.Contains("（株）"));

    /// <summary> 有限会社か </summary>
    public bool IsLimited => (this.Text.Contains("有限会社") ||
                              this.Text.Contains("(有)") ||
                              this.Text.Contains("（有）"));

    /// <summary> 表示用 </summary>
    public string DisplayValue => (this == CompanyNameValue.Undefined ? "<未登録>" : this.Text);
}
