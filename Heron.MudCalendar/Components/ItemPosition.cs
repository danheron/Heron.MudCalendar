namespace Heron.MudCalendar;

public class ItemPosition
{
    public CalendarItem Item { get; set; } = new();
    public int Position { get; set; }
    public int Total { get; set; }
    public DateOnly Date { get; set; }
    public int Top { get; set; }
    public int Left { get; set; }
    public int Height { get; set; }
    public int Width { get; set; } = 1;
    public int Bottom => Top + Height;
}