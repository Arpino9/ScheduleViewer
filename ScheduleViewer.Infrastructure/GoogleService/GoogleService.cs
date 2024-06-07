namespace ScheduleViewer.Infrastructure.GoogleService;

/// <summary>
/// Google Service
/// </summary>
/// <remarks>
/// サービス共通用
/// </remarks>
internal static class GoogleService<T> where T : class
{
    private static string API_Name = "myApi";

    /// <summary>
    /// Factory - Google Service
    /// </summary>
    /// <param name="initializer">BaseClientService.Initializer</param>
    /// <returns>Tのインスタンス</returns>
    internal delegate T ServiceFactory(BaseClientService.Initializer initializer);

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="factory">ファクトリメソッド</param>
    /// <param name="scope">スコープ</param>
    /// <param name="tokenFolderName">トークンフォルダ名</param>
    /// <returns>Initializer</returns>
    internal static T Initialize_OAuth(ServiceFactory factory, string scope, string tokenFolderName)
    {
        try
        {
            using (var stream = new FileStream(Shared.ClientSecret, FileMode.Open, FileAccess.Read))
            {
                var secrets = GoogleClientSecrets.FromStream(stream).Secrets;
                var dataStore = new FileDataStore(tokenFolderName, true);

                Task<UserCredential> credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    secrets,
                    new[] { scope },
                    "user", CancellationToken.None, dataStore);

                var initializer = new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential.Result,
                    ApplicationName       = API_Name,
                };

                return factory(initializer);
            }
        }
        catch(Domain.Exceptions.FormatException ex) 
        {
            throw new Domain.Exceptions.FormatException(ex.Message);
        }
    }

    /// <summary>
    /// 初期化 - サービスアカウント
    /// </summary>
    /// <param name="factory">ファクトリメソッド</param>
    /// <param name="keyPath">鍵</param>
    /// <param name="scope">スコープ</param>
    /// <returns></returns>
    /// <remarks>
    /// Googleカレンダーに接続するための初期設定を行う。
    /// </remarks>
    internal static T Initialize_ServiceAccount(ServiceFactory factory, string keyPath, string scope)
    {
        // Google Calendar APIの認証
        string[] scopes = { scope };

        GoogleCredential credential;

        using (var stream = new FileStream(keyPath, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(scopes);
        }

        var service = new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName       = API_Name,
        };

        return factory(service);
    }
}
