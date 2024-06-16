namespace ScheduleViewer.Infrastructure.Google_Drive;

/// <summary>
/// Google Drive 読込
/// </summary>
public sealed class DriveReader
{
    /// <summary> 初期化 </summary>
    public static DriveService Initializer => GoogleService<DriveService>.Initialize_OAuth(
                                                initializer => new DriveService(initializer),
                                                new[] { DriveService.Scope.DriveReadonly },
                                                "token_Drive");

    public static List<ExpenditureEntity> Expenditures = new List<ExpenditureEntity>();

    public static async Task Initialize()
    {
        Expenditures.Clear();

        try
        {
            await Task.Run(() =>
            {
                var files = DriveReader.GetFilesInFolder(Shared.DriveFolderID);

                foreach (var file in files)
                {
                    if (file.Name.EndsWith(".csv"))
                    {
                        var contents = DriveReader.ReadCsvFileContent(file.ID);

                        // 見出し行は除外
                        var records = contents.Split('\n').Skip(1);

                        foreach (var record in records)
                        {
                            if (record.IsEmpty())
                            {
                                continue;
                            }

                            Expenditures.Add(new ExpenditureEntity(record.Split(',')));
                        }
                    }
                }
            }).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// 指定された日の支出を取得
    /// </summary>
    /// <param name="date">日付</param>
    /// <returns>支出</returns>
    public static List<ExpenditureEntity> GetExpenditure(DateTime date)
        => Expenditures.Any() ? 
           Expenditures.Where(x => x.Date.Year  == date.Year &&
                                   x.Date.Month == date.Month &&
                                   x.Date.Day   == date.Day).ToList() : 
           new List<ExpenditureEntity>();

    /// <summary>
    /// ファイル名を返す
    /// </summary>
    /// <param name="fileId">ファイルID</param>
    /// <returns>ファイル名</returns>
    public static string GetFileName(string fileId)
    {
        if (string.IsNullOrEmpty(fileId))
        {
            throw new FileReaderException("Google DriveのファイルIDが不正のため、ファイル名の取得に失敗しました。");
        }

        var request = Initializer.Files.Get(fileId);
        var file = request.Execute();

        return file.Name;
    }

    /// <summary>
    /// ファイルの最終更新日を返す
    /// </summary>
    /// <param name="fileId">ファイルID</param>
    /// <returns>最終更新日</returns>
    public static DateTimeOffset GetFileCreatedTime(string fileId)
    {
        if (string.IsNullOrEmpty(fileId))
        {
            throw new FileReaderException("Google DriveのファイルIDが不正のため、最終更新日の取得に失敗しました。");
        }

        var request = Initializer.Files.Get(fileId);
        var file = request.Execute();

        return file.CreatedTimeDateTimeOffset.HasValue ? file.CreatedTimeDateTimeOffset.Value : 
                                                         DateTimeOffset.MinValue;
    }

    /// <summary>
    /// フォルダを読み込み
    /// </summary>
    /// <param name="folderId">フォルダID</param>    
    /// <returns>(ファイルID, ファイル名)</returns>
    /// <exception cref="FileReaderException">ファイルの読み込み失敗</exception>
    /// <remarks>
    /// Google Driveから指定されたIDのフォルダを読み込み、フォルダ内のファイルを全て取得する。
    /// </remarks>
    public static List<(string ID, string Name)> GetFilesInFolder(string folderId)
    {
        FilesResource.ListRequest listRequest = Initializer.Files.List();

        listRequest.Q      = $"'{folderId}' in parents";
        listRequest.Fields = "nextPageToken, files(id, name)";

        var request = listRequest.Execute();
        
        if (request.Files is null || request.Files.IsEmpty())
        {
            throw new FileReaderException("Google Driveのフォルダが空です。");
        }

        var files = new List<(string ID, string Name)>();

        foreach (var file in request.Files)
        {
            files.Add((file.Id, file.Name));
        }

        return files;
    }

    /// <summary>
    /// CSVファイルを読み込み
    /// </summary>
    /// <param name="fileId"></param>
    /// <returns>CSVファイルの中身</returns>
    public static string ReadCsvFileContent(string fileId)
    {
        try
        {
            var request = Initializer.Files.Get(fileId);

            var stream = new MemoryStream();

            request.MediaDownloader.ProgressChanged += (IDownloadProgress progress) =>
            {
                switch (progress.Status)
                {
                    case DownloadStatus.Completed:
                        Console.WriteLine("Download complete.");
                        break;
                    case DownloadStatus.Failed:
                        Console.WriteLine("Download failed.");
                        break;
                }
            };

            request.Download(stream);

            stream.Position = 0;

            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
        catch (Exception ex) 
        {
            throw new FileReaderException("指定されたファイルの読み込みに失敗しました。", ex);
        }
    }
}
