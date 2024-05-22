namespace ScheduleViewer.Domain.ValueObjects;

/// <summary>
/// Value Object - 有給日数
/// </summary>
public sealed record class PaidVacationDaysValue
{
    /// <summary> 上限値 </summary>
    public static readonly int Maximum = 40;

    /// <summary> 下限値 </summary>
    public static readonly int Minimum = 0;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="paidVacationDays">有給日数</param>
    /// <exception cref="ArgumentOutOfRangeException">上限値を超えた場合</exception>
    public PaidVacationDaysValue(double paidVacationDays)
    {
        if (paidVacationDays < PaidVacationDaysValue.Minimum)
        {
            throw new ArgumentOutOfRangeException("有給日数の下限値を下回っています。");
        }

        if (paidVacationDays > PaidVacationDaysValue.Maximum)
        {
            throw new ArgumentOutOfRangeException("有給日数の上限値を超えています。");
        }

        this.Value = paidVacationDays;
    }

    /// <summary> 有給日数 </summary>
    public readonly double Value;

    /// <summary>
    /// Text
    /// </summary>
    public string Text => ($"{this.Value.ToString()}日");

    /// <summary>
    /// To String
    /// </summary>
    /// <returns>金額</returns>
    public override string ToString() => ($"有給日数：{this.Value}日");
}