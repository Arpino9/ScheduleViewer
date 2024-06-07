namespace ScheduleViewer.Infrastructure.GoogleService;

/// <summary>
/// Google Service
/// </summary>
/// <remarks>
/// サービス共通用
/// </remarks>
internal static class GoogleService<T>
{
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
    internal static T Initialize(ServiceFactory factory, string scope, string tokenFolderName)
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
                    ApplicationName = "myApi",
                };

                return factory(initializer);
            }
        }
        catch(Domain.Exceptions.FormatException ex) 
        {
            throw new Domain.Exceptions.FormatException(ex.Message);
        }
    }
}
