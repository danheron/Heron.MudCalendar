namespace Heron.MudCalendar;

public class CalendarItem
{
    public DateTime Start { get; set; }
    
    public DateTime? End { get; set; }
    
    public bool AllDay { get; set; }

    public string Text { get; set; } = string.Empty;
}
