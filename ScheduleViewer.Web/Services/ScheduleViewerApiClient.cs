using System.Net.Http.Json;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.AspNetCore.Components.Forms;
using ScheduleViewer.Web.Models;

namespace ScheduleViewer.Web.Services;

/// <summary>ScheduleViewer APIを呼び出し、Blazor画面用のレコードへ変換します。</summary>
/// <param name="httpClient">ScheduleViewer APIのベースアドレスが設定されたHTTPクライアント。</param>
public sealed class ScheduleViewerApiClient(HttpClient httpClient)
{
    private static readonly Regex BookTypePattern = new("コミック|文庫|単行本|新書|大型本|電子書籍|ペーパーバック", RegexOptions.Compiled);
    private static readonly Regex NumberPattern = new("[^0-9]", RegexOptions.Compiled);
    private static readonly Regex BreakPattern = new("<br\\s*/?>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex UrlPattern = new("https?://[^\\s<>\\\"']+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex SectionPattern = new("(?:^|\\n)【[^】]+】", RegexOptions.Compiled);

    /// <summary>各外部サービスの認証状態を取得します。</summary>
    /// <param name="cancellationToken">要求を取り消すためのトークン。</param>
    /// <returns>サービス識別子をキー、認証済みかどうかを値とする読み取り専用ディクショナリ。</returns>
    public async Task<IReadOnlyDictionary<string, bool>> GetAuthStatusAsync(
        CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<Dictionary<string, bool>>(
            "api/auth/status", cancellationToken) ?? new Dictionary<string, bool>();

    /// <summary>指定した外部サービスの認証を開始し、認証ページの情報を取得します。</summary>
    /// <param name="service">認証対象のサービス識別子。</param>
    /// <param name="cancellationToken">要求を取り消すためのトークン。</param>
    /// <returns>認証状態、認証URL、説明メッセージを含むレスポンス。</returns>
    public async Task<AuthorizationResponseDto> AuthorizeServiceAsync(
        string service,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsync(
            $"api/auth/google/{Uri.EscapeDataString(service)}", null, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AuthorizationResponseDto>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("認証APIから応答が返されませんでした。");
    }

    /// <summary>Google Healthへのインポート用書き込み認証の状態を取得します。</summary>
    /// <param name="cancellationToken">要求を取り消すためのトークン。</param>
    /// <returns>インポート用認証が完了している場合は<see langword="true"/>。</returns>
    public async Task<bool> GetGoogleHealthImportStatusAsync(CancellationToken cancellationToken = default)
    {
        var status = await httpClient.GetFromJsonAsync<GoogleHealthImportStatusDto>(
            "api/fitbit/import/status", cancellationToken);
        return status?.Authorized ?? false;
    }

    /// <summary>Google Healthへのインポート用書き込み認証を開始します。</summary>
    /// <param name="force">保存済みトークンを破棄して再認証する場合は<see langword="true"/>。</param>
    /// <param name="cancellationToken">要求を取り消すためのトークン。</param>
    /// <returns>認証状態と認証ページのURL。</returns>
    public async Task<AuthorizationResponseDto> AuthorizeGoogleHealthImportAsync(
        bool force = false,
        CancellationToken cancellationToken = default)
    {
        var uri = "api/fitbit/import/auth" + (force ? "?force=true" : string.Empty);
        using var response = await httpClient.PostAsync(uri, null, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AuthorizationResponseDto>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("認証APIから応答が返されませんでした。");
    }

    /// <summary>Google Fit TakeoutのJSONファイルをGoogle Healthへ取り込みます。</summary>
    /// <param name="files">アップロードするJSONファイル。</param>
    /// <param name="cancellationToken">要求を取り消すためのトークン。</param>
    /// <returns>ファイルごとの登録件数、重複件数、エラー。</returns>
    public async Task<GoogleFitImportResponseDto> ImportGoogleFitAsync(
        IReadOnlyList<IBrowserFile> files,
        CancellationToken cancellationToken = default)
    {
        using var form = new MultipartFormDataContent();
        foreach (var file in files)
        {
            var content = new StreamContent(file.OpenReadStream(25 * 1024 * 1024, cancellationToken));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
                string.IsNullOrWhiteSpace(file.ContentType) ? "application/json" : file.ContentType);
            form.Add(content, "files", file.Name);
        }

        using var response = await httpClient.PostAsync("api/fitbit/import/google-fit", form, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiMessageDto>(cancellationToken: cancellationToken);
            throw new InvalidOperationException(error?.Message ?? $"インポートに失敗しました ({(int)response.StatusCode})");
        }
        return await response.Content.ReadFromJsonAsync<GoogleFitImportResponseDto>(cancellationToken: cancellationToken)
            ?? new GoogleFitImportResponseDto();
    }

    /// <summary>指定日のカレンダーイベントから、書籍・アニメを除いた予定を取得します。</summary>
    /// <param name="date">取得対象の日付。</param>
    /// <param name="cancellationToken">要求を取り消すためのトークン。</param>
    /// <returns>開始時刻順に並んだ予定の読み取り専用リスト。</returns>
    public async Task<IReadOnlyList<ScheduleRecord>> GetSchedulesAsync(
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var events = await GetCalendarEventsAsync(date, cancellationToken);

        return events
            .Where(item => !item.IsBook && !item.IsProgram)
            .OrderBy(item => item.IsAllDay ? DateTime.MinValue : item.StartDate)
            .Select(item => ToScheduleRecord(item, date))
            .ToList();
    }

    /// <summary>住所を地図表示用の緯度・経度へ変換します。</summary>
    /// <param name="address">検索する住所または場所名。</param>
    /// <param name="cancellationToken">要求を取り消すためのトークン。</param>
    /// <returns>取得できた位置情報。住所が空または取得に失敗した場合は<see langword="null"/>。</returns>
    public async Task<MapLocationRecord?> GetMapLocationAsync(
        string address,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(address)) return null;

        try
        {
            return await httpClient.GetFromJsonAsync<MapLocationRecord>(
                $"api/map/geocode?address={Uri.EscapeDataString(address)}",
                cancellationToken);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            return null;
        }
    }

    /// <summary>予定の説明文にある写真セクションからGoogle Photosリンクを抽出します。</summary>
    /// <param name="schedules">抽出対象の予定。</param>
    /// <returns>許可ホストに限定し、URLの重複を除いた写真リンク。</returns>
    public static IReadOnlyList<PhotoLinkRecord> ExtractPhotoLinks(IEnumerable<ScheduleRecord> schedules)
    {
        var results = new List<PhotoLinkRecord>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var schedule in schedules)
        {
            var normalized = WebUtility.HtmlDecode(BreakPattern.Replace(schedule.Description, "\n"));
            var markerIndex = normalized.IndexOf("【写真】", StringComparison.Ordinal);
            if (markerIndex < 0) continue;

            var photoSection = normalized[(markerIndex + "【写真】".Length)..];
            var nextSection = SectionPattern.Match(photoSection);
            if (nextSection.Success) photoSection = photoSection[..nextSection.Index];

            foreach (Match match in UrlPattern.Matches(photoSection))
            {
                var url = match.Value;
                if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
                    !IsGooglePhotosHost(uri.Host) ||
                    !seen.Add(url)) continue;

                results.Add(new PhotoLinkRecord(schedule.Title, url));
            }
        }

        return results;
    }

    /// <summary>予定に添付された有効なHTTPまたはHTTPSリンクを抽出します。</summary>
    /// <param name="schedules">抽出対象の予定。</param>
    /// <returns>URLの重複を除いた添付リンク。</returns>
    public static IReadOnlyList<AttachmentLinkRecord> ExtractAttachmentLinks(IEnumerable<ScheduleRecord> schedules)
    {
        var results = new List<AttachmentLinkRecord>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var schedule in schedules)
        {
            foreach (var attachment in schedule.Attachments)
            {
                if (!Uri.TryCreate(attachment.Url, UriKind.Absolute, out var uri) ||
                    (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps) ||
                    !seen.Add(attachment.Url)) continue;

                results.Add(attachment);
            }
        }

        return results;
    }

    private static bool IsGooglePhotosHost(string host)
        => host.Equals("photos.google.com", StringComparison.OrdinalIgnoreCase) ||
           host.Equals("photos.app.goo.gl", StringComparison.OrdinalIgnoreCase);

    /// <summary>指定日の支出情報を取得します。</summary>
    /// <param name="date">取得対象の日付。</param>
    /// <param name="cancellationToken">要求を取り消すためのトークン。</param>
    /// <returns>支出情報の読み取り専用リスト。</returns>
    public async Task<IReadOnlyList<ExpenditureRecord>> GetExpendituresAsync(
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var items = await httpClient.GetFromJsonAsync<List<ExpenditureDto>>(
            $"api/drive/expenditure/date/{date:yyyy-MM-dd}", cancellationToken) ?? [];

        return items.Select(item => new ExpenditureRecord(
                item.ItemName,
                item.Price,
                item.FinancialInstitutions,
                item.CategoryLarge,
                item.CategoryMiddle,
                item.Memo,
                string.Equals(item.CanCalc, "はい", StringComparison.Ordinal)))
            .ToList();
    }

    /// <summary>支出情報をデータソースから再読込するようAPIへ要求します。</summary>
    /// <param name="cancellationToken">要求を取り消すためのトークン。</param>
    public async Task ReloadExpendituresAsync(CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsync(
            "api/drive/expenditure/reload", null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>Steam実績画像のキャッシュを破棄し、次回取得時に実績情報を再読込させます。</summary>
    /// <param name="cancellationToken">要求を取り消すためのトークン。</param>
    public async Task ReloadAchievementsAsync(CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsync(
            "api/spreadsheet/achievement/reload", null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>Box共有リンクのメタデータをカレンダーイベントへ添付します。</summary>
    /// <param name="eventId">添付先のGoogle CalendarイベントID。</param>
    /// <param name="fileUrl">Box共有リンクのURL。</param>
    /// <param name="fileTitle">画面へ表示するファイル名。</param>
    /// <param name="cancellationToken">要求を取り消すためのトークン。</param>
    public async Task AttachBoxFileAsync(
        string eventId,
        string fileUrl,
        string fileTitle,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            $"api/calendar/events/{Uri.EscapeDataString(eventId)}/attachments",
            new { fileUrl, fileTitle },
            cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>Google Photos共有URLをカレンダーイベントの説明へ追加します。</summary>
    /// <param name="eventId">追加先のGoogle CalendarイベントID。</param>
    /// <param name="photoUrl">追加するGoogle Photos共有URL。</param>
    /// <param name="cancellationToken">要求を取り消すためのトークン。</param>
    public async Task AddPhotoUrlAsync(
        string eventId,
        string photoUrl,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            $"api/calendar/events/{Uri.EscapeDataString(eventId)}/photo",
            new { photoUrl },
            cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>アニメ視聴記録をGoogle Calendarへ登録します。</summary>
    /// <param name="date">視聴日。</param>
    /// <param name="title">作品タイトル。</param>
    /// <param name="episode">話数。</param>
    /// <param name="subtitle">サブタイトル。</param>
    /// <param name="service">視聴先サービス。</param>
    /// <param name="summary">概要またはメモ。</param>
    /// <param name="cancellationToken">要求を取り消すためのトークン。</param>
    /// <returns>APIが返した登録完了メッセージ。</returns>
    public async Task<string> RegisterAnimeAsync(
        DateOnly date,
        string title,
        int episode,
        string subtitle,
        string service,
        string summary,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "api/anime/register",
            new
            {
                date = date.ToString("yyyy-MM-dd"),
                title,
                episode,
                subtitle,
                service,
                summary
            },
            cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<AnimeRegisterResponseDto>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("登録APIから応答が返されませんでした。");
        if (!string.Equals(result.Status, "ok", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(result.Message.Length > 0 ? result.Message : "登録に失敗しました。");
        }

        return result.Message.Length > 0 ? result.Message : "視聴記録を登録しました";
    }

    /// <summary>通常の予定をGoogle Calendarへ登録します。</summary>
    /// <param name="title">予定のタイトル。</param>
    /// <param name="date">予定の日付。</param>
    /// <param name="allDay">終日予定の場合は<see langword="true"/>。</param>
    /// <param name="startTime">時刻指定予定の開始時刻。</param>
    /// <param name="endTime">時刻指定予定の終了時刻。</param>
    /// <param name="location">予定の場所。</param>
    /// <param name="description">予定の説明。</param>
    /// <param name="cancellationToken">要求を取り消すためのトークン。</param>
    /// <returns>登録完了メッセージ。</returns>
    public async Task<string> RegisterCalendarEventAsync(
        string title,
        DateOnly date,
        bool allDay,
        TimeOnly startTime,
        TimeOnly endTime,
        string location,
        string description,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "api/calendar/events",
            new
            {
                title,
                date = date.ToString("yyyy-MM-dd"),
                allDay,
                startTime = allDay ? null : startTime.ToString("HH:mm"),
                endTime = allDay ? null : endTime.ToString("HH:mm"),
                location,
                description
            },
            cancellationToken);

        CalendarRegisterResponseDto? result = null;
        try
        {
            result = await response.Content.ReadFromJsonAsync<CalendarRegisterResponseDto>(
                cancellationToken: cancellationToken);
        }
        catch (JsonException) when (!response.IsSuccessStatusCode)
        {
        }

        if (!response.IsSuccessStatusCode || result is null ||
            !string.Equals(result.Status, "ok", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(string.IsNullOrWhiteSpace(result?.Message)
                ? "カレンダーへの登録に失敗しました"
                : result.Message);
        }

        return result.Message;
    }

    /// <summary>カレンダーの非同期再読込が完了するまで状態をポーリングします。</summary>
    /// <param name="cancellationToken">待機を取り消すためのトークン。</param>
    public async Task WaitForCalendarReloadAsync(CancellationToken cancellationToken = default)
    {
        await Task.Delay(300, cancellationToken);
        for (var attempt = 0; attempt < 60; attempt++)
        {
            var loading = await httpClient.GetFromJsonAsync<bool>("api/calendar/status", cancellationToken);
            if (!loading) return;
            await Task.Delay(500, cancellationToken);
        }
    }

    /// <summary>キーワードに一致するカレンダーイベントを検索します。</summary>
    /// <param name="query">検索キーワード。</param>
    /// <param name="cancellationToken">要求を取り消すためのトークン。</param>
    /// <returns>開始日順に並んだ検索結果。</returns>
    public async Task<IReadOnlyList<CalendarSearchRecord>> SearchSchedulesAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        var events = await httpClient.GetFromJsonAsync<List<CalendarEventDto>>(
            $"api/calendar/search?q={Uri.EscapeDataString(query.Trim())}", cancellationToken) ?? [];

        return events
            .OrderBy(item => item.StartDate)
            .Select(item => new CalendarSearchRecord(
                DateOnly.FromDateTime(item.StartDate),
                item.IsAllDay ? "終日" : item.StartDate.ToString("HH:mm"),
                CleanTitle(item.DisplayTitle.Length > 0 ? item.DisplayTitle : item.Title),
                item.Place,
                item.Description,
                item.IsBook ? "本" : item.IsProgram ? "アニメ" : item.IsAllDay ? "終日予定" : "予定",
                item.IsBook ? "books" : item.IsProgram ? "anime" : "schedule"))
            .ToList();
    }

    /// <summary>指定日が期限のGoogle Tasksを取得します。</summary>
    /// <param name="date">取得対象の日付。</param>
    /// <param name="cancellationToken">要求を取り消すためのトークン。</param>
    /// <returns>未完了を優先して並べたタスクの読み取り専用リスト。</returns>
    public async Task<IReadOnlyList<TaskRecord>> GetTasksAsync(
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var tasks = await httpClient.GetFromJsonAsync<List<TaskDto>>(
            $"api/tasks/date/{date:yyyy-MM-dd}", cancellationToken) ?? [];

        return tasks
            .OrderBy(item => item.Completed.HasValue)
            .ThenBy(item => item.DueDate ?? DateTime.MaxValue)
            .Select(item => new TaskRecord(
                item.TaskListName,
                item.TaskName,
                item.Details,
                item.DueDate,
                item.Completed.HasValue))
            .ToList();
    }

    /// <summary>Google Tasksを再読込するようAPIへ要求します。</summary>
    /// <param name="cancellationToken">要求を取り消すためのトークン。</param>
    public async Task ReloadTasksAsync(CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsync("api/tasks/reload", null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>指定日のGoogle Health活動量、心拍数、体重、睡眠情報を取得します。</summary>
    /// <param name="date">取得対象の日付。</param>
    /// <param name="cancellationToken">要求を取り消すためのトークン。</param>
    /// <returns>取得できた各種データをまとめた健康記録。</returns>
    public async Task<FitbitHealthRecord> GetHealthAsync(
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var activityTask = httpClient.GetFromJsonAsync<FitbitActivityDto>(
            $"api/fitbit/activity?date={date:yyyy-MM-dd}", cancellationToken);
        var heartTask = GetOrDefaultAsync<FitbitHeartDto>(
            $"api/fitbit/heart?date={date:yyyy-MM-dd}", cancellationToken);
        var weightTask = GetOrDefaultAsync<FitbitWeightDto>(
            $"api/fitbit/weight?date={date:yyyy-MM-dd}", cancellationToken);
        var sleepTask = GetOrDefaultAsync<FitbitSleepDto>(
            $"api/fitbit/sleep?date={date:yyyy-MM-dd}", cancellationToken);

        await Task.WhenAll(activityTask, heartTask, weightTask, sleepTask);

        var activity = await activityTask ?? new FitbitActivityDto();
        var heart = await heartTask ?? new FitbitHeartDto();
        var weight = await weightTask ?? new FitbitWeightDto();
        var sleep = await sleepTask ?? new FitbitSleepDto();

        return new FitbitHealthRecord(
            activity.Steps,
            activity.CaloriesOut,
            activity.Elevation,
            activity.Distance,
            heart.RestingHeartRate,
            weight.Bmi,
            weight.Weight,
            ParseFitbitDateTime(sleep.StartTime),
            ParseFitbitDateTime(sleep.EndTime),
            ParseFitbitDuration(sleep.Sleeping),
            ParseFitbitDuration(sleep.Awake),
            ParseFitbitDuration(sleep.Restless),
            ParseFitbitDuration(sleep.Rem),
            ParseFitbitDuration(sleep.Asleep));
    }

    /// <summary>指定日のカレンダーイベントから書籍情報を取得します。</summary>
    /// <param name="date">取得対象の日付。</param>
    /// <param name="cancellationToken">要求を取り消すためのトークン。</param>
    /// <returns>説明文を項目別に解析した書籍情報。</returns>
    public async Task<IReadOnlyList<BookRecord>> GetBooksAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        var events = await GetCalendarEventsAsync(date, cancellationToken);
        var books = events.Where(x => x.IsBook || x.Description.Contains("【出版社】", StringComparison.Ordinal)).ToList();
        var results = new List<BookRecord>(books.Count);

        foreach (var item in books)
        {
            var fields = ParseBookDescription(item.Description);
            results.Add(new BookRecord(
                CleanTitle(item.Title),
                date,
                Value(fields, "著者"),
                Value(fields, "出版社"),
                Value(fields, "発売日"),
                Value(fields, "本の種類"),
                Value(fields, "ISBN-10"),
                Value(fields, "ISBN-13"),
                Value(fields, "本の概要"),
                Value(fields, "サムネイル"),
                Value(fields, "本の評価")));
        }

        return results;
    }

    /// <summary>指定日のアニメ視聴イベントを取得し、作品情報とサムネイルを付加します。</summary>
    /// <param name="date">取得対象の日付。</param>
    /// <param name="cancellationToken">要求を取り消すためのトークン。</param>
    /// <returns>作品情報を付加したアニメ視聴記録。</returns>
    public async Task<IReadOnlyList<AnimeRecord>> GetAnimeAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        var events = await httpClient.GetFromJsonAsync<List<CalendarEventDto>>(
            $"api/calendar/anime?date={date:yyyy-MM-dd}", cancellationToken) ?? [];
        var results = new List<AnimeRecord>(events.Count);

        foreach (var item in events)
        {
            var fields = ParseSections(item.Description);
            var normalizedTitle = item.Title.Replace('_', ' ').Replace('　', ' ');
            var matchTitle = GetAnimeMatchTitle(normalizedTitle);
            var part = GetPart(normalizedTitle);

            var animeTask = GetOrDefaultAsync<List<AnimeApiDto>>(
                $"api/anime?title={Uri.EscapeDataString(matchTitle)}&first=5&castFirst=10", cancellationToken);
            var thumbnailTask = GetOrDefaultAsync<Dictionary<string, string>>(
                $"api/spreadsheet/thumbnail?title={Uri.EscapeDataString(matchTitle)}", cancellationToken);
            var episodeThumbnailTask = GetOrDefaultAsync<Dictionary<string, string>>(
                $"api/spreadsheet/episode-thumbnail?title={Uri.EscapeDataString(item.Title)}", cancellationToken);

            await Task.WhenAll(animeTask, thumbnailTask, episodeThumbnailTask);
            var matches = await animeTask ?? [];
            var anime = FindAnime(matches, matchTitle);
            var thumbnailData = await thumbnailTask;
            var episodeThumbnailData = await episodeThumbnailTask;

            results.Add(new AnimeRecord(
                CleanTitle(item.Title),
                anime?.Title ?? matchTitle,
                part,
                anime?.EpisodesCount ?? string.Empty,
                Value(fields, "サブタイトル"),
                Value(fields, "視聴先"),
                Value(fields, "概要"),
                anime?.SeasonYear ?? string.Empty,
                anime?.SeasonName ?? string.Empty,
                anime?.Cast ?? string.Empty,
                DictionaryValue(thumbnailData, "url", anime?.Thumbnail),
                DictionaryValue(episodeThumbnailData, "url"),
                anime?.OfficialSiteUrl ?? string.Empty,
                anime?.WikipediaUrl ?? string.Empty));
        }

        return results;
    }

    /// <summary>書籍イベントの説明文を解析し、書誌情報を項目名と値の組へ変換します。</summary>
    /// <param name="description">解析対象の説明文。</param>
    /// <returns>説明文から抽出した書誌情報。</returns>
    internal static Dictionary<string, string> ParseBookDescription(string description)
    {
        var result = ParseSections(description);
        var lines = NormalizeLines(description);

        foreach (var line in lines)
        {
            var separator = line.IndexOf(" : ", StringComparison.Ordinal);
            if (separator < 0) continue;
            var key = line[..separator].Trim();
            var value = line[(separator + 3)..].Trim();

            if (key is "発売日" or "ISBN-10" or "ISBN-13") result[key] = value;
            else if (BookTypePattern.IsMatch(key)) result["本の種類"] = key;
        }

        var captionIndex = Array.FindIndex(lines, x => x.StartsWith("【本の概要】", StringComparison.Ordinal));
        var ratingIndex = Array.FindIndex(lines, x => x.StartsWith("【本の評価】", StringComparison.Ordinal));
        if (captionIndex >= 0 && ratingIndex > captionIndex)
        {
            result["本の概要"] = string.Join('\n', lines[(captionIndex + 1)..ratingIndex]).Trim();
        }

        return result;
    }

    /// <summary>「【項目名】」で始まる説明文のセクションを解析します。</summary>
    /// <param name="description">解析対象の説明文。</param>
    /// <returns>セクション名をキー、後続テキストを値とするディクショナリ。</returns>
    internal static Dictionary<string, string> ParseSections(string description)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        var currentKey = string.Empty;
        var values = new List<string>();

        void Flush()
        {
            if (currentKey.Length > 0) result[currentKey] = string.Join('\n', values).Trim();
        }

        foreach (var line in NormalizeLines(description))
        {
            var match = Regex.Match(line, "^【([^】]+)】(.*)");
            if (match.Success)
            {
                Flush();
                currentKey = match.Groups[1].Value.Trim();
                var inlineValue = match.Groups[2].Value.Trim();
                values = inlineValue.Length > 0 ? [inlineValue] : [];
            }
            else if (currentKey.Length > 0)
            {
                values.Add(line);
            }
        }

        Flush();
        return result;
    }

    private async Task<T?> GetOrDefaultAsync<T>(string uri, CancellationToken cancellationToken)
    {
        try { return await httpClient.GetFromJsonAsync<T>(uri, cancellationToken); }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException) { return default; }
    }

    private async Task<List<CalendarEventDto>> GetCalendarEventsAsync(
        DateOnly date,
        CancellationToken cancellationToken)
        => await httpClient.GetFromJsonAsync<List<CalendarEventDto>>(
            $"api/calendar?date={date:yyyy-MM-dd}", cancellationToken) ?? [];

    private static string FormatDuration(DateTime start, DateTime end)
    {
        var duration = end - start;
        if (duration <= TimeSpan.Zero) return string.Empty;

        var hours = (int)duration.TotalHours;
        var minutes = duration.Minutes;
        if (hours > 0 && minutes > 0) return $"{hours}時間{minutes}分";
        if (hours > 0) return $"{hours}時間";
        return $"{minutes}分";
    }

    private static string GetScheduleKind(CalendarEventDto item)
    {
        if (item.IsAllDay) return "private";
        if (item.Description.Contains("健康", StringComparison.Ordinal) ||
            item.Title.Contains("病院", StringComparison.Ordinal) ||
            item.Title.Contains("運動", StringComparison.Ordinal)) return "health";
        return "work";
    }

    private static ScheduleRecord ToScheduleRecord(CalendarEventDto item, DateOnly date)
        => new(
            item.EventId,
            date,
            item.IsAllDay ? "終日" : item.StartDate.ToString("HH:mm"),
            item.IsAllDay ? string.Empty : FormatDuration(item.StartDate, item.EndDate),
            CleanTitle(item.DisplayTitle.Length > 0 ? item.DisplayTitle : item.Title),
            item.IsAllDay ? "終日" : "予定",
            GetScheduleKind(item),
            item.Place,
            item.Description,
            item.AchievementImageUrl,
            item.IsAllDay,
            item.Attachments
                .Select(attachment => new AttachmentLinkRecord(
                    attachment.Title.Length > 0
                        ? attachment.Title
                        : CleanTitle(item.DisplayTitle.Length > 0 ? item.DisplayTitle : item.Title),
                    attachment.Url,
                    attachment.MimeType))
                .ToList());

    private static DateTime? ParseFitbitDateTime(string value)
        => DateTime.TryParse(value, out var parsed) && parsed.Year > 1 ? parsed : null;

    private static TimeSpan ParseFitbitDuration(string value)
    {
        try { return XmlConvert.ToTimeSpan(value); }
        catch (FormatException) { return TimeSpan.Zero; }
    }

    private static AnimeApiDto? FindAnime(IReadOnlyList<AnimeApiDto> candidates, string matchTitle)
    {
        static string Normalize(string value) => value.Replace('　', ' ');
        return candidates.FirstOrDefault(x => Normalize(x.Title) == matchTitle)
            ?? candidates.FirstOrDefault(x => matchTitle.StartsWith(Normalize(x.Title), StringComparison.Ordinal))
            ?? candidates.LastOrDefault(x => Normalize(x.Title).StartsWith(matchTitle + " ", StringComparison.Ordinal));
    }

    private static string GetAnimeMatchTitle(string title)
    {
        var parts = title.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 2 ? $"{parts[0]} {parts[1]}" : parts.FirstOrDefault() ?? title;
    }

    private static string GetPart(string title)
    {
        var parts = title.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var raw = parts.Length > 2 ? parts[2] : parts.Length > 1 ? parts[1] : string.Empty;
        return NumberPattern.Replace(raw, string.Empty);
    }

    private static string[] NormalizeLines(string value) => value.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
    private static string CleanTitle(string value) => value.TrimStart('\uFEFF').Replace("ï»¿", string.Empty, StringComparison.Ordinal);
    private static string Value(IReadOnlyDictionary<string, string> values, string key, string? fallback = null)
        => values.TryGetValue(key, out var value) && value.Length > 0 ? value : fallback ?? string.Empty;
    private static string DictionaryValue(IReadOnlyDictionary<string, string>? values, string key, string? fallback = null)
        => values is not null && values.TryGetValue(key, out var value) && value.Length > 0 ? value : fallback ?? string.Empty;
}
