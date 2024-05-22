namespace ScheduleViewer.Domain.Entities;

/// <summary>
/// Entity - 手当有無
/// </summary>
public class AllowanceExistenceEntity
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="perfectAttendance">皆勤手当</param>
    /// <param name="education">教育手当</param>
    /// <param name="electricity">在宅手当</param>
    /// <param name="certification">資格手当</param>
    /// <param name="Overtime">時間外手当</param>
    /// <param name="travel">出張手当</param>
    /// <param name="housing">住宅手当</param>
    /// <param name="food">食事手当</param>
    /// <param name="lateNight">深夜手当</param>
    /// <param name="area">地域手当</param>
    /// <param name="commution">通勤手当</param>
    /// <param name="dependency">扶養手当</param>
    /// <param name="executive">役職手当</param>
    /// <param name="special">特別手当</param>
    public AllowanceExistenceEntity(
        bool perfectAttendance,
        bool education,
        bool electricity,
        bool certification,
        bool Overtime,
        bool travel,
        bool housing,
        bool food,
        bool lateNight,
        bool area,
        bool commution,
        bool prepaidRetirement,
        bool dependency,
        bool executive,
        bool special)
    {
        this.PerfectAttendance = new AlternativeValue(perfectAttendance);
        this.Education = new AlternativeValue(education);
        this.Electricity = new AlternativeValue(electricity);
        this.Certification = new AlternativeValue(certification);
        this.Overtime = new AlternativeValue(Overtime);
        this.Travel = new AlternativeValue(travel);
        this.Housing = new AlternativeValue(housing);
        this.Food = new AlternativeValue(food);
        this.LateNight = new AlternativeValue(lateNight);
        this.Area = new AlternativeValue(area);
        this.Commution = new AlternativeValue(commution);
        this.PrepaidRetirement = new AlternativeValue(prepaidRetirement);
        this.Dependency = new AlternativeValue(dependency);
        this.Executive = new AlternativeValue(executive);
        this.Special = new AlternativeValue(special);
    }

    /// <summary> 皆勤手当 </summary>
    public AlternativeValue PerfectAttendance { get; }

    /// <summary> 教育手当 </summary>
    public AlternativeValue Education { get; }

    /// <summary> 在宅手当 </summary>
    public AlternativeValue Electricity { get; }

    /// <summary> 資格手当 </summary>
    public AlternativeValue Certification { get; }

    /// <summary> 時間外手当 </summary>
    public AlternativeValue Overtime { get; }

    /// <summary> 出張手当 </summary>
    public AlternativeValue Travel { get; }

    /// <summary> 住宅手当 </summary>
    public AlternativeValue Housing { get; }

    /// <summary> 食事手当 </summary>
    public AlternativeValue Food { get; }

    /// <summary> 深夜手当 </summary>
    public AlternativeValue LateNight { get; }

    /// <summary> 地域手当 </summary>
    public AlternativeValue Area { get; }

    /// <summary> 通勤手当 </summary>
    public AlternativeValue Commution { get; }

    /// <summary> 前払退職金 </summary>
    public AlternativeValue PrepaidRetirement { get; }

    /// <summary> 扶養手当 </summary>
    public AlternativeValue Dependency { get; }

    /// <summary> 役職手当 </summary>
    public AlternativeValue Executive { get; }

    /// <summary> 特別手当 </summary>
    public AlternativeValue Special { get; }
}