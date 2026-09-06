namespace ScheduleViewer.WPF.Models.Presentation;

/// <summary>
/// カレンダーの1日分を表示するための項目です。
/// </summary>
public sealed class ScheduleItem
{
    private const int MaximumDailyEventCount = 5;

    /// <summary>
    /// 表示内容が空の項目を取得します。
    /// </summary>
    public static ScheduleItem Empty { get; } = new(
        default,
        default,
        default,
        default,
        Array.Empty<string>());

    /// <summary>
    /// カレンダーの1日分を表示するための項目を初期化します。
    /// </summary>
    /// <param name="foreground">文字色。</param>
    /// <param name="background">背景色。</param>
    /// <param name="date">日付。</param>
    /// <param name="allDayEvent">全日予定のタイトル。</param>
    /// <param name="dailyEvents">時刻が指定された予定のタイトル。</param>
    public ScheduleItem(
        Brush foreground,
        SolidColorBrush background,
        DateOnly date,
        string allDayEvent,
        IEnumerable<string> dailyEvents)
    {
        var eventTitles = dailyEvents?
            .Take(MaximumDailyEventCount)
            .ToArray() ?? Array.Empty<string>();

        Date = date;
        Day_Text = date == default ? string.Empty : date.Day.ToString();
        Background = background;
        Foreground = foreground;
        AllDayEvent_Text = string.IsNullOrEmpty(allDayEvent) ? default : $"★{allDayEvent}";
        DailyEvent1_Text = eventTitles.ElementAtOrDefault(0);
        DailyEvent2_Text = eventTitles.ElementAtOrDefault(1);
        DailyEvent3_Text = eventTitles.ElementAtOrDefault(2);
        DailyEvent4_Text = eventTitles.ElementAtOrDefault(3);
        DailyEvent5_Text = eventTitles.ElementAtOrDefault(4);
    }

    /// <summary>背景色を取得します。</summary>
    public SolidColorBrush Background { get; }

    /// <summary>文字色を取得します。</summary>
    public Brush Foreground { get; }

    /// <summary>日付を取得します。</summary>
    public DateOnly Date { get; }

    /// <summary>表示用の日を取得します。</summary>
    public string Day_Text { get; }

    /// <summary>表示用の全日予定を取得します。</summary>
    public string AllDayEvent_Text { get; }

    /// <summary>表示用の予定1を取得します。</summary>
    public string DailyEvent1_Text { get; }

    /// <summary>表示用の予定2を取得します。</summary>
    public string DailyEvent2_Text { get; }

    /// <summary>表示用の予定3を取得します。</summary>
    public string DailyEvent3_Text { get; }

    /// <summary>表示用の予定4を取得します。</summary>
    public string DailyEvent4_Text { get; }

    /// <summary>表示用の予定5を取得します。</summary>
    public string DailyEvent5_Text { get; }
}
