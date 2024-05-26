using Google.Apis.Sheets.v4;

namespace ScheduleViewer.Infrastructure.Google_SpreadSheet;

/// <summary>
/// Google Calendar 読込
/// </summary>
public static class SheetReader
{
    /// <summary>
    /// 読込
    /// </summary>
    /// <param name="sheetId">シートID</param>
    /// <param name="sheetRange">シート範囲</param>
    public static IList<IList<object>> ReadOAuth(string sheetId, string sheetRange)
    {
        var service = Initialize();
        return GetTasks(service, sheetId, sheetRange);
    }

    /// <summary>
    /// 初期化
    /// </summary>
    /// <returns>Tasks Service</returns>
    private static SheetsService Initialize()
    {
        using (var stream = new FileStream(@"C:\Users\OKAJIMA\source\repos\SalaryManager\SalaryManager.Infrastructure\Google Calendar\\client_secret_732519433057-69j4ur5vdpca55vfscem296gesd5j16o.apps.googleusercontent.com.json", FileMode.Open, FileAccess.Read))
        {
            var secrets = GoogleClientSecrets.Load(stream).Secrets;
            var scope = SheetsService.Scope.Spreadsheets;
            var dataStore = new FileDataStore("token_Sheets", true);

            // tokenを保存するディレクトリ
            string credPath = "token_Sheets";
            Task<UserCredential> credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                secrets,
                new[] { scope },
                "user", CancellationToken.None, new FileDataStore(credPath, true));  // 第二引数をtrueにすると、カレントディレクトリからの相対パスに保存される

            return new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential.Result,
                ApplicationName = "myApi"
            });
        }
    }

    /// <summary>
    /// タスクの取得
    /// </summary>
    /// <param name="service">Google ToDo</param>
    /// <param name="sheetId">シートID</param>
    /// <param name="sheetRange">シート範囲</param>
    /// <returns>全てのタスク</returns>
    /// <remarks>
    /// ページ1つにつき最大100件までしか取得できないため、
    /// ページネーションを用いて全件取得できるまでループする。
    /// </remarks>
    private static IList<IList<object>> GetTasks(SheetsService service, 
                                                 string sheetId, 
                                                 string sheetRange)
    {
        if (service is null)
        {
            return new List<IList<object>>();
        }

        var request = service.Spreadsheets.Values.Get(sheetId, sheetRange);

        return request.Execute().Values;
    }
}
