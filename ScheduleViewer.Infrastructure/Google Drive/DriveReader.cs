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
    /// <returns>ファイルの中身</returns>
    /// <exception cref="FileReaderException">ファイルの読み込み失敗</exception>
    /// <remarks>
    /// Google Driveから指定されたIDのフォルダを読み込み、フォルダ内のファイルを取得する。
    /// ファイルがCSV形式であれば、その中身を返す。
    /// </remarks>
    public static string ReadFolder(string folderId)
    {
        FilesResource.ListRequest listRequest = Initializer.Files.List();

        listRequest.Q      = $"'{folderId}' in parents";
        listRequest.Fields = "nextPageToken, files(id, name)";

        var request = listRequest.Execute();
        
        if (request.Files is null || request.Files.IsEmpty())
        {
            // フォルダが空
            return string.Empty;
        }

        foreach (var file in request.Files)
        {
            Console.WriteLine("{0} ({1})", file.Name, file.Id);

            if (file.Name.EndsWith(".csv"))
            {
                return ReadCsvFileContent(file.Id);
            }
        }

        throw new FileReaderException($"Google Driveのフォルダ( {folderId} )を読み込めませんでした。");
    }

    /// <summary>
    /// CSVファイルを読み込み
    /// </summary>
    /// <param name="fileId"></param>
    /// <returns>CSVファイルの中身</returns>
    public static string ReadCsvFileContent(string fileId)
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
}
