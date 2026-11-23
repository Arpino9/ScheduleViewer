namespace ScheduleViewer.Web.Models;

public sealed record ExpenditureRecord(
    string ItemName,
    long Price,
    string FinancialInstitutions,
    string CategoryLarge,
    string CategoryMiddle,
    string Memo,
    bool CanCalculate);
