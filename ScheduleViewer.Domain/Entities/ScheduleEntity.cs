using ScheduleViewer.Domain.Modules.Helpers;

namespace ScheduleViewer.Domain.Entities;

public sealed class ScheduleEntity
{
    public ScheduleEntity(
        DateTime date,
        string allDayEvent,
        string event1,
        string event2,
        string event3,
        string event4,
        string event5)
    {
        this.Date             = date;
        this.Day_Text         = date.Day.ToString();
        this.AllDayEvent_Text = allDayEvent.IsNull() ? default : "★" + allDayEvent;
        this.DailyEvent1_Text = event1;
        this.DailyEvent2_Text = event2;
        this.DailyEvent3_Text = event3;
        this.DailyEvent4_Text = event4;
        this.DailyEvent5_Text = event5;
    }

    public DateTime Date { get; set; }

    public string Day_Text { get; set; }

    public string AllDayEvent_Text { get; set; }
    public string DailyEvent1_Text { get; set; }
    public string DailyEvent2_Text { get; set; }
    public string DailyEvent3_Text { get; set; }
    public string DailyEvent4_Text { get; set; }
    public string DailyEvent5_Text { get; set; }
}
