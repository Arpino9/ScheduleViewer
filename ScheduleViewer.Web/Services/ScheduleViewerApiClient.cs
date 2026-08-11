using System.Net.Http.Json;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using ScheduleViewer.Web.Models;

namespace ScheduleViewer.Web.Services;

public sealed class ScheduleViewerApiClient(HttpClient httpClient)
{
    private static readonly Regex BookTypePattern = new("コミック|文庫|単行本|新書|大型本|電子書籍|ペーパーバック", RegexOptions.Compiled);
    private static readonly Regex NumberPattern = new("[^0-9]", RegexOptions.Compiled);
    private static readonly Regex BreakPattern = new("<br\\s*/?>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex UrlPattern = new("https?://[^\\s<>\\\"']+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex SectionPattern = new("(?:^|\\n)【[^】]+】", RegexOptions.Compiled);

    public async Task<IReadOnlyList<ScheduleRecord>> GetSchedulesAsync(
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var events = await GetCalendarEventsAsync(date, cancellationToken);

        return events
            .Where(item => !item.Book && !item.Program)
            .OrderBy(item => item.IsAllDay ? DateTime.MinValue : item.StartDate)
            .Select(item => ToScheduleRecord(item, date))
            .ToList();
    }

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

    public async Task ReloadExpendituresAsync(CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsync(
            "api/drive/expenditure/reload", null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task ReloadAchievementsAsync(CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsync(
            "api/spreadsheet/achievement/reload", null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

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
                item.Book ? "本" : item.Program ? "アニメ" : item.IsAllDay ? "終日予定" : "予定",
                item.Book ? "books" : item.Program ? "anime" : "schedule"))
            .ToList();
    }

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

    public async Task ReloadTasksAsync(CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsync("api/tasks/reload", null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

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

    public async Task<IReadOnlyList<BookRecord>> GetBooksAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        var events = await GetCalendarEventsAsync(date, cancellationToken);
        var books = events.Where(x => x.Book || x.Description.Contains("【出版社】", StringComparison.Ordinal)).ToList();
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
