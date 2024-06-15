namespace ScheduleViewer.Infrastructure.Google_Fitness;

/// <summary>
/// Google Fitness - 読み込み
/// </summary>
public sealed class FitnessReader
{
    /// <summary> 初期化 </summary>
    public static FitnessService Initializer_Activity => GoogleService<FitnessService>.Initialize_OAuth(
                                                         initializer => new FitnessService(initializer),
                                                         new[] { FitnessService.Scope.FitnessActivityRead },
                                                         "token_FitnessActivity");

    /// <summary> 初期化 </summary>
    public static FitnessService Initializer_Sleep => GoogleService<FitnessService>.Initialize_OAuth(
                                                      initializer => new FitnessService(initializer),
                                                      new[] { FitnessService.Scope.FitnessSleepRead },
                                                      "token_FitnessSleep");


    public static async Task ReadSession()
    {
        try
        {
            var response = Initializer_Activity.Users.Sessions.List("me").Execute();

            foreach (var session in response.Session)
            {
                Console.WriteLine($"Activity: {session.ActivityType}, Start Time: {session.StartTimeMillis}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    /// <summary>
    /// データソースを取得する
    /// </summary>
    public static void ReadData()
    {
        try
        {
            var dataSources = Initializer_Activity.Users.DataSources.List("me").Execute();

            foreach (var dataSource in dataSources.DataSource)
            {
                Console.WriteLine($"DataSource ID: {dataSource.DataStreamId}");
                Console.WriteLine($"  Data Type: {dataSource.DataType.Name}");
            }
        }
        catch (Google.GoogleApiException ex)
        {
            Console.WriteLine($"Google API error: {ex.Error.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General error: {ex.Message}");
        }
    }

    /// <summary>
    /// データソースを取得する
    /// </summary>
    /// <param name="service">利用するサービス名</param>
    /// <param name="startTime">開始日</param>
    /// <param name="endTime">終了日</param>
    /// <param name="Id">ID</param>
    private static List<int> ReadDataSource(FitnessService service, DateTimeOffset startTime, DateTimeOffset endTime, string Id)
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

            var values = new List<int>();

            foreach (var bucket in dataSet.Bucket)
            {
                foreach (var dataPoint in bucket.Dataset[0].Point)
                {
                    foreach (var value in dataPoint.Value)
                    {
                        if (value.IntVal.HasValue)
                        {
                            values.Add(value.IntVal.Value);
                        }
                    }
                }
            }

            return values;
        }
        catch (Google.GoogleApiException ex)
        {
            Console.WriteLine($"Google API error: {ex.Error.Message}");

            return new List<int>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General error: {ex.Message}");

            return new List<int>();
        }
    }

    /// <summary>
    /// 指定された期間の歩数を取得する
    /// </summary>
    /// <param name="startTime">開始日</param>
    /// <param name="endTime">終了日</param>
    public static List<int> ReadSteps(DateTimeOffset startTime, DateTimeOffset endTime)
        => ReadDataSource(Initializer_Activity, startTime, endTime, 
                          "derived:com.google.step_count.delta:com.google.android.gms:merge_step_deltas");
 
    /// <summary>
    /// 指定された期間の活動ポイントを取得する
    /// </summary>
    /// <param name="startTime">開始日</param>
    /// <param name="endTime">終了日</param>
    public static List<int> ReadActivity(DateTimeOffset startTime, DateTimeOffset endTime)
         => ReadDataSource(Initializer_Activity, startTime, endTime, "derived:com.google.active_minutes:com.google.android.gms:merge_active_minutes");

    /// <summary>
    /// 指定された期間の睡眠時間を取得する
    /// </summary>
    /// <param name="startTime">開始日</param>
    /// <param name="endTime">終了日</param>
    public static List<int> ReadSleepTime(DateTimeOffset startTime, DateTimeOffset endTime)
         => ReadDataSource(Initializer_Sleep, startTime, endTime, "derived:com.google.sleep.segment:com.google.android.gms:merged");

    /// <summary>
    /// 指定された期間のFitness記録を取得する
    /// </summary>
    /// <param name="startTime">開始日</param>
    /// <param name="endTime">終了日</param>
    /// <remarks>
    /// セグメントごとにリスト化されて返ってくる模様。
    /// </remarks>
    public async static void ReadFitnessDataAsync(DateTime startTime, DateTime endTime)
    {
        try
        {
            var dataSources = Initializer_Activity.Users.DataSources.List("me").ExecuteAsync();

            foreach (var dataSource in dataSources.Result.DataSource)
            {
                if (dataSource.DataType.Name == "com.google.activity.step_count" ||
                    dataSource.DataType.Name == "com.google.sleep.segment" || 
                    dataSource.DataType.Name == "com.google.activity.segment")
                {
                    var datasetId = $"{startTime.ToNanos_StartDate()}-{endTime.ToNanos_EndDate()}";
                    var request = Initializer_Activity.Users.DataSources.Datasets.Get("me", dataSource.DataStreamId, datasetId);
                    var dataSet = await request.ExecuteAsync();

                    foreach (var point in dataSet.Point)
                    {
                        Console.WriteLine($"Data Point from {point.StartTimeNanos} to {point.EndTimeNanos}");
                        foreach (var value in point.Value)
                        {
                            Console.WriteLine($"  Value: {value}");
                        }
                    }
                }
            }
        }
        catch (Google.GoogleApiException ex)
        {
            Console.WriteLine($"Google API error: {ex.Error.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General error: {ex.Message}");
        }
    }
}
