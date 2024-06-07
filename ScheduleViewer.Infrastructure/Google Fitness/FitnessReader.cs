namespace ScheduleViewer.Infrastructure.Google_Fitness;

/// <summary>
/// Fitness
/// </summary>
public sealed class FitnessReader
{
    /// <summary> 初期化 </summary>
    public static FitnessService Initializer => GoogleService<FitnessService>.Initialize_OAuth(
                                                 initializer => new FitnessService(initializer),
                                                 FitnessService.Scope.FitnessActivityRead,
                                                 "token_Fitness");

    public static async Task Read()
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


    public static void ReadSteps(DateTimeOffset startTime, DateTimeOffset endTime)
    {
        // 歩数を取得する
        try
        {
            var dataSet = Initializer.Users.Dataset.Aggregate(new AggregateRequest()
            {
                AggregateBy = new List<AggregateBy>()
            {
                new AggregateBy() { DataSourceId = "derived:com.google.step_count.delta", 
                                    DataTypeName = "com.google.step_count.delta" }
            },
                BucketByTime = new BucketByTime() { DurationMillis = 86400000 },
                StartTimeMillis = startTime.ToUnixTimeMilliseconds(),
                EndTimeMillis   = endTime.ToUnixTimeMilliseconds(),
            }, "me").Execute();

            // 結果の処理
            foreach (var bucket in dataSet.Bucket)
            {
                foreach (var dataPoint in bucket.Dataset[0].Point)
                {
                    foreach (var value in dataPoint.Value)
                    {
                        Console.WriteLine($"Steps: {value.IntVal}");
                    }
                }
            }
        }
        catch(Exception ex) 
        {
            Console.WriteLine(ex.ToString());
        }        
    }
}
