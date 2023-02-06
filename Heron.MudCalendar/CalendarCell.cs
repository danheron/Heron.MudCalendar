namespace Heron.MudCalendar;

public class CalendarCell<T> where T : CalendarItem
{
    public DateTime Date { get; set; }

    public IEnumerable<T> Items { get; set; } = new List<T>();
    
    public bool Outside { get; set; }
    
    public bool Today { get; set; }
}