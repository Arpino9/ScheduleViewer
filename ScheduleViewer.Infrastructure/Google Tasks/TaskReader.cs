namespace ScheduleViewer.Infrastructure.GoogleTasks;

/// <summary>
/// Google Tasks 読込
/// </summary>
public sealed class TaskReader
{
    /// <summary> 初期化 </summary>
    public static TasksService Initializer => GoogleService<TasksService>.Initialize_OAuth(
                                              initializer => new TasksService(initializer),
                                              TasksService.Scope.Tasks,
                                              "token_Tasks");

    public static List<TaskEntity> Entities { get; private set; } = new List<TaskEntity>();

    /// <summary>
    /// 読込
    /// </summary>
    public static void ReadOAuth()
    {
        var services  = Initialize();
        var taskLists = GetTaskLists();

        var titleLabel = taskLists[0][0].ToString();

        foreach (var task in taskLists)
        {
            if (task[0].ToString() == titleLabel)
            {
                continue;
            }

            var taskListName = task[0].ToString();
            var taskList = GetTasks(Initializer, task[1].ToString());

            foreach (var todo in taskList) 
            {
                if (todo.Completed.IsNull() ||
                    todo.Due.IsNull())
                {
                    continue;
                }

                Entities.Add(new TaskEntity(taskListName, todo.Title, todo.Notes, todo.Completed.ToDateTime(), todo.Due.ToDateTime()));
            }
        }

        Entities = Entities.OrderByDescending(x => x.DueDate).ToList();
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

    /// <summary>
    /// タスク一覧の取得
    /// </summary>
    /// <returns>タスク一覧</returns>
    private static IList<IList<object>> GetTaskLists()
    {
        var sheetId    = "1tc5uFTh09PBVVnV2OYmGZ3svY6C-6SwCAF6KIUO8l9c";
        var sheetRange = "タスク一覧!A:B";

        return SheetReader.ReadOAuth(sheetId, sheetRange);
    }

    /// <summary>
    /// タスクの取得
    /// </summary>
    /// <returns>全てのタスク</returns>
    /// <remarks>
    /// ページ1つにつき最大100件までしか取得できないため、
    /// ページネーションを用いて全件取得できるまでループする。
    /// </remarks>
    private static IReadOnlyList<Google.Apis.Tasks.v1.Data.Task> GetTasks(TasksService service, string sheetId)
    {
        if (service is null)
        {
            return new List<Google.Apis.Tasks.v1.Data.Task>();
        }

        //var taskListId = "ekFqcmthbmtuRVdQNlJZaQ";
        var request = service.Tasks.List(sheetId);

        request.MaxResults = 100;

        // 最初のページ
        request.PageToken = null;

        request.ShowCompleted = true;
        request.ShowDeleted = false;
        request.ShowHidden = true;

        var schedules = new List<Google.Apis.Tasks.v1.Data.Task>();

        do
        {
            // イベントを取得
            var events = request.Execute();

            if (events is null || events.Items.IsEmpty())
            {
                throw new DatabaseException("スケジュールの取得に失敗しました。");
            }

            // イベントの処理
            foreach (var eventItem in events.Items)
            {
                if (eventItem != null)
                {
                    // イベントの詳細を処理
                    schedules.Add(eventItem);
                }
            }

            // 次のページのトークンを設定
            request.PageToken = events.NextPageToken;
        } while (!String.IsNullOrEmpty(request.PageToken));

        return schedules.ToList().AsReadOnly();
    }

    public static IReadOnlyList<TaskEntity> FindByDate(DateTime date)
        => Entities.Where(x => x.DueDate.Year  == date.Year &&
                               x.DueDate.Month == date.Month &&
                               x.DueDate.Day   == date.Day).ToList();
}
