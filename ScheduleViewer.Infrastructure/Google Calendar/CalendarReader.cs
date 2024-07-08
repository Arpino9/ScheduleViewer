﻿namespace ScheduleViewer.Infrastructure.Google_Calendar;

/// <summary>
/// Google Calendar 読込
/// </summary>
/// <remarks>
/// やや読込に時間がかかるため、起動時に非同期でロードしておくこと。
/// </remarks>
internal class CalendarReader : GoogleServiceBase<CalendarService>
{
    /// <summary> Googleカレンダーのイベント </summary>
    private List<CalendarEventsEntity> CalendarEvents = new List<CalendarEventsEntity>();

    /// <summary> 取得判定用 </summary>
    internal Executing Loading { get; set; }

    /// <summary> 
    /// 初期化子 
    /// </summary>
    protected override CalendarService Initializer
    {
        get => base.Initialize_OAuth(initializer => new CalendarService(initializer),
                                     new[] { CalendarService.Scope.CalendarReadonly },
                                     "token_Calendar");
    }

    /// <summary>
    /// OAuth読込
    /// </summary>
    /// <remarks>
    /// OAuth認証後、カレンダーに登録されたイベントを取得する。
    /// </remarks>
    internal async Task InitializeAsync()
    {
        using (Loading = new Executing())
        {
            try
            {
                await Task.Run(() =>
                {
                    CalendarEvents.Clear();

                    var events = GetEvents(Initializer);

                    var attachments = events.Where(x => x.Attachments != null);

                    foreach (var eventItem in events)
                    {
                        if (String.IsNullOrEmpty(eventItem.Start.DateTime.ToString()) ||
                            eventItem.Location is null)
                        {
                            CalendarEvents.Add(new CalendarEventsEntity(eventItem.Summary,
                                                                       Convert.ToDateTime(eventItem.Start.Date),
                                                                       Convert.ToDateTime(eventItem.End.Date),
                                                                       eventItem.Description));

                            continue;
                        }

                        CalendarEvents.Add(new CalendarEventsEntity(eventItem.Summary,
                                                                   eventItem.Start.DateTime.Value,
                                                                   eventItem.End.DateTime.Value,
                                                                   eventItem.Location,
                                                                   eventItem.Description));
                    }
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }

    /// <summary>
    /// イベントの取得
    /// </summary>
    /// <param name="service">カレンダー</param>
    /// <returns>全てのイベント</returns>
    /// <remarks>
    /// ページ1つにつき最大2500件までしか取得できないため、
    /// ページネーションを用いて全件取得できるまでループする。
    /// </remarks>
    private IReadOnlyList<Event> GetEvents(CalendarService service)
    {
        if (service is null)
        {
            return new List<Event>();
        }

        EventsResource.ListRequest request = service.Events.List("okajima100@gmail.com");

        request.MaxResults = 2500;

        // 最初のページ
        request.PageToken = null;

        var schedules = new List<Event>();

        do
        {
            // イベントを取得
            Events events = request.Execute();

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

        return schedules.OrderBy(x => x.Start.DateTime).ToList().AsReadOnly();
    }

    /// <summary>
    /// イベントを取得する
    /// </summary>
    /// <param name="title">タイトル</param>
    /// <param name="startDate">開始日付</param>
    /// <returns>イベント</returns>
    /// <remarks>
    /// 指定されたタイトル、開始日と一致するイベントを取得する。
    /// </remarks>
    internal IReadOnlyList<CalendarEventsEntity> FindByTitle(string title, DateTime startDate)
        => CalendarEvents.Any() ?
           CalendarEvents.Where(x => x.Title != null &&
                                     x.Title.Contains(title) &&
                                     x.StartDate == startDate).ToList().AsReadOnly() :
           new List<CalendarEventsEntity>();

    /// <summary>
    /// イベントを取得する
    /// </summary>
    /// <param name="address">住所</param>
    /// <returns>イベント</returns>
    /// <remarks>
    /// 指定された住所と一致するイベントを抽出する。
    /// </remarks>
    internal IReadOnlyList<CalendarEventsEntity> FindByAddress(string address)
        => CalendarEvents.Any() ?
           CalendarEvents.Where(x => x.Place != null &&
                                     x.Place.Contains(address)).ToList().AsReadOnly() :
           new List<CalendarEventsEntity>();

    /// <summary>
    /// イベントを取得する
    /// </summary>
    /// <param name="date">開始日付</param>
    /// <returns>イベント</returns>
    /// <remarks>
    /// 指定された日付と一致するイベントを取得する。
    /// </remarks>
    internal IReadOnlyList<CalendarEventsEntity> FindByDate(DateTime date)
        => CalendarEvents.Any() ?
           CalendarEvents.Where(x => x.StartDate.Year  == date.Year && 
                                     x.StartDate.Month == date.Month &&
                                     x.StartDate.Day   == date.Day).ToList().AsReadOnly() :
           new List<CalendarEventsEntity>();

    /// <summary>
    /// イベントを取得する
    /// </summary>
    /// <param name="startDate">開始日付</param>
    /// <param name="endDate">終了日付</param>
    /// <returns>イベント</returns>
    /// <remarks>
    /// 指定された開始日、終了日と一致するイベントを取得する。
    /// </remarks>
    internal IReadOnlyList<CalendarEventsEntity> FindByDate(DateTime startDate, DateTime endDate)
        => CalendarEvents.Any() ?
           CalendarEvents.Where(x => x.StartDate.Year  == startDate.Year &&
                                     x.StartDate.Month == startDate.Month &&
                                     x.StartDate.Day   == startDate.Day &&
                                     x.EndDate.Year    == endDate.Year &&
                                     x.EndDate.Month   == endDate.Month &&
                                     x.EndDate.Day     == endDate.Day).ToList().AsReadOnly() :
           new List<CalendarEventsEntity>();

    /// <summary>
    /// イベントを取得する
    /// </summary>
    /// <param name="startDate">開始日付</param>
    /// <param name="endDate">終了日付</param>
    /// <returns>イベント</returns>
    /// <remarks>
    /// 指定された開始日、終了日と一致するイベントを取得する。
    /// </remarks>
    internal IReadOnlyList<CalendarEventsEntity> FindByDate(DateTime startDate, DateTime endDate, TimeSpan startTime)
        => CalendarEvents.Any() ?
           CalendarEvents.Where(x => x.StartDate.Year  == startDate.Year &&
                                     x.StartDate.Month == startDate.Month &&
                                     x.StartDate.Day   == startDate.Day &&
                                     x.EndDate.Year    == endDate.Year &&
                                     x.EndDate.Month   == endDate.Month &&
                                     x.EndDate.Day     == endDate.Day &&
                                     new TimeSpan(x.StartDate.Hour, x.StartDate.Minute, 0) >= startTime)
                         .ToList().AsReadOnly() :
           new List<CalendarEventsEntity>();

    /// <summary>
    /// イベントを取得する
    /// </summary>
    /// <param name="address">住所</param>
    /// <param name="startDate">開始日付</param>
    /// <returns>イベント</returns>
    /// <remarks>
    /// 指定された住所、開始日と一致するイベントを取得する。
    /// </remarks>
    internal IReadOnlyList<CalendarEventsEntity> FindByAddress(string address, DateTime startDate)
        => CalendarEvents.Any() ?
           CalendarEvents.Where(x => x.Place != null &&
                                     x.Place.Contains(address) &&
                                     x.StartDate == startDate).ToList().AsReadOnly() :
           new List<CalendarEventsEntity>();

    /// <summary>
    /// イベントを取得する
    /// </summary>
    /// <param name="address">住所</param>
    /// <param name="startDate">開始日付</param>
    /// <param name="endDate">終了日付</param>
    /// <returns>イベント</returns>
    /// <remarks>
    /// 指定された住所、開始日、終了日と一致するイベントを取得する。
    /// </remarks>
    internal IReadOnlyList<CalendarEventsEntity> FindByAddress(string address, DateTime startDate, DateTime endDate)
        => CalendarEvents.Any() ?
           CalendarEvents.Where(x => x.Place != null &&
                                     x.Place.Contains(address) &&
                                     x.StartDate.Year  == startDate.Year &&
                                     x.StartDate.Month == startDate.Month &&
                                     x.StartDate.Day   == startDate.Day &&
                                     x.EndDate.Year    == endDate.Year &&
                                     x.EndDate.Month   == endDate.Month &&
                                     x.EndDate.Day     == endDate.Day).ToList().AsReadOnly() :
           new List<CalendarEventsEntity>();

    /// <summary>
    /// イベントを取得する
    /// </summary>
    /// <param name="title">タイトル</param>
    /// <param name="startDate">開始日付</param>
    /// <param name="endDate">終了日付</param>
    /// <returns>イベント</returns>
    /// <remarks>
    /// 指定されたタイトル、開始日、終了日と一致するイベントを取得する。
    /// </remarks>
    internal IReadOnlyList<CalendarEventsEntity> FindByTitle(string title, DateTime startDate, DateTime endDate)
        => CalendarEvents.Any() ?
           CalendarEvents.Where(x => x.Title != null &&
                                     x.Title.Contains(title) &&
                                     x.StartDate.Year  >= startDate.Year &&
                                     x.StartDate.Month >= startDate.Month &&
                                     x.StartDate.Day   >= startDate.Day &&
                                     x.EndDate.Year    <= endDate.Year &&
                                     x.EndDate.Month   <= endDate.Month &&
                                     x.EndDate.Day     <= endDate.Day).ToList().AsReadOnly() :
           new List<CalendarEventsEntity>();

    /// <summary>
    /// イベントを取得する
    /// </summary>
    /// <param name="address">住所</param>
    /// <param name="startTime">開始時刻</param>
    /// <param name="endTime">終了時刻</param>
    /// <returns>イベント</returns>
    /// <remarks>
    /// 指定された住所、開始時刻、終了時刻と一致するイベントを取得する。
    /// </remarks>
    internal IReadOnlyList<CalendarEventsEntity> FindByAddress(string address, TimeSpan startTime, TimeSpan endTime)
        => CalendarEvents.Any() ?
           CalendarEvents.Where(x => x.Place != null &&
                                     x.Place.Contains(address) &&
                                     x.StartDate.Hour   >= startTime.Hours &&
                                     x.StartDate.Minute >= startTime.Minutes &&
                                     x.EndDate.Hour     >= endTime.Hours &&
                                     x.EndDate.Minute   >= endTime.Minutes).ToList().AsReadOnly() :
           new List<CalendarEventsEntity>();

    /// <summary>
    /// イベントを取得する
    /// </summary>
    /// <param name="address">住所</param>
    /// <param name="startDate">開始日付</param>
    /// <param name="endDate">終了日付</param>
    /// <param name="startTime">開始時刻</param>
    /// <returns>イベント</returns>
    /// <remarks>
    /// 指定された住所、開始日時、終了日と一致するイベントを取得する。
    /// </remarks>
    internal IReadOnlyList<CalendarEventsEntity> FindByAddress(string address, DateTime startDate, DateTime endDate, TimeSpan startTime)
        => CalendarEvents.Any() ?
           CalendarEvents.Where(x => x.Place != null &&
                                     x.Place.Contains(address) &&
                                     x.StartDate.Year  >= startDate.Year &&
                                     x.StartDate.Month >= startDate.Month &&
                                     x.StartDate.Day   >= startDate.Day &&
                                     new TimeSpan(x.StartDate.Hour, x.StartDate.Minute, 0) >= new TimeSpan(startTime.Hours, startTime.Minutes, 0) &&
                                     x.EndDate.Year    <= endDate.Year &&
                                     x.EndDate.Month   <= endDate.Month &&
                                     x.EndDate.Day     <= endDate.Day).ToList().AsReadOnly() :
           new List<CalendarEventsEntity>();

    /// <summary>
    /// イベントを取得する
    /// </summary>
    /// <param name="address">住所</param>
    /// <param name="startDate">開始日付</param>
    /// <param name="endDate">終了日付</param>
    /// <param name="startTime">開始時刻</param>
    /// <param name="endTime">終了時刻</param>
    /// <returns>イベント</returns>
    /// <remarks>
    /// 指定された住所、開始日時、終了日時と一致するイベントを取得する。
    /// </remarks>
    internal IReadOnlyList<CalendarEventsEntity> FindByAddress(string address, DateTime startDate, DateTime endDate, TimeSpan startTime, TimeSpan endTime)
        => CalendarEvents.Any() ?
           CalendarEvents.Where(x => x.Place != null &&
                                     x.Place.Contains(address) &&
                                     x.StartDate.Year   >= startDate.Year &&
                                     x.StartDate.Month  >= startDate.Month &&
                                     x.StartDate.Day    >= startDate.Day &&
                                     x.StartDate.Hour   >= startTime.Hours &&
                                     x.StartDate.Minute >= startTime.Minutes &&
                                     x.EndDate.Year     <= endDate.Year &&
                                     x.EndDate.Month    <= endDate.Month &&
                                     x.EndDate.Day      <= endDate.Day &&
                                     x.EndDate.Hour     <= endTime.Hours &&
                                     x.EndDate.Minute   <= endTime.Minutes).ToList().AsReadOnly() :
           new List<CalendarEventsEntity>();
}