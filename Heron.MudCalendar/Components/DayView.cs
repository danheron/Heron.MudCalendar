using Microsoft.AspNetCore.Components;

namespace Heron.MudCalendar;

public class DayView : DayWeekViewBase
{
    protected override int DaysInView => 1;

    protected override List<CalendarCell> BuildCells()
    {
        var cell = new CalendarCell { Date = Calendar.CurrentDay };
        if (Calendar.CurrentDay.Date == DateTime.Today) cell.Today = true;
        
        cell.Items = Calendar.Items.Where(i =>
                (i.Start.Date == Calendar.CurrentDay) || 
                (i.Start.Date <= Calendar.CurrentDay && i.End.HasValue && i.End.Value > Calendar.CurrentDay))
            .OrderBy(i => i.Start)
            .ToList();
        
        return new List<CalendarCell> { cell };
    }

    protected override RenderFragment<CalendarItem> CellTemplate => Calendar.DayTemplate ?? Calendar.CellTemplate;
}
