package com.scheduleviewer.domain.entity;

import com.scheduleviewer.domain.valueobject.CompanyNameValue;
import com.scheduleviewer.domain.valueobject.WorkingDateValue;

import java.time.LocalDate;

/**
 * Entity - 職歴
 */
public final class CareerEntity {

    private final int id;
    private final String workingStatus;
    private final CompanyNameValue companyName;
    private final String employeeNumber;
    private final WorkingDateValue workingStartDate;
    private final WorkingDateValue workingEndDate;
    private final AllowanceExistenceEntity allowanceExistence;
    private final String remarks;

    public CareerEntity(
            int id,
            String workingStatus,
            String companyName,
            String employeeNumber,
            LocalDate workingStartDate,
            LocalDate workingEndDate,
            AllowanceExistenceEntity allowanceExistence,
            String remarks) {
        this.id                  = id;
        this.workingStatus       = workingStatus;
        this.companyName         = new CompanyNameValue(companyName);
        this.employeeNumber      = employeeNumber;
        this.workingStartDate    = new WorkingDateValue(workingStartDate);
        this.workingEndDate      = new WorkingDateValue(workingEndDate);
        this.allowanceExistence  = allowanceExistence;
        this.remarks             = remarks;
    }

    /** ID */
    public int getId() { return id; }

    /** 雇用形態 */
    public String getWorkingStatus() { return workingStatus; }

    /** 会社名 */
    public CompanyNameValue getCompanyName() { return companyName; }

    /** 社員番号 */
    public String getEmployeeNumber() { return employeeNumber; }

    /** 勤務開始日 */
    public WorkingDateValue getWorkingStartDate() { return workingStartDate; }

    /** 勤務終了日 */
    public WorkingDateValue getWorkingEndDate() { return workingEndDate; }

    /** 手当 */
    public AllowanceExistenceEntity getAllowanceExistence() { return allowanceExistence; }

    /** 備考 */
    public String getRemarks() { return remarks; }
}
