using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace Heron.MudCalendar;

public partial class MonthView : CalendarViewBase
{
    /// <summary>
    /// Classes added to main div of component.
    /// </summary>
    protected virtual string Classname =>
        new CssBuilder("mud-cal-month-view")
            .AddClass("mud-cal-month-fixed-height", Calendar.MonthCellMinHeight == 0)
            .Build();

    /// <summary>
    /// Styles added to each row of the component.
    /// </summary>
    protected virtual string RowStyle =>
        new StyleBuilder()
            .AddStyle("min-height", Calendar.MonthCellMinHeight + "px", Calendar.MonthCellMinHeight > 0)
            .AddStyle("height", $"{100 / (Cells.Count / 7)}%", Calendar.MonthCellMinHeight == 0)
            .Build();

    /// <summary>
    /// Classes added to the day number.
    /// </summary>
    /// <param name="calendarCell">The cell.</param>
    /// <returns></returns>
    protected virtual string DayClassname(CalendarCell calendarCell)
    {
        return new CssBuilder()
            .AddClass("mud-cal-month-cell-title")
            .AddClass("mud-cal-month-outside", calendarCell.Outside)
            .Build();
    }

    /// <summary>
    /// Styles added to each cell.
    /// </summary>
    /// <param name="calendarCell">The cell.</param>
    /// <returns></returns>
    protected virtual string DayStyle(CalendarCell calendarCell)
    {
        return new StyleBuilder()
            .AddStyle("border", $"1px solid var(--mud-palette-{Calendar.Color.ToDescriptionString()})",
                calendarCell.Today && Calendar.HighlightToday)
            .AddStyle("min-height", Calendar.MonthCellMinHeight + "px", Calendar.MonthCellMinHeight > 0)
            .Build();
    }

    /// <summary>
    /// Method invoked when the user clicks on the hyper link in the cell.
    /// </summary>
    /// <param name="cell">The cell that was clicked.</param>
    /// <returns></returns>
    protected virtual Task OnCellLinkClicked(CalendarCell cell)
    {
        return Calendar.CellClicked.InvokeAsync(cell.Date);
    }
    
    protected override List<CalendarCell> BuildCells()
    {
        var cells = new List<CalendarCell>();
        var monthStart = new DateTime(Calendar.CurrentDay.Year, Calendar.CurrentDay.Month, 1);
        var monthEnd = new DateTime(Calendar.CurrentDay.AddMonths(1).Year, Calendar.CurrentDay.AddMonths(1).Month, 1).AddDays(-1);

        var range = new CalendarDateRange(Calendar.CurrentDay, CalendarView.Month);
        if (range.Start == null || range.End == null) return cells;
        
        var date = range.Start.Value;
        var lastDate = range.End.Value;
        while (date <= lastDate)
        {
            var cell = BuildCell(date, monthStart, monthEnd);
            cells.Add(cell);

            // Next day
            date = date.AddDays(1);
        }

        return cells;
    }
    
    /// <summary>
    /// Builds a cell for the month view.
    /// </summary>
    /// <param name="date">The date of the cell.</param>
    /// <param name="monthStart">The first day of the month being shown.</param>
    /// <param name="monthEnd">The last day of the month being shown.</param>
    /// <returns></returns>
    protected virtual CalendarCell BuildCell(DateTime date, DateTime monthStart, DateTime monthEnd)
    {
        var cell = new CalendarCell { Date = date };
        if (date.Date == DateTime.Today) cell.Today = true;
        if (date < monthStart || date > monthEnd)
        {
            cell.Outside = true;
        }
        cell.Items = Calendar.Items.Where(i => i.Start >= date && i.Start < date.AddDays(1)).OrderBy(i => i.Start).ToList();
        return cell;
    }

    private RenderFragment<CalendarItem> CellTemplate => Calendar.MonthTemplate ?? Calendar.CellTemplate;
}