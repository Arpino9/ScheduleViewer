namespace ScheduleViewer.Domain.Entities;

/// <summary>
/// Entity - 自宅
/// </summary>
public sealed class HomeEntity
{
    public HomeEntity(
        int id,
        string displayName,
        DateTime livingStart,
        DateTime livingEnd,
        bool isLiving,
        string postCode,
        string address,
        string address_google,
        string remarks)
    {
        this.ID = id;
        this.DisplayName = displayName;
        this.LivingStart = livingStart;
        this.LivingEnd = livingEnd;
        this.IsLiving = isLiving;
        this.PostCode = postCode;
        this.Address = address;
        this.Address_Google = address_google;
        this.Remarks = remarks;
    }

    /// <summary> ID </summary>
    public int ID { get; }

    /// <summary> 名称 </summary>
    public string DisplayName { get; }

    /// <summary> 郵便番号 </summary>
    public string PostCode { get; }

    /// <summary> 在住開始日 </summary>
    public DateTime LivingStart { get; }

    private DateTime _livingEnd;

    /// <summary> 在住終了日 </summary>
    public DateTime LivingEnd
    {
        get => this.IsLiving ? DateTime.Today : _livingEnd;
        set => _livingEnd = value;
    }

    /// <summary> 在住中か </summary>
    public bool IsLiving { get; }

    /// <summary> 住所 </summary>
    public string Address { get; }

    /// <summary> 住所 </summary>
    public string Address_Google { get; }

    /// <summary> 備考 </summary>
    public string Remarks { get; }
}