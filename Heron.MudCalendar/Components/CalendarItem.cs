namespace Heron.MudCalendar;

public class CalendarItem
{
    public DateTime Start { get; set; }
    
    public DateTime? End { get; set; }
    
    public bool AllDay { get; set; }

    public string Text { get; set; } = string.Empty;

    protected internal bool IsMultiDay => (End == null && Start.TimeOfDay > TimeSpan.FromHours(23)) ||
                                          (End.HasValue && End.Value.Date > Start.Date);

    protected internal readonly string Id = Guid.NewGuid().ToString();
}
