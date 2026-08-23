namespace ScheduleViewer.Web.Models;

public sealed class ExpenditureDto
{
    public string Id { get; set; } = string.Empty;
    public string CanCalc { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public long Price { get; set; }
    public string FinancialInstitutions { get; set; } = string.Empty;
    public string CategoryLarge { get; set; } = string.Empty;
    public string CategoryMiddle { get; set; } = string.Empty;
    public string Memo { get; set; } = string.Empty;
    public string Change { get; set; } = string.Empty;
}
