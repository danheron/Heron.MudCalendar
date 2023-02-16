using Microsoft.AspNetCore.Components;

namespace Heron.MudCalendar;

public class DayView : DayWeekViewBase
{
    protected override List<CalendarCell> BuildCells()
    {
        var cell = new CalendarCell { Date = Calendar.CurrentDay };
        if (Calendar.CurrentDay.Date == DateTime.Today) cell.Today = true;
        
        cell.Items = Calendar.Items.Where(i => i.Start >= Calendar.CurrentDay && i.Start < Calendar.CurrentDay.AddDays(1)).ToList();

        return new List<CalendarCell> { cell };
    }

    protected override RenderFragment<CalendarItem> CellTemplate => Calendar.DayTemplate ?? Calendar.CellTemplate;
}
