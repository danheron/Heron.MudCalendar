using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;

namespace Heron.MudCalendar;

public partial class MonthView : CalendarViewBase
{
    private MudDropContainer<CalendarItem>? _dropContainer;
    
    /// <summary>
    /// Classes added to main div of component.
    /// </summary>
    protected virtual string Classname =>
        new CssBuilder("mud-cal-month-table-body")
            .AddClass("mud-cal-month-fixed-height", Calendar.MonthCellMinHeight == 0)
            .Build();

    protected virtual string GridStyle =>
        new StyleBuilder()
            .AddStyle("grid-template-rows", $"repeat({Cells.Count / 7}, {100 / (Cells.Count / 7)}%)",
                Calendar.MonthCellMinHeight == 0)
            .Build();

    /// <summary>
    /// Styles added to each row of the component.
    /// </summary>
    protected virtual string RowStyle =>
        new StyleBuilder()
            .AddStyle("min-height", Calendar.MonthCellMinHeight + "px", Calendar.MonthCellMinHeight > 0)
            .Build();

    protected virtual string CellStyle =>
        new StyleBuilder()
            .AddStyle("overflow-y", "scroll", Calendar.MonthCellMinHeight == 0)
            .Build();

    /// <summary>
    /// Classes added to each month cell.
    /// </summary>
    protected virtual string CellClassname =>
        new CssBuilder()
            .AddClass("mud-cal-month-cell")
            .AddClass("mud-cal-month-link", Calendar.CellClicked.HasDelegate)
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
    /// Method invoked when the user clicks on the hyperlink in the cell.
    /// </summary>
    /// <param name="cell">The cell that was clicked.</param>
    /// <returns></returns>
    protected virtual async Task OnCellLinkClicked(CalendarCell cell)
    {
        if (Calendar.CellClicked.HasDelegate)
        {
            await Calendar.CellClicked.InvokeAsync(cell.Date);
        }
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        
        // Ensure that the order of items is correct
        _dropContainer?.Refresh();
    }
    
    /// <summary>
    /// Method invoked when the user clicks on the calendar item.
    /// </summary>
    /// <param name="item">The calendar item that was clicked.</param>
    /// <returns></returns>
    protected virtual Task OnItemClicked(CalendarItem item)
    {
        return Calendar.ItemClicked.InvokeAsync(item);
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
        cell.Items = Calendar.Items.Where(i =>
                (i.Start.Date < date && i.End.HasValue && i.End.Value.Date >= date))
            .OrderBy(i => i.Start)
            .ToList();

        return cell;
    }

    private async Task ItemDropped(MudItemDropInfo<CalendarItem> dropItem)
    {
        if (dropItem.Item == null) return;
        var item = dropItem.Item;
        
        // Update start and end time
        var duration = item.End?.Subtract(item.Start) ?? TimeSpan.Zero;
        item.Start = DateTime.Parse(dropItem.DropzoneIdentifier).Add(item.Start.TimeOfDay);
        if (item.End.HasValue)
        {
            item.End = item.Start.Add(duration);
        }
        
        Calendar.Refresh();

        await Calendar.ItemChanged.InvokeAsync(item);
    }

    private RenderFragment<CalendarItem> CellTemplate => Calendar.MonthTemplate ?? Calendar.CellTemplate;
}