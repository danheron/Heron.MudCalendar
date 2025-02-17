using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;

namespace Heron.MudCalendar;

public class WeekView<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T> : DayWeekViewBase<T> where T:CalendarItem
{
    protected override int DaysInView => 7;
    
    protected override List<CalendarCell<T>> BuildCells()
    {
        var cells = new List<CalendarCell<T>>();
        var range = new CalendarDateRange(Calendar.CurrentDay.Date, CalendarView.Week, Calendar.FirstDayOfWeek);
        
        if (range.Start == null || range.End == null) return cells;
        
        var date = range.Start.Value;
        var lastDate = range.End.Value;
        while (date <= lastDate)
        {
            var cell = new CalendarCell<T> { Date = date };
            if (date.Date == DateTime.Today) cell.Today = true;
            
            cell.Items = Calendar.Items.Where(i =>
                    (i.Start.Date == date) || 
                    (i.Start.Date <= date && i.End.HasValue && i.End.Value > date))
                .OrderBy(i => i.Start)
                .ToList();
            
            cells.Add(cell);
            
            // Next day
            date = date.AddDays(1);
        }

        return cells;
    }
    
    protected override RenderFragment<T> CellTemplate => Calendar.WeekTemplate ?? Calendar.CellTemplate;
}