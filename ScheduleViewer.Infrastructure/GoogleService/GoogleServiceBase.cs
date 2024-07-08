namespace ScheduleViewer.Infrastructure.GoogleService;

/// <summary>
/// Google Service - 基底
/// </summary>
/// <remarks>
/// サービス共通用
/// </remarks>
internal abstract class GoogleServiceBase<Service> where Service : class
{
    /// <summary>
    /// 初期化子
    /// </summary>
    /// <returns>サービスのインスタンス</returns>
    /// <remarks>
    /// Initialize_OAuth()の初期化子
    /// </remarks>
    protected abstract Service Initializer { get; }

    /// <summary> API名 </summary>
    private string API_Name = "myApi";

    /// <summary>
    /// Factory - Google Service
    /// </summary>
    /// <param name="initializer">BaseClientService.Initializer</param>
    /// <returns>サービスのインスタンス</returns>
    protected delegate Service ServiceFactory(BaseClientService.Initializer initializer);

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="factory">ファクトリメソッド</param>
    /// <param name="scopes">スコープ</param>
    /// <param name="tokenFolderName">トークンフォルダ名</param>
    /// <returns>Initializer</returns>
    /// <remarks>
    /// OAuth認証を行い、トークンを生成する。スコープは複数設定可能。
    /// </remarks>
    protected Service Initialize_OAuth(ServiceFactory factory, string[] scopes, string tokenFolderName)
    {
        try
        {
            using (var stream = new FileStream(Shared.ClientSecret, FileMode.Open, FileAccess.Read))
            {
                var secrets   = GoogleClientSecrets.FromStream(stream).Secrets;
                var dataStore = new FileDataStore(tokenFolderName, true);

                Task<UserCredential> credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    secrets,
                    scopes,
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
    /// <param name="scopes">スコープ</param>
    /// <returns></returns>
    /// <remarks>
    /// Googleカレンダーに接続するための初期設定を行う。
    /// </remarks>
    protected Service Initialize_ServiceAccount(ServiceFactory factory, string keyPath, string[] scopes)
    {
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
