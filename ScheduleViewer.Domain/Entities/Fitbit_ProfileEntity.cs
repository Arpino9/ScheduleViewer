namespace ScheduleViewer.Domain.Entities;

/// <summary>
/// Entity - Fitbit (プロフィール)
/// </summary>
public sealed class Fitbit_ProfileEntity
{
    public Fitbit_ProfileEntity(
        string name, 
        int age,
        string gender,
        double height,
        double weight,
        List<EarnedBadge> badges)
    {
        this.Name   = name;
        this.Age    = age;
        this.Gender = gender;
        this.Height = height;
        this.Weight = weight;
        this.Badges = badges;
    }

    /// <summary> 名前 </summary>
    public string Name { get; }

    /// <summary> 年齢 </summary>
    public int Age { get; }

    /// <summary> 性別 </summary>
    public string Gender { get; }

    /// <summary> 身長 </summary>
    public double Height { get; }

    /// <summary> 体重 </summary>
    public double Weight { get; }

    /// <summary> 獲得したバッジ </summary>
    public List<EarnedBadge> Badges { get; }
}

/// <summary>
/// Entity - Fitbit (獲得バッジ)
/// </summary>
public sealed class EarnedBadge
{
    public EarnedBadge(
        string name,
        DateTime earnedDate,
        string description)
    {
        this.Name = name;
        this.EarnedDate = earnedDate;
        this.Description = description;
    }

    /// <summary> バッジ名 </summary>
    public string Name { get; }

    /// <summary> 取得した日付 </summary>
    public DateTime EarnedDate { get; }

    /// <summary> 説明 </summary>
    public string Description { get; }
}
