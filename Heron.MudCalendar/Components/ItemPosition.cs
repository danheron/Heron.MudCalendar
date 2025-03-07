using System.Diagnostics.CodeAnalysis;

namespace Heron.MudCalendar;

public class ItemPosition<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T>  where T : CalendarItem
{
    public T Item { get; set; } = (T)Activator.CreateInstance(typeof(T))!;
    public int Position { get; set; }
    public int Total { get; set; }
    public DateOnly Date { get; set; }
    public int Top { get; set; }
    public int Left { get; set; }
    public int Height { get; set; }
    public int Width { get; set; } = 1;
    public int Bottom => Top + Height;
}