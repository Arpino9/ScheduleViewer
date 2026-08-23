namespace ScheduleViewer.Web.Models;

public sealed record AnimeRecord(
    string CalendarTitle,
    string Title,
    string Part,
    string EpisodesCount,
    string SubTitle,
    string WatchedFrom,
    string Caption,
    string SeasonYear,
    string SeasonName,
    string Cast,
    string Thumbnail,
    string EpisodeThumbnail,
    string OfficialSiteUrl,
    string WikipediaUrl);
