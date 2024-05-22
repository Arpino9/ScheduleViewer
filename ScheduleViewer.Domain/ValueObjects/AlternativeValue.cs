namespace ScheduleViewer.Domain.ValueObjects;

/// <summary>
/// Value Object - 二者択一
/// </summary>
public sealed record class AlternativeValue
{
    /// <summary> ○ </summary>
    public static readonly AlternativeValue Valid = new AlternativeValue(true);

    /// <summary> × </summary>
    public static readonly AlternativeValue Invalid = new AlternativeValue(false);

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="value">値</param>
    public AlternativeValue(bool value)
    {
        this.Value = value;
    }

    /// <summary> Value </summary>
    public bool Value { get; }

    /// <summary> Text </summary>
    public string Text => (this == AlternativeValue.Valid ? "○" : "×");
}