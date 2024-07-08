namespace ScheduleViewer.Infrastructure.Google_Fitness;

/// <summary>
/// Google Fitness - 書き込み
/// </summary>
internal sealed class FitnessWriter : GoogleServiceBase<FitnessService>
{
    /// <summary> 
    /// 初期化子 
    /// </summary>
    protected override FitnessService Initializer
    {
        get => base.Initialize_OAuth(initializer => new FitnessService(initializer),
                                     new[] { FitnessService.Scope.FitnessActivityWrite },
                                     "token_FitnessActivity_Write");
    } 

    /// <summary>
    /// データソースの新規作成
    /// </summary>
    /// <param name="service">利用するサービス名</param>
    /// <returns>データソース</returns>
    /// <remarks>
    /// DataSoruceはデバイスごとに生成する必要があるため、ない場合は新規作成する。
    /// 既にデータソースが存在する場合は例外が出るので注意する。
    /// </remarks>
    private DataSource CreateDataSource(FitnessService service)
    {
        var dataSource = new DataSource()
        {
            Type = "derived",
            Application = new Application()
            {
                //TODO:アプリ名称は外部から呼び出す
                Name = "myapi",
                Version = "1"
            },
            DataType = new DataType()
            {
                Name = "com.google.activity.segment",
                Field = new List<DataTypeField>()
                {
                    new DataTypeField()
                    {
                        Name   = "activity",
                        Format = "integer"
                    }
                }
            },
            Device = new Device()
            {
                Uid          = "scale",
                Type         = "phone",
                Model        = "iPhone15",
                Manufacturer = "Apple",
                Version      = "1"
            },
            DataStreamName = "FitnessDataStream"
        };

        try
        {
            var dataSourceRequest = service.Users.DataSources.Create(dataSource, "me");
            return dataSourceRequest.Execute();
        }
        catch (Google.GoogleApiException ex)
        {
            throw new FileWriterException(ex.Message);
        }
        catch (Exception ex)
        {
            throw new FileWriterException(ex.Message);
        }
    }

    /// <summary>
    /// データソースの書き込み
    /// </summary>
    /// <param name="service">利用するサービス名</param>
    /// <param name="startTime">開始日</param>
    /// <param name="endTime">終了日</param>
    internal void WriteDataSource(DateTimeOffset startTime, DateTimeOffset endTime)
    {
        var dataSourceId = GetDataSourceId(Initializer);
        //dataSourceId = "derived:com.google.activity.segment:com.google.android.gms:aggregated";

        var dataSet = new Dataset()
        {
            DataSourceId   = dataSourceId,
            MaxEndTimeNs   = endTime.ToNanos(),
            MinStartTimeNs = startTime.ToNanos(),
            Point          = new List<DataPoint>()
            {
                new DataPoint()
                {
                    DataTypeName   = "com.google.activity.segment",
                    StartTimeNanos = startTime.ToNanos(),
                    EndTimeNanos   = endTime.ToNanos(),
                    Value = new List<Value>()
                    {
                        new Value()
                        {
                            //TODO:列挙子を使う
                            IntVal = 8  // 8は筋力トレーニングのアクティビティタイプを表します
                        }
                    }
                }
            }
        };

        // データセットをGoogle Fitに書き込む
        var dataSets = Initializer.Users.DataSources.Datasets;
        
        try
        {
            var request = dataSets.Patch(dataSet, "me", dataSourceId, $"{startTime}-{endTime}");
            request.Execute();
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
    /// データソースIDを取得
    /// </summary>
    /// <param name="service">利用するサービス名</param>
    /// <returns>データソースID</returns>
    private string GetDataSourceId(FitnessService service)
    {
        var dataSourceList = service.Users.DataSources.List("me").Execute();

        if (dataSourceList.DataSource.Any())
        {
            // データソースが存在する
            return dataSourceList.DataSource.First().DataStreamId;
        }
        else
        {
            // データソースが存在しない
            return CreateDataSource(service).DataStreamId;
        }
    }
}
