using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;

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
    private static List<(DateTime Date, int Value)> ReadDataSource(FitnessService service, DateTimeOffset startTime, DateTimeOffset endTime, string Id)
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
                    var datetimeEnd   = dataPoint.EndTimeNanos.Value.ToDateTime();

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

    /// <summary>
    /// データソースを取得する
    /// </summary>
    /// <param name="service">利用するサービス名</param>
    /// <param name="startTime">開始日</param>
    /// <param name="endTime">終了日</param>
    /// <param name="Id">ID</param>
    private static List<ActivityEntity> ReadActivitiesSource(FitnessService service, DateTimeOffset startTime, DateTimeOffset endTime, string Id)
    {
        try
        {
            var entities = new List<ActivityEntity>();

            var dataSet = service.Users.Dataset.Aggregate(new AggregateRequest()
            {
                AggregateBy = new List<AggregateBy>()
                {
                    new AggregateBy() { DataSourceId = Id }
                },
                BucketByTime = new BucketByTime() { DurationMillis = 86400000 },
                StartTimeMillis = startTime.ToUnixTimeMilliseconds(),
                EndTimeMillis = endTime.ToUnixTimeMilliseconds(),
            }, "me").ExecuteAsync();

            var activities = JSONExtension.DeserializeSettings<IReadOnlyList<JSONProperty_Activities>>(FilePath.GetJSONActivitiesDefaultPath());

            foreach (var bucket in dataSet.Result.Bucket)
            {
                foreach (var dataPoint in bucket.Dataset[0].Point)
                {
                    var list = new List<int>();

                    var datetimeStart = dataPoint.StartTimeNanos.Value.ToDateTime();
                    var datetimeEnd   = dataPoint.EndTimeNanos.Value.ToDateTime();

                    // 結果を表示
                    Console.WriteLine("DateTime: " + datetimeStart.ToString("yyyy-MM-dd HH:mm:ss.fffffff"));
                    Console.WriteLine("DateTime: " + datetimeEnd.ToString("yyyy-MM-dd HH:mm:ss.fffffff"));

                    foreach (var value in dataPoint.Value)
                    {
                        if (value.IntVal.HasValue)
                        {
                            list.Add(value.IntVal.Value);
                        }
                    }

                    var record = activities.Where(x => x.ID == list[0]).FirstOrDefault();
                    if (record != null)
                    {
                        entities.Add(new ActivityEntity(record.ID, record.Name, datetimeEnd, list[1], list[2]));
                    }
                }
            }

            return entities;
        }
        catch (Google.GoogleApiException ex)
        {
            Console.WriteLine($"Google API error: {ex.Error.Message}");

            return new List<ActivityEntity>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General error: {ex.Message}");

            return new List<ActivityEntity>();
        }
    }

    public static List<(DateTime Date, int Value)> Steps { get; set; }

    /// <summary>
    /// 指定された期間の歩数を取得する
    /// </summary>
    /// <param name="startTime">開始日</param>
    /// <param name="endTime">終了日</param>
    public async static Task ReadSteps(DateTimeOffset startTime, DateTimeOffset endTime)
    {
        await Task.Run(() =>
        {
            Steps = ReadDataSource(Initializer_Activity, startTime, endTime, 
                   "derived:com.google.step_count.delta:com.google.android.gms:merge_step_deltas");
        }).ConfigureAwait(false);
    }

    /// <summary>
    /// 日付で検索
    /// </summary>
    /// <param name="date">日付</param>
    /// <returns>歩数</returns>
    public static List<int> FindStepsByDate(DateTime date)
        => Steps.Any() ?
           Steps.Where(x => x.Date.Year  == date.Year &&
                            x.Date.Month == date.Month &&
                            x.Date.Day   == date.Day)
                .Select(x => x.Value).ToList() : 
           new List<int>();

    private static List<ActivityEntity> Activities { get; set; }

    /// <summary>
    /// 指定された期間の活動ポイントを取得する
    /// </summary>
    /// <param name="startTime">開始日</param>
    /// <param name="endTime">終了日</param>
    public async static Task ReadActivity(DateTimeOffset startTime, DateTimeOffset endTime)
    {
        await Task.Run(() =>
        {
            Activities = ReadActivitiesSource(Initializer_Activity, startTime, endTime, "derived:com.google.activity.segment:com.google.android.gms:merge_activity_segments");
        }).ConfigureAwait(false);
    }

    /// <summary>
    /// 日付で検索
    /// </summary>
    /// <param name="date">日付</param>
    /// <returns>活動記録</returns>
    public static List<ActivityEntity> FindActivitiesByDate(DateTime date)
        => Activities.Any() ?
           Activities.Where(x => x.Date.Year  == date.Year &&
                                 x.Date.Month == date.Month &&
                                 x.Date.Day   == date.Day).ToList() :
           new List<ActivityEntity>();

    private static List<(DateTime Date, int Value)> SleepingTime { get; set; }

    /// <summary>
    /// 指定された期間の睡眠時間を取得する
    /// </summary>
    /// <param name="startTime">開始日</param>
    /// <param name="endTime">終了日</param>
    public async static Task ReadSleepTime(DateTimeOffset startTime, DateTimeOffset endTime)
    {
        await Task.Run(() =>
        {
            SleepingTime = ReadDataSource(Initializer_Sleep, startTime, endTime, "derived:com.google.sleep.segment:com.google.android.gms:merged");
        }).ConfigureAwait(false);
    }

    /// <summary>
    /// 日付で検索
    /// </summary>
    /// <param name="date">日付</param>
    /// <returns>睡眠時間</returns>
    public static List<int> FindSleepTimeByDate(DateTime date)
        => SleepingTime.Any() ?
           SleepingTime.Where(x => x.Date.Year  == date.Year &&
                                   x.Date.Month == date.Month &&
                                   x.Date.Day   == date.Day)
                       .Select(x => x.Value).ToList() :
           new List<int>();

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
