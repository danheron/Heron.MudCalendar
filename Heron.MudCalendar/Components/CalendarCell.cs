namespace Heron.MudCalendar;

public class CalendarCell
{
    public DateTime Date { get; set; }

    public IEnumerable<CalendarItem> Items { get; set; } = new List<CalendarItem>();
    
    public bool Outside { get; set; }
    
    public bool Today { get; set; }
}