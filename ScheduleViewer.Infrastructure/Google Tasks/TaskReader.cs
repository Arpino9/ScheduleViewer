namespace ScheduleViewer.Infrastructure.GoogleTasks;

/// <summary>
/// Google Tasks 読込
/// </summary>
public sealed class TaskReader
{
    /// <summary>
    /// 読込
    /// </summary>
    public static void Read()
    {
        var service = Initialize();

        var taskListId = "ekFqcmthbmtuRVdQNlJZaQ"; // タスクリストのIDを指定
        var tasks = service.Tasks.List(taskListId).Execute();

        foreach (var task in tasks.Items)
        {
            Console.WriteLine(task.Title);
        }
    }

    /// <summary>
    /// 初期化
    /// </summary>
    /// <returns>Tasks Service</returns>
    private static TasksService Initialize()
    {
        using (var stream = new FileStream(@"C:\Users\OKAJIMA\source\repos\SalaryManager\SalaryManager.Infrastructure\Google Calendar\\client_secret_732519433057-69j4ur5vdpca55vfscem296gesd5j16o.apps.googleusercontent.com.json", FileMode.Open, FileAccess.Read))
        {
            var secrets = GoogleClientSecrets.Load(stream).Secrets;
            var scope = TasksService.Scope.Tasks;
            var dataStore = new FileDataStore("token_Tasks", true);

            // tokenを保存するディレクトリ
            string credPath = "token_Tasks";
            Task<UserCredential> credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                secrets,
                new[] { scope },
                "user", CancellationToken.None, new FileDataStore(credPath, true));  // 第二引数をtrueにすると、カレントディレクトリからの相対パスに保存される

            return new TasksService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential.Result,
                ApplicationName = "myApi"
            });
        }
    }
}
