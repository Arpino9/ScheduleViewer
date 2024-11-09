namespace ScheduleViewer.Infrastructure.Fitbit;

/// <summary>
/// Fitbit Reader
/// </summary>
internal class FitbitReader : FitbitBase
{
    /// <summary>
    /// プロフィールを取得する
    /// </summary>
    /// <returns>プロフィール</returns>
    /// <remarks>
    /// ユーザーの基本情報（年齢、身長、体重、性別など）を取得する。
    /// </remarks>
    internal async Task<Fitbit_ProfileEntity> GetProfileAsync()
    {
        var jsonData = await base.FetchAsync(Endpoint.Profile);

        var badges = new List<EarnedBadge>();

        if (string.IsNullOrEmpty(jsonData))
        {
            // 未登録
            return new Fitbit_ProfileEntity(
                string.Empty, 0, string.Empty, 0, 0, badges);
        }

        JObject jsonObject = JObject.Parse(jsonData);

        var name   = (string)jsonObject["user"]["fullName"];
        var age    = (int)jsonObject["user"]["age"];
        var gender = (string)jsonObject["user"]["gender"];
        var height = (double)jsonObject["user"]["height"];
        var weight = (double)jsonObject["user"]["weight"];
        
        var topBadges = jsonObject["user"]["topBadges"];

        foreach(var badge in topBadges)
        {
            badges.Add(new EarnedBadge((string)badge["category"], 
                                       (DateTime)badge["dateTime"],
                                       (string)badge["description"]));
        }

        return new Fitbit_ProfileEntity(name, age, gender, height, weight, badges);
    }

    /// <summary>
    /// 睡眠データを取得する
    /// </summary>
    /// <param name="date">取得日付</param>
    /// <returns>睡眠データ</returns>
    /// <remarks>
    /// 指定した日の睡眠データ（睡眠時間、睡眠サイクル、覚醒時間など）を取得する。
    /// </remarks>
    internal async Task<Fitbit_SleepEntity> GetSleepAsync(DateTime date)
    {
        var jsonData = await base.FetchAsync(string.Format(Endpoint.Sleep, DateUtils.ToSQLiteDate(date)));

        if (string.IsNullOrEmpty(jsonData))
        {
            // 未登録
            return new Fitbit_SleepEntity(
                new DateTime(), new DateTime(), 
                new TimeSpan(), new TimeSpan(), new TimeSpan(), new TimeSpan());
        }

        JObject jsonObject = JObject.Parse(jsonData);

        var sleepArray = jsonObject["sleep"] as JArray;
        var sleep = sleepArray.LastOrDefault();

        if (sleep is null)
        {
            return new Fitbit_SleepEntity(new DateTime(), new DateTime(),
                                          new TimeSpan(), new TimeSpan(), new TimeSpan(), new TimeSpan());
        }

        var startTime = (DateTime)sleep["startTime"];
        var endTime   = (DateTime)sleep["endTime"];

        var summary = sleep["levels"]["summary"];

        if (summary is null)
        {
            return new Fitbit_SleepEntity(startTime, endTime, 
                                          new TimeSpan(), new TimeSpan(), new TimeSpan(), new TimeSpan());
        }

        var asleep   = summary["asleep"] is null ?
                       new TimeSpan() : 
                       TimeSpan.FromMinutes((double)summary["asleep"]["minutes"]);

        if (asleep == default(TimeSpan))
        {
            asleep = TimeSpan.FromMinutes((double)summary["deep"]["minutes"]);
        }

        var rem = summary["rem"] is null ?
                  new TimeSpan() :
                  TimeSpan.FromMinutes((double)summary["rem"]["minutes"]);

        var awake    = summary["awake"] is null ?
                       new TimeSpan() :
                       TimeSpan.FromMinutes((double)summary["awake"]["minutes"]);

        if (awake == default(TimeSpan))
        {
            awake = TimeSpan.FromMinutes((double)summary["wake"]["minutes"]);
        }

        var restless = summary["restless"] is null ?
                       new TimeSpan() : 
                       TimeSpan.FromMinutes((double)summary["restless"]["minutes"]);

        if (restless == default(TimeSpan))
        {
            restless = TimeSpan.FromMinutes((double)summary["light"]["minutes"]);
        }

        return new Fitbit_SleepEntity(startTime, endTime, awake, restless, rem, asleep);
    }

    /// <summary>
    /// アクティビティを取得する
    /// </summary>
    /// <param name="date">取得日付</param>
    /// <returns>アクティビティ</returns>
    /// <remarks>
    /// 日ごとの活動サマリー（ステップ数、消費カロリー、移動距離など）を取得する。
    /// </remarks>
    internal async Task<Fitbit_ActivityEntity> GetActivityAsync(DateTime date)
    {
        var jsonData = await base.FetchAsync(string.Format(Endpoint.Activity, DateUtils.ToSQLiteDate(date)));

        if (string.IsNullOrEmpty(jsonData))
        {
            // 未登録
            return new Fitbit_ActivityEntity(0, 0, 0, 0);
        }

        JObject jsonObject = JObject.Parse(jsonData);

        var steps       = (double)jsonObject["summary"]["steps"];
        var caloriesOut = (double)jsonObject["summary"]["caloriesOut"];
        var elevation   = (double)jsonObject["summary"]["elevation"];
        var distance    = (double)jsonObject["summary"]["distances"][0]["distance"];

        return new Fitbit_ActivityEntity(steps, caloriesOut, elevation, distance);
    }

    /// <summary>
    /// 心拍数を取得する
    /// </summary>
    /// <param name="date">取得日付</param>
    /// <returns>心拍数</returns>
    /// <remarks>
    /// 日ごとの心拍数データを取得する。
    /// </remarks>
    internal async Task<Fitbit_HeartEntity> GetHeartAsync(DateTime date)
    {
        var jsonData = await base.FetchAsync(string.Format(Endpoint.Heart, DateUtils.ToSQLiteDate(date)));

        if (string.IsNullOrEmpty(jsonData))
        {
            // 未登録
            return new Fitbit_HeartEntity(0);
        }

        JObject jsonObject = JObject.Parse(jsonData);

        var value = jsonObject["activities-heart"][0]["value"]["restingHeartRate"];

        if (value is null)
        {
            // 未登録
            return new Fitbit_HeartEntity(0);
        }

        return new Fitbit_HeartEntity(double.Parse(value.ToString()));
    }

    /// <summary>
    /// 体重を取得する
    /// </summary>
    /// <param name="date">取得日付</param>
    /// <returns>体重</returns>
    /// <remarks>
    /// 日ごとの体重記録を取得する。体重変化のトラッキングなどに使える。
    /// </remarks>
    internal async Task<Fitbit_WeightEntity> GetWeightAsync(DateTime date)
    {
        var jsonData = await base.FetchAsync(string.Format(Endpoint.Weight, DateUtils.ToSQLiteDate(date)));

        if (string.IsNullOrEmpty(jsonData))
        {
            // 未登録
            return new Fitbit_WeightEntity(0, 0);
        }

        JObject jsonObject = JObject.Parse(jsonData);

        var bmi    = (double)jsonObject["weight"][0]["bmi"];
        var weight = (double)jsonObject["weight"][0]["weight"];

        return new Fitbit_WeightEntity(bmi, weight);
    }
}
