using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace Heron.MudCalendar;

public partial class MonthView : CalendarViewBase
{
    /// <summary>
    /// If 0 the month view will be fixed height. If set the month view will exapnd when necessary with this being the minimum height of each cell.
    /// </summary>
    [Parameter]
    public int MinCellHeight { get; set; }
    
    /// <summary>
    /// Classes added to main div of component.
    /// </summary>
    protected virtual string Classname =>
        new CssBuilder("mud-cal-month-view")
            .AddClass("mud-cal-month-fixed-height", MinCellHeight == 0)
            .Build();

    /// <summary>
    /// Styles added to each row of the component.
    /// </summary>
    protected virtual string RowStyle =>
        new StyleBuilder()
            .AddStyle("min-height", MinCellHeight + "px", MinCellHeight > 0)
            .AddStyle("height", $"{100 / (Cells.Count / 7)}%", MinCellHeight == 0)
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
            .AddStyle("border", $"1px solid var(--mud-palette-{Color.ToDescriptionString()})",
                calendarCell.Today && HighlightToday)
            .AddStyle("min-height", MinCellHeight + "px", MinCellHeight > 0)
            .Build();
    }

    /// <summary>
    /// Method invoked when the user clicks on the hyper link in the cell.
    /// </summary>
    /// <param name="cell">The cell that was clicked.</param>
    /// <returns></returns>
    protected virtual Task OnCellLinkClicked(CalendarCell cell)
    {
        return CellClicked.InvokeAsync(cell.Date);
    }
    
    protected override List<CalendarCell> BuildCells()
    {
        var cells = new List<CalendarCell>();
        var monthStart = new DateTime(CurrentDay.Year, CurrentDay.Month, 1);
        var monthEnd = new DateTime(CurrentDay.AddMonths(1).Year, CurrentDay.AddMonths(1).Month, 1).AddDays(-1);

        var range = new CalendarDateRange(CurrentDay, CalendarView.Month);
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
        cell.Items = Items.Where(i => i.Start >= date && i.Start < date.AddDays(1)).ToList();
        return cell;
    }
}