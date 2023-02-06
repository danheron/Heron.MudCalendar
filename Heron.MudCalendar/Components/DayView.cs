namespace Heron.MudCalendar;

public class DayView : DayWeekViewBase
{
    protected override List<CalendarCell> BuildCells()
    {
        var cell = new CalendarCell { Date = CurrentDay };
        if (CurrentDay.Date == DateTime.Today) cell.Today = true;
        
        cell.Items = Items.Where(i => i.Start >= CurrentDay && i.Start < CurrentDay.AddDays(1)).ToList();

        return new List<CalendarCell> { cell };
    }
}
