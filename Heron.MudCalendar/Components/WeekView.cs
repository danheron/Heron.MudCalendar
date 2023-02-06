namespace Heron.MudCalendar;

public class WeekView : DayWeekViewBase
{
    protected override List<CalendarCell> BuildCells()
    {
        var cells = new List<CalendarCell>();
        var range = new CalendarDateRange(CurrentDay, CalendarView.Week);
        
        if (range.Start == null || range.End == null) return cells;
        
        var date = range.Start.Value;
        var lastDate = range.End.Value;
        while (date <= lastDate)
        {
            var cell = new CalendarCell { Date = date };
            if (date.Date == DateTime.Today) cell.Today = true;
            
            cell.Items = Items.Where(i => i.Start >= date && i.Start < date.AddDays(1)).ToList();
            
            cells.Add(cell);
            
            // Next day
            date = date.AddDays(1);
        }

        return cells;
    }
}