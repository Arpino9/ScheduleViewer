namespace ScheduleViewer.Infrastructure.SQLite;

/// <summary>
/// SQLite - 職歴
/// </summary>
public class CareerSQLite : ICareerRepository
{
    public IReadOnlyList<CareerEntity> GetEntities()
    {
        string sql = @"
SELECT ID, 
CompanyName, 
EmployeeNumber, 
WorkingStatus, 
WorkingStartDate, 
WorkingEndDate, 
PerfectAttendance, 
Education, 
Electricity, 
Certification, 
Overtime, 
Travel, 
Housing, 
Food, 
LateNight, 
Area, 
Commution, 
PrepaidRetirement, 
Dependency, 
Executive, 
Special, 
Remarks
FROM Career";

        return SQLiteHelper.Query(
            sql,
            reader =>
            {
                var allowance = new AllowanceExistenceEntity(
                    Convert.ToBoolean(reader["PerfectAttendance"]),
                    Convert.ToBoolean(reader["Education"]),
                    Convert.ToBoolean(reader["Electricity"]),
                    Convert.ToBoolean(reader["Certification"]),
                    Convert.ToBoolean(reader["Overtime"]),
                    Convert.ToBoolean(reader["Travel"]),
                    Convert.ToBoolean(reader["Housing"]),
                    Convert.ToBoolean(reader["Food"]),
                    Convert.ToBoolean(reader["LateNight"]),
                    Convert.ToBoolean(reader["Area"]),
                    Convert.ToBoolean(reader["Commution"]),
                    Convert.ToBoolean(reader["PrepaidRetirement"]),
                    Convert.ToBoolean(reader["Dependency"]),
                    Convert.ToBoolean(reader["Executive"]),
                    Convert.ToBoolean(reader["Special"]));

                return new CareerEntity(
                            Convert.ToInt32(reader["ID"]),
                            Convert.ToString(reader["WorkingStatus"]),
                            Convert.ToString(reader["CompanyName"]),
                            Convert.ToString(reader["EmployeeNumber"]),
                            DateOnly.FromDateTime(Convert.ToDateTime(reader["WorkingStartDate"])),
                            DateOnly.FromDateTime(Convert.ToDateTime(reader["WorkingEndDate"])),
                            allowance,
                            Convert.ToString(reader["Remarks"]));
            });
    }

    /// <summary>
    /// Get - 支給額
    /// </summary>
    /// <param name="id">ID</param>
    /// <returns>支給額</returns>
    public CareerEntity GetEntity(int id)
    {
        string sql = @"
SELECT ID, 
CompanyName, 
EmployeeNumber, 
WorkingStatus, 
WorkingStartDate, 
WorkingEndDate, 
PerfectAttendance, 
Education, 
Electricity, 
Certification, 
Overtime, 
Travel, 
Housing, 
Food, 
LateNight, 
Area, 
Commution, 
PrepaidRetirement, 
Dependency, 
Executive, 
Special, 
Remarks
FROM Career
Where ID = @ID";

        var args = new List<SQLiteParameter>()
        {
            new SQLiteParameter("ID", id),
        };

        return SQLiteHelper.QuerySingle<CareerEntity>(
            sql,
            args.ToArray(),
            reader =>
            {
                var allowance = new AllowanceExistenceEntity(
                    Convert.ToBoolean(reader["PerfectAttendance"]),
                    Convert.ToBoolean(reader["Education"]),
                    Convert.ToBoolean(reader["Electricity"]),
                    Convert.ToBoolean(reader["Certification"]),
                    Convert.ToBoolean(reader["Overtime"]),
                    Convert.ToBoolean(reader["Travel"]),
                    Convert.ToBoolean(reader["Housing"]),
                    Convert.ToBoolean(reader["Food"]),
                    Convert.ToBoolean(reader["LateNight"]),
                    Convert.ToBoolean(reader["Area"]),
                    Convert.ToBoolean(reader["Commution"]),
                    Convert.ToBoolean(reader["PrepaidRetirement"]),
                    Convert.ToBoolean(reader["Dependency"]),
                    Convert.ToBoolean(reader["Executive"]),
                    Convert.ToBoolean(reader["Special"]));

                return new CareerEntity(
                            Convert.ToInt32(reader["ID"]),
                            Convert.ToString(reader["WorkingStatus"]),
                            Convert.ToString(reader["CompanyName"]),
                            Convert.ToString(reader["EmployeeNumber"]),
                            DateOnly.FromDateTime(Convert.ToDateTime(reader["WorkingStartDate"])),
                            DateOnly.FromDateTime(Convert.ToDateTime(reader["WorkingEndDate"])),
                            allowance,
                            Convert.ToString(reader["Remarks"]));
            },
            null);
    }

    public void Save(
        CareerEntity entity)
    {
        string insert = @"
insert into Career
(ID,
CompanyName, 
EmployeeNumber, 
WorkingStatus, 
WorkingStartDate, 
WorkingEndDate, 
PerfectAttendance, 
Education, 
Electricity, 
Certification, 
Overtime, 
Travel, 
Housing, 
Food, 
LateNight, 
Area, 
Commution, 
PrepaidRetirement, 
Dependency, 
Executive, 
Special, 
Remarks)
values
(@ID, 
@CompanyName, 
@EmployeeNumber, 
@WorkingStatus, 
@WorkingStartDate, 
@WorkingEndDate, 
@PerfectAttendance, 
@Education, 
@Electricity, 
@Certification, 
@Overtime, 
@Travel, 
@Housing, 
@Food, 
@LateNight, 
@Area, 
@Commution, 
@PrepaidRetirement, 
@Dependency, 
@Executive, 
@Special, 
@Remarks)
";

        string update = @"
update Career
set ID                = @ID, 
    CompanyName       = @CompanyName, 
    EmployeeNumber    = @EmployeeNumber, 
    WorkingStatus     = @WorkingStatus, 
    WorkingStartDate  = @WorkingStartDate, 
    WorkingEndDate    = @WorkingEndDate, 
    PerfectAttendance = @PerfectAttendance,  
    Education         = @Education,  
    Electricity       = @Electricity,  
    Certification     = @Certification,  
    Overtime          = @Overtime,  
    Travel            = @Travel,  
    Housing           = @Housing,  
    Food              = @Food,  
    LateNight         = @LateNight,  
    Area              = @Area,  
    Commution         = @Commution,  
    PrepaidRetirement = @PrepaidRetirement,  
    Dependency        = @Dependency,  
    Executive         = @Executive,  
    Special           = @Special,  
    Remarks           = @Remarks
where ID = @ID
";

        var args = new List<SQLiteParameter>()
        {
            new SQLiteParameter("ID",                entity.ID),
            new SQLiteParameter("CompanyName",       entity.CompanyName.Text),
            new SQLiteParameter("EmployeeNumber",    entity.EmployeeNumber),
            new SQLiteParameter("WorkingStatus",     entity.WorkingStatus),
            new SQLiteParameter("WorkingStartDate",  entity.WorkingStartDate.Value.ToSQLiteDate()),
            new SQLiteParameter("WorkingEndDate",    entity.WorkingEndDate.Value.ToSQLiteDate()),
            new SQLiteParameter("PerfectAttendance", entity.AllowanceExistence.PerfectAttendance.Value),
            new SQLiteParameter("Education",         entity.AllowanceExistence.Education.Value),
            new SQLiteParameter("Electricity",       entity.AllowanceExistence.Electricity.Value),
            new SQLiteParameter("Certification",     entity.AllowanceExistence.Certification.Value),
            new SQLiteParameter("Overtime",          entity.AllowanceExistence.Overtime.Value),
            new SQLiteParameter("Travel",            entity.AllowanceExistence.Travel.Value),
            new SQLiteParameter("Housing",           entity.AllowanceExistence.Housing.Value),
            new SQLiteParameter("Food",              entity.AllowanceExistence.Food.Value),
            new SQLiteParameter("LateNight",         entity.AllowanceExistence.LateNight.Value),
            new SQLiteParameter("Area",              entity.AllowanceExistence.Area.Value),
            new SQLiteParameter("Commution",         entity.AllowanceExistence.Commution.Value),
            new SQLiteParameter("PrepaidRetirement", entity.AllowanceExistence.PrepaidRetirement.Value),
            new SQLiteParameter("Dependency",        entity.AllowanceExistence.Dependency.Value),
            new SQLiteParameter("Executive",         entity.AllowanceExistence.Executive.Value),
            new SQLiteParameter("Special",           entity.AllowanceExistence.Special.Value),
            new SQLiteParameter("Remarks",           entity.Remarks),
        };

        SQLiteHelper.Execute(insert, update, args.ToArray());
    }

    public void Delete(
        int id)
    {
        string delete = @"
Delete From Career
where ID = @ID
";

        var args = new List<SQLiteParameter>()
        {
            new SQLiteParameter("ID", id),
        };

        SQLiteHelper.Execute(delete, args.ToArray());
    }
}