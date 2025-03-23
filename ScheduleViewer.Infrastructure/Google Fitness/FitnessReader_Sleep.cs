namespace ScheduleViewer.Infrastructure.Google_Fitness;

/// <summary>
/// Google Fitness (睡眠記録) - 読み込み
/// </summary>
internal class FitnessReader_Sleep : GoogleServiceBase<FitnessService>
{
    /// <summary> 
    /// 初期化子 
    /// </summary>
    protected override FitnessService Initializer
    {
        get => base.Initialize_OAuth(initializer => new FitnessService(initializer),
                                     new[] { FitnessService.Scope.FitnessSleepRead },
                                     "token_FitnessSleep");
    }

    /// <summary>
    /// データソースを取得する
    /// </summary>
    /// <param name="service">利用するサービス名</param>
    /// <param name="startTime">開始日</param>
    /// <param name="endTime">終了日</param>
    /// <param name="Id">ID</param>
    internal List<(DateTime Date, int Value)> ReadDataSource(FitnessService service, DateTimeOffset startTime, DateTimeOffset endTime, string Id)
    {
        try
        {
            var dataSet = service.Users.Dataset.Aggregate(new AggregateRequest()
            {
                AggregateBy = new List<AggregateBy>()
            {
                new AggregateBy() { DataSourceId = Id }
            },
                BucketByTime = new BucketByTime() { DurationMillis = 86400000 },
                StartTimeMillis = startTime.ToUnixTimeMilliseconds(),
                EndTimeMillis = endTime.ToUnixTimeMilliseconds(),
            }, "me").Execute();

            var values = new List<(DateTime Date, int Value)>();

            foreach (var bucket in dataSet.Bucket)
            {
                foreach (var dataPoint in bucket.Dataset[0].Point)
                {
                    var datetimeStart = dataPoint.StartTimeNanos.Value.ToDateTime();
                    var datetimeEnd = dataPoint.EndTimeNanos.Value.ToDateTime();

                    foreach (var value in dataPoint.Value)
                    {
                        if (value.IntVal.HasValue)
                        {
                            values.Add((datetimeEnd, value.IntVal.Value));
                        }
                    }
                }
            }

            return values;
        }
        catch (Google.GoogleApiException ex)
        {
            Console.WriteLine($"Google API error: {ex.Error.Message}");

            return new List<(DateTime Date, int Value)>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General error: {ex.Message}");

            return new List<(DateTime Date, int Value)>();
        }
    }

    /// <summary> 睡眠時間 </summary>
    internal List<(DateTime Date, int Value)> SleepingTime { get; set; }

    /// <summary>
    /// 指定された期間の睡眠時間を取得する
    /// </summary>
    /// <param name="startTime">開始日</param>
    /// <param name="endTime">終了日</param>
    internal async Task ReadSleepTime(DateTimeOffset startTime, DateTimeOffset endTime)
    {
        await Task.Run(() =>
        {
            SleepingTime = ReadDataSource(Initializer, startTime, endTime, "derived:com.google.sleep.segment:com.google.android.gms:merged");
        }).ConfigureAwait(false);
    }

    /// <summary>
    /// 日付で検索
    /// </summary>
    /// <param name="date">日付</param>
    /// <returns>睡眠時間</returns>
    internal List<int> FindSleepTimeByDate(DateOnly date)
        => SleepingTime.Any() ?
           SleepingTime.Where(x => x.Date.Year  == date.Year &&
                                   x.Date.Month == date.Month &&
                                   x.Date.Day   == date.Day)
                       .Select(x => x.Value).ToList() :
           new List<int>();
}