using ScheduleViewer.Domain.Modules.Helpers;

namespace ScheduleViewer.Domain.Entities;

/// <summary>
/// Entity - スケジュール
/// </summary>
public sealed class ScheduleEntity
{
    public ScheduleEntity(
        SolidColorBrush basckground,
        DateTime date,
        string allDayEvent,
        string event1,
        string event2,
        string event3,
        string event4,
        string event5)
    {
        this.Date             = date;

        if (date == default)
        {
            this.Day_Text = string.Empty;
        }
        else
        {
            this.Day_Text = date.Day.ToString();
        }
        
        this.Background       = basckground;
        this.AllDayEvent_Text = string.IsNullOrEmpty(allDayEvent) ? default : "★" + allDayEvent;
        this.DailyEvent1_Text = event1;
        this.DailyEvent2_Text = event2;
        this.DailyEvent3_Text = event3;
        this.DailyEvent4_Text = event4;
        this.DailyEvent5_Text = event5;
    }

    /// <summary> 背景色 </summary>
    public SolidColorBrush Background { get; set; }

    /// <summary> 日付 </summary>
    public DateTime Date { get; set; }

    /// <summary> 日(表示用) </summary>
    public string Day_Text { get; set; }

    /// <summary> 全日イベント </summary>
    public string AllDayEvent_Text { get; set; }
    
    /// <summary> イベント1 </summary>
    public string DailyEvent1_Text { get; set; }
    
    /// <summary> イベント2 </summary>
    public string DailyEvent2_Text { get; set; }
    
    /// <summary> イベント3 </summary>
    public string DailyEvent3_Text { get; set; }
    
    /// <summary> イベント4 </summary>
    public string DailyEvent4_Text { get; set; }
    
    /// <summary> イベント5 </summary>
    public string DailyEvent5_Text { get; set; }
}
