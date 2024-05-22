namespace ScheduleViewer.Domain.Entities;

/// <summary>
/// Entity - 職歴
/// </summary>
public sealed class CareerEntity
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="id">ID</param>
    /// <param name="workingStatus">雇用形態</param>
    /// <param name="companyName">会社名</param>
    /// <param name="employeeNumber">社員番号</param>
    /// <param name="workingStartDate">勤務開始日</param>
    /// <param name="workingEndDate">勤務終了日</param>
    /// <param name="allowanceExistence">手当</param>
    /// <param name="remarks">備考</param>
    public CareerEntity(
        int id,
        string workingStatus,
        string companyName,
        string employeeNumber,
        DateTime workingStartDate,
        DateTime workingEndDate,
        AllowanceExistenceEntity allowanceExistence,
        string remarks)
    {
        this.ID = id;
        this.WorkingStatus = workingStatus;
        this.CompanyName = new CompanyNameValue(companyName);
        this.EmployeeNumber = employeeNumber;
        this.WorkingStartDate = new WorkingDateValue(workingStartDate);
        this.WorkingEndDate = new WorkingDateValue(workingEndDate);
        this.AllowanceExistence = allowanceExistence;
        this.Remarks = remarks;
    }

    /// <summary> ID </summary>
    public int ID { get; }

    /// <summary> 雇用形態 </summary>
    public string WorkingStatus { get; }

    /// <summary> 会社名 </summary>
    public CompanyNameValue CompanyName { get; }

    /// <summary> 社員番号 </summary>
    public string EmployeeNumber { get; }

    /// <summary> 勤務開始日 </summary>
    public WorkingDateValue WorkingStartDate { get; }

    /// <summary> 勤務終了日 </summary>
    public WorkingDateValue WorkingEndDate { get; }

    /// <summary> 手当 </summary>
    public AllowanceExistenceEntity AllowanceExistence { get; }

    /// <summary> 備考 </summary>
    public string Remarks { get; }
}