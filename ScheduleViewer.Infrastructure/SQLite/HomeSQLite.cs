namespace ScheduleViewer.Infrastructure.SQLite;

/// <summary>
/// SQLite - 会社
/// </summary>
public class HomeSQLite : IHomeRepository
{
    public IReadOnlyList<HomeEntity> GetEntities()
    {
        string sql = @"
SELECT ID, 
DisplayName, 
LivingStart,
LivingEnd,
IsLiving,
PostCode, 
Address, 
Address_Google, 
Remarks
FROM Home";

        return SQLiteHelper.Query(
            sql,
            reader =>
            {
                return new HomeEntity(
                            Convert.ToInt32(reader["ID"]),
                            Convert.ToString(reader["DisplayName"]),
                            DateOnly.FromDateTime(Convert.ToDateTime(reader["LivingStart"])),
                            DateOnly.FromDateTime(Convert.ToDateTime(reader["LivingEnd"])),
                            Convert.ToBoolean(reader["IsLiving"]),
                            Convert.ToString(reader["PostCode"]),
                            Convert.ToString(reader["Address"]),
                            Convert.ToString(reader["Address_Google"]),
                            Convert.ToString(reader["Remarks"]));
            });
    }

    public HomeEntity GetEntity(int id)
    {
        string sql = @"
SELECT ID, 
DisplayName, 
LivingStart,
LivingEnd,
IsLiving,
PostCode, 
Address, 
Address_Google, 
Remarks
FROM Home
Where ID = @ID"
        ;

        var args = new List<SQLiteParameter>()
        {
            new SQLiteParameter("ID", id),
        };

        return SQLiteHelper.QuerySingle<HomeEntity>(
            sql,
            args.ToArray(),
            reader =>
            {
                return new HomeEntity(
                            Convert.ToInt32(reader["ID"]),
                            Convert.ToString(reader["DisplayName"]),
                            DateOnly.FromDateTime(Convert.ToDateTime(reader["LivingStart"])),
                            DateOnly.FromDateTime(Convert.ToDateTime(reader["LivingEnd"])),
                            Convert.ToBoolean(reader["IsLiving"]),
                            Convert.ToString(reader["PostCode"]),
                            Convert.ToString(reader["Address"]),
                            Convert.ToString(reader["Address_Google"]),
                            Convert.ToString(reader["Remarks"]));
            },
            null);
    }

    public void Save(HomeEntity entity)
    {
        string insert = @"
insert into Home
(ID,
DisplayName, 
LivingStart,
LivingEnd,
IsLiving,
PostCode, 
Address, 
Address_Google, 
Remarks)
values
(@ID, 
@DisplayName, 
@LivingStart, 
@LivingEnd, 
@IsLiving, 
@PostCode, 
@Address, 
@Address_Google, 
@Remarks)
";

        string update = @"
update Home
set ID             = @ID, 
    DisplayName    = @DisplayName, 
    LivingStart    = @LivingStart, 
    LivingEnd      = @LivingEnd, 
    IsLiving       = @IsLiving, 
    PostCode       = @PostCode, 
    Address        = @Address, 
    Address_Google = @Address_Google, 
    Remarks        = @Remarks
where ID = @ID
"
        ;

        var args = new List<SQLiteParameter>()
        {
            new SQLiteParameter("ID",             entity.ID),
            new SQLiteParameter("DisplayName",    entity.DisplayName),
            new SQLiteParameter("LivingStart",    entity.LivingStart.ToSQLiteDate()),
            new SQLiteParameter("LivingEnd",      entity.LivingEnd.ToSQLiteDate()),
            new SQLiteParameter("IsLiving",       entity.IsLiving),
            new SQLiteParameter("PostCode",       entity.PostCode),
            new SQLiteParameter("Address",        entity.Address),
            new SQLiteParameter("Address_Google", entity.Address_Google),
            new SQLiteParameter("Remarks",        entity.Remarks),
        };

        SQLiteHelper.Execute(insert, update, args.ToArray());
    }

    public void Delete(int id)
    {
        string delete = @"
Delete From Home
where ID = @ID
"
        ;

        var args = new List<SQLiteParameter>()
        {
            new SQLiteParameter("ID", id),
        };

        SQLiteHelper.Execute(delete, args.ToArray());
    }
}
