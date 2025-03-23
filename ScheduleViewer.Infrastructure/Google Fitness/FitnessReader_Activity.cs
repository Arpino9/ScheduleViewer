namespace ScheduleViewer.Infrastructure.Google_Fitness;

/// <summary>
/// Google Fitness (活動記録) - 読み込み
/// </summary>
internal sealed class FitnessReader_Activity : GoogleServiceBase<FitnessService>
{
    /// <summary> 
    /// 初期化子 
    /// </summary>
    protected override FitnessService Initializer
    {
        get => base.Initialize_OAuth(initializer => new FitnessService(initializer),
                                     new[] { FitnessService.Scope.FitnessActivityRead },
                                     "token_FitnessActivity");
    } 

    internal async Task ReadSession()
    {
        try
        {
            var response = Initializer.Users.Sessions.List("me").Execute();

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
    internal void ReadData()
    {
        try
        {
            var dataSources = Initializer.Users.DataSources.List("me").Execute();

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
    internal List<ActivityEntity> ReadActivitiesSource(FitnessService service, DateTimeOffset startTime, DateTimeOffset endTime, string Id)
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

    /// <summary> 歩数 </summary>
    internal List<(DateTime Date, int Value)> Steps { get; set; }

    /// <summary> 活動内容 </summary>
    internal List<ActivityEntity> Activities { get; set; }

    /// <summary>
    /// 指定された期間の活動ポイントを取得する
    /// </summary>
    /// <param name="startTime">開始日</param>
    /// <param name="endTime">終了日</param>
    internal async Task ReadActivity(DateTimeOffset startTime, DateTimeOffset endTime)
    {
        await Task.Run(() =>
        {
            Activities = ReadActivitiesSource(Initializer, startTime, endTime, 
                        "derived:com.google.activity.segment:com.google.android.gms:merge_activity_segments");
        }).ConfigureAwait(false);
    }

    /// <summary>
    /// 指定された期間の歩数を取得する
    /// </summary>
    /// <param name="startTime">開始日</param>
    /// <param name="endTime">終了日</param>
    internal async Task ReadSteps(DateTimeOffset startTime, DateTimeOffset endTime)
    {
        await Task.Run(() =>
        {
            Steps = ReadDataSource(Initializer, startTime, endTime,
                   "derived:com.google.step_count.delta:com.google.android.gms:merge_step_deltas");
        }).ConfigureAwait(false);
    }

    /// <summary>
    /// 日付で検索
    /// </summary>
    /// <param name="date">日付</param>
    /// <returns>歩数</returns>
    internal List<int> FindStepsByDate(DateOnly date)
        => Steps.Any() ?
           Steps.Where(x => x.Date.Year  == date.Year &&
                            x.Date.Month == date.Month &&
                            x.Date.Day   == date.Day)
                .Select(x => x.Value).ToList() :
           new List<int>();

    /// <summary>
    /// 日付で検索
    /// </summary>
    /// <param name="date">日付</param>
    /// <returns>活動記録</returns>
    internal List<ActivityEntity> FindActivitiesByDate(DateOnly date)
        => Activities.Any() ?
           Activities.Where(x => x.Date.Year  == date.Year &&
                                 x.Date.Month == date.Month &&
                                 x.Date.Day   == date.Day).ToList() :
           new List<ActivityEntity>();

    /// <summary>
    /// 指定された期間のFitness記録を取得する
    /// </summary>
    /// <param name="startTime">開始日</param>
    /// <param name="endTime">終了日</param>
    /// <remarks>
    /// セグメントごとにリスト化されて返ってくる模様。
    /// </remarks>
    internal async void ReadFitnessDataAsync(DateTime startTime, DateTime endTime)
    {
        try
        {
            var dataSources = Initializer.Users.DataSources.List("me").ExecuteAsync();

            foreach (var dataSource in dataSources.Result.DataSource)
            {
                if (dataSource.DataType.Name == "com.google.activity.step_count" ||
                    dataSource.DataType.Name == "com.google.sleep.segment" || 
                    dataSource.DataType.Name == "com.google.activity.segment")
                {
                    var datasetId = $"{startTime.ToNanos_StartDate()}-{endTime.ToNanos_EndDate()}";
                    var request = Initializer.Users.DataSources.Datasets.Get("me", dataSource.DataStreamId, datasetId);
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
