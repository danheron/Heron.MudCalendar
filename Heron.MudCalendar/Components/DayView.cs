using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;

namespace Heron.MudCalendar;

public class DayView<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T> : DayWeekViewBase<T> where T:CalendarItem
{
    protected override int DaysInView => 1;
    protected override CalendarView View => CalendarView.Day;
    protected override string HeaderClassname => "mud-cal-day-header";
    protected override string GridClassname => "mud-cal-day-grid";

    protected override List<CalendarCell<T>> BuildCells()
    {
        var cell = new CalendarCell<T> { Date = Calendar.CurrentDay.Date };
        if (Calendar.CurrentDay.Date == DateTime.Today) cell.Today = true;
        
        cell.Items = Calendar.Items.Where(i =>
                (i.Start.Date == Calendar.CurrentDay.Date) || 
                (i.Start.Date <= Calendar.CurrentDay.Date && i.End.HasValue && i.End.Value > Calendar.CurrentDay.Date))
            .OrderBy(i => i.Start)
            .ToList();
        
        return new List<CalendarCell<T>> { cell };
    }

    protected override RenderFragment<T> CellTemplate => Calendar.DayTemplate ?? Calendar.CellTemplate;
}
