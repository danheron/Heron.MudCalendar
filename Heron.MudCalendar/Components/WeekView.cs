using Microsoft.AspNetCore.Components;

namespace Heron.MudCalendar;

public class WeekView : DayWeekViewBase
{
    protected override int DaysInView => 7;
    
    protected override List<CalendarCell> BuildCells()
    {
        var cells = new List<CalendarCell>();
        var range = new CalendarDateRange(Calendar.CurrentDay, CalendarView.Week);
        
        if (range.Start == null || range.End == null) return cells;
        
        var date = range.Start.Value;
        var lastDate = range.End.Value;
        while (date <= lastDate)
        {
            var cell = new CalendarCell { Date = date };
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
    
    protected override RenderFragment<CalendarItem> CellTemplate => Calendar.WeekTemplate ?? Calendar.CellTemplate;
}