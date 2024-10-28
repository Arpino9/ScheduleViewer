namespace ScheduleViewer.Infrastructure.Fitbit;

/// <summary>
/// エンドポイント
/// </summary>
/// <remarks>
/// {0}の部分に日付が入る。
/// </remarks>
internal struct Endpoint
{
    /// <summary> プロフィール </summary>
    internal static readonly string Profile = "https://api.fitbit.com/1/user/-/profile.json";

    /// <summary> 睡眠データ </summary>
    internal static readonly string Sleep = "https://api.fitbit.com/1.2/user/-/sleep/date/{0}.json";

    /// <summary> アクティビティ </summary>
    internal static readonly string Activity = "https://api.fitbit.com/1/user/-/activities/date/{0}.json";

    /// <summary> 心拍数 </summary>
    internal static readonly string Heart = "https://api.fitbit.com/1/user/-/activities/heart/date/{0}/1d.json";

    /// <summary> 体重 </summary>
    internal static readonly string Weight = "https://api.fitbit.com/1/user/-/body/log/weight/date/{0}.json";

    /// <summary> 体脂肪率 </summary>
    internal static readonly string Fat = "https://api.fitbit.com/1/user/-/body/log/fat/date/{0}.json";

    /// <summary> 水分摂取 </summary>
    internal static readonly string Water = "https://api.fitbit.com/1/user/-/foods/log/water/date/{0}.json";

    /// <summary> 食事 </summary>
    internal static readonly string Foods = "https://api.fitbit.com/1/user/-/foods/log/date/{0}.json";

    /// <summary> 呼吸 </summary>
    internal static readonly string Breathing = "https://api.fitbit.com/1/user/-/breathing/log/date/{0}.json";

    /// <summary> デバイス情報 </summary>
    internal static readonly string Devices = "https://api.fitbit.com/1/user/-/devices.json";
}
