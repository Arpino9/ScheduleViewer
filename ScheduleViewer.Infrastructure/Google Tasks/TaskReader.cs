namespace ScheduleViewer.Infrastructure.Google_Tasks;

/// <summary>
/// Google Tasks 読込
/// </summary>
internal class TaskReader : GoogleServiceBase<TasksService>
{
    /// <summary> 
    /// 初期化子 
    /// </summary>
    protected override TasksService Initializer
    {
        get => base.Initialize_OAuth(initializer => new TasksService(initializer),
                                     new[] { TasksService.Scope.Tasks },
                                     "token_Tasks");
    }
    
    private List<TaskEntity> Entities { get; set; } = new List<TaskEntity>();

    /// <summary>
    /// 読込
    /// </summary>
    internal async Task InitializeAsync()
    {
        await Task.Run(() =>
        {
            var taskLists = GoogleFacade.SpreadSheet.Initialize_Tasks();

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
        }).ConfigureAwait(false);
    }

    /// <summary>
    /// タスクの取得
    /// </summary>
    /// <returns>全てのタスク</returns>
    /// <remarks>
    /// ページ1つにつき最大100件までしか取得できないため、
    /// ページネーションを用いて全件取得できるまでループする。
    /// </remarks>
    private IReadOnlyList<Google.Apis.Tasks.v1.Data.Task> GetTasks(TasksService service, string sheetId)
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

    /// <summary>
    /// 対象日から検索
    /// </summary>
    /// <param name="date">対象日</param>
    /// <returns>タスク</returns>
    internal IReadOnlyList<TaskEntity> FindTasksByDate(DateOnly date)
        => Entities.Any() ?
           Entities.Where(x => x.DueDate.Year  == date.Year &&
                               x.DueDate.Month == date.Month &&
                               x.DueDate.Day   == date.Day).ToList() :
           new List<TaskEntity>();
}
