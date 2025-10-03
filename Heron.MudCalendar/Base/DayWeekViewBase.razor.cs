using Heron.MudCalendar.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Utilities;
using System.Diagnostics.CodeAnalysis;
using static System.Runtime.InteropServices.JavaScript.JSType;
using EnumExtensions = Heron.MudCalendar.Extensions.EnumExtensions;

namespace Heron.MudCalendar;

public abstract partial class DayWeekViewBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T> : CalendarViewBase<T>, IDisposable where T:CalendarItem
{
    private ElementReference _scrollDiv;
    private JsService? _jsService;

    private const int MinutesInDay = 24 * 60;
    private int PixelsInCell => Calendar.DayCellHeight;

    private int CellsInDay => MinutesInDay / (int)Calendar.DayTimeInterval;
    private int PixelsInDay => CellsInDay * PixelsInCell;

    protected virtual int DaysInView => 7;
    protected virtual CalendarView View => CalendarView.Week;
    protected virtual string HeaderClassname => string.Empty;
    protected virtual string GridClassname => string.Empty;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            if (Calendar.CellRangeSelected.HasDelegate)
            {
                _jsService ??= new JsService(JsRuntime);
                await _jsService.AddMultiSelect(CellsInDay, Calendar._id);
                _jsService.OnCellsSelected += OnCellRangeSelected;
            }

            await ScrollToDay();
        }
    }

    

    protected virtual bool IsSelectable(CalendarCell<T> cell, int row)
    {
        var date = cell.Date.AddMinutes(row * (int)Calendar.DayTimeInterval);
        return (Calendar.IsDateTimeDisabledFunc == null || !Calendar.IsDateTimeDisabledFunc(date, View));
    }

    /// <summary>
    /// Styles the header grid
    /// </summary>
    protected virtual string HeaderClass =>
        new CssBuilder("mud-cal-grid")
            .AddClass("mud-cal-grid-header")
            .AddClass(HeaderClassname)
            .Build();

    /// <summary>
    /// Styles the main grid
    /// </summary>
    protected virtual string GridClass =>
        new CssBuilder("mud-cal-grid")
            .AddClass(GridClassname)
            .Build();

    /// <summary>
    /// Styles added to each day.
    /// </summary>
    /// <param name="calendarCell">The cell.</param>
    /// <param name="row">The current row in the table being rendered.</param>
    /// <returns></returns>
    protected virtual string DayStyle(CalendarCell<T> calendarCell, int row)
    {
        return new StyleBuilder()
            .AddStyle("border-left",
                $"1px solid var(--mud-palette-{EnumExtensions.ToDescriptionString(Calendar.Color)})",
                calendarCell.Today && Calendar.HighlightToday)
            .AddStyle("border-right",
                $"1px solid var(--mud-palette-{EnumExtensions.ToDescriptionString(Calendar.Color)})",
                calendarCell.Today && Calendar.HighlightToday)
            .AddStyle("border-top",
                $"1px solid var(--mud-palette-{EnumExtensions.ToDescriptionString(Calendar.Color)})",
                row == 0 && calendarCell.Today && Calendar.HighlightToday)
            .AddStyle("border-bottom",
                $"1px solid var(--mud-palette-{EnumExtensions.ToDescriptionString(Calendar.Color)})",
                row + 1 == CellsInDay && calendarCell.Today && Calendar.HighlightToday)
            .Build();
    }

    /// <summary>
    /// Styles the position and height of the div containing an item.
    /// </summary>
    /// <param name="position">Position information for the div.</param>
    /// <returns></returns>
    protected virtual string EventStyle(ItemPosition<T> position)
    {
        var maxWidth = Calendar.EnableParallelItemClick ? 95.0 : 100.0;
        return new StyleBuilder()
            .AddStyle("position", "absolute")
            .AddStyle("top", $"{position.Top}px")
            .AddStyle("height", $"{position.Height}px")
            .AddStyle("left",
                (((position.Position / (double)position.Total) - (1.0 / position.Total)) * maxWidth).ToInvariantString() +
                "%")
            .AddStyle("width", (maxWidth / position.Total).ToInvariantString() + "%")
            .Build();
    }

    /// <summary>
    /// Styles for the cell where the time is displayed..
    /// </summary>
    /// <param name="row">The row being styled.</param>
    /// <returns></returns>
    protected virtual string TimeCellClassname(int row)
    {
        return new CssBuilder()
            .AddClass("mud-cal-week-cell", IsHourCell(row))
            .AddClass("mud-cal-time-cell", IsHourCell(row))
            .AddClass("mud-cal-week-not-today")
            .Build();
    }

    /// <summary>
    /// Styles for each cell in the view.
    /// </summary>
    /// <param name="cell">The cell being styled.</param>
    /// <param name="row">The row being styled.</param>
    /// <returns></returns>
    protected virtual string DayCellClassname(CalendarCell<T> cell, int row)
    {
        return new CssBuilder()
            .AddClass("mud-cal-week-cell")
            .AddClass(Calendar.AdditionalDateTimeClassesFunc?.Invoke(cell.Date.AddMinutes(row * (int)Calendar.DayTimeInterval), View))
            .AddClass("mud-cal-week-cell-half", !IsHourCell(row))
            .AddClass("mud-cal-week-not-today", !cell.Today || !Calendar.HighlightToday)
            .Build();
    }

    /// <summary>
    /// Styles that set the height of each cell.
    /// </summary>
    /// <returns></returns>
    protected virtual string CellHeightStyle()
    {
        return new StyleBuilder()
            .AddStyle("height", $"{Calendar.DayCellHeight}px")
            .Build();
    }

    /// <summary>
    /// The style of the line showing the current time.
    /// </summary>
    /// <returns></returns>
    protected virtual string TimelineStyle()
    {
        return new StyleBuilder()
            .AddStyle("position", "absolute")
            .AddStyle("width", "100%")
            .AddStyle("border", "1px solid var(--mud-palette-gray-default)")
            .AddStyle("top", $"{TimelinePosition().ToInvariantString()}px")
            .Build();
    }

    /// <summary>
    /// Method invoked when the user clicks on the hyper link in the cell.
    /// </summary>
    /// <param name="cell">The cell that was clicked.</param>
    /// <param name="row">The row that was clicked.</param>
    /// <returns></returns>
    protected virtual async Task OnCellLinkClicked(CalendarCell<T> cell, int row)
    {
        if (AllowCellLinkClick(cell, row))
        {
            var date = cell.Date.AddMinutes(row * (int)Calendar.DayTimeInterval);
            await Calendar.CellClicked.InvokeAsync(date);
        }
    }

    /// <summary>
    /// Determines if the click event is allowed on a cell.
    /// </summary>
    /// <param name="cell">The cell that was clicked.</param>
    /// <param name="row">The row that was clicked.</param>
    /// <returns><c>true</c> if the cell can be clicked.</returns>
    protected virtual bool AllowCellLinkClick(CalendarCell<T> cell, int row)
    {
        var date = cell.Date.AddMinutes(row * (int)Calendar.DayTimeInterval);
        return (Calendar.CellClicked.HasDelegate || Calendar.CellRangeSelected.HasDelegate) && (Calendar.IsDateTimeDisabledFunc == null || !Calendar.IsDateTimeDisabledFunc(date, View));
    }

    /// <summary>
    /// Called when the user selects a range of cells in the calendar.
    /// </summary>
    /// <param name="sender">The source object that triggered the selection event.</param>
    /// <param name="selectedCells">A collection of selected cells, each represented as a tuple containing the date and row index.</param>
    /// <returns></returns>
    protected virtual async void OnCellRangeSelected(object? sender, IEnumerable<(DateTime date, int row)> selectedCells)
    {
        var start = selectedCells.First();
        var end = selectedCells.Last();
        var dateStart = start.date.AddMinutes(start.row * (int)Calendar.DayTimeInterval);
        var dateEnd = end.date.AddMinutes((end.row + 1) * (int)Calendar.DayTimeInterval);
        await Calendar.CellRangeSelected.InvokeAsync(new DateRange(dateStart, dateEnd));
    }

    /// <summary>
    /// Method invoked when the user clicks on the calendar item.
    /// </summary>
    /// <param name="item">The calendar item that was clicked.</param>
    /// <returns></returns>
    protected virtual Task OnItemClicked(T item)
    {
        return Calendar.ItemClicked.InvokeAsync(item);
    }

    /// <summary>
    /// Creates a string with the time to be displayed.
    /// </summary>
    /// <param name="row">The current row in the table.</param>
    /// <returns></returns>
    protected virtual string DrawTime(int row)
    {
        var hour = row / (60.0 / (double)Calendar.DayTimeInterval);
        var timeSpan = TimeSpan.FromHours(hour);
        var time = TimeOnly.FromTimeSpan(timeSpan);

        return Calendar.Use24HourClock ? time.ToString("HH:mm") : time.ToString("h tt");
    }

    /// <summary>
    /// Calculates the row of the timeline for the current time.
    /// </summary>
    /// <returns>The row of the timeline.</returns>
    protected int TimelineRow()
    {
        var minutes = DateTime.Now.Subtract(DateTime.Today).TotalMinutes;
        var row = (int)Math.Floor(minutes / (int)Calendar.DayTimeInterval);

        return row;
    }
    
    /// <summary>
    /// Adjusts the end time of a calendar item based on the specified number of intervals.
    /// </summary>
    /// <param name="item">The calendar item whose height is changing.</param>
    /// <param name="intervals">The number of intervals by which the item's end time should be extended.</param>
    /// <returns>A task representing the asynchronous operation of invoking the item changed event.</returns>
    protected Task ItemHeightChanged(T item, int intervals)
    {
        // Calculate end time from height
        var minutes = intervals * (int)Calendar.DayTimeInterval;
        item.End = item.Start.AddMinutes(minutes);
        
        return Calendar.ItemChanged.InvokeAsync(item);
    }

    private double TimelinePosition()
    {
        var minutes = DateTime.Now.Subtract(DateTime.Today).TotalMinutes -
                      (TimelineRow() * (int)Calendar.DayTimeInterval);
        var position = (minutes / (int)Calendar.DayTimeInterval) * Calendar.DayCellHeight;

        return position;
    }

    private int CalcTop(ItemPosition<T> position)
    {
        double minutes = 0;
        if (DateOnly.FromDateTime(position.Item.Start.Date) == position.Date)
        {
            minutes = position.Item.Start.Hour * 60 + position.Item.Start.Minute;
        }

        var percent = minutes / MinutesInDay;
        var top = PixelsInDay * percent;

        return (int)Math.Round(top);
    }

    private int CalcHeight(ItemPosition<T> position)
    {
        double start = 0;
        if (DateOnly.FromDateTime(position.Item.Start.Date) == position.Date)
        {
            start = position.Item.Start.Hour * 60 + position.Item.Start.Minute;
        }

        var end = start + 60;
        if (position.Item.End.HasValue)
        {
            end = MinutesInDay;
            if (DateOnly.FromDateTime(position.Item.End.Value.Date) == position.Date)
            {
                end = position.Item.End.Value.Hour * 60 + position.Item.End.Value.Minute;
            }
        }

        if (end > MinutesInDay) end = MinutesInDay;
        var minutes = end - start;
        var percent = minutes / MinutesInDay;
        var height = PixelsInDay * percent;

        if (height < Calendar.DayItemMinHeight)
        {
            height = Calendar.DayItemMinHeight;
        }

        return (int)Math.Round(height);
    }

    private async Task ScrollToDay()
    {
        var startMinutes = (Calendar.DayStartTime.Hour * 60) + Calendar.DayStartTime.Minute;
        var percent = (double)startMinutes / MinutesInDay;
        var scrollTo = PixelsInDay * percent;

        _jsService ??= new JsService(JsRuntime);
        await _jsService.Scroll(_scrollDiv, (int)scrollTo);
    }

    protected virtual RenderFragment<T> CellTemplate => Calendar.CellTemplate;

    private IEnumerable<ItemPosition<T>> CalcPositions(IEnumerable<T> items, DateOnly date)
    {
        var positions = new List<ItemPosition<T>>();
        var overlaps = new List<ItemPosition<T>>();
        foreach (var item in items)
        {
            // Check that the end date is valid
            if (item.End.HasValue && item.End <= item.Start)
            {
                throw new ApplicationException("End date of calendar item must be after start date");
            }

            // Create new position object
            var position = new ItemPosition<T> { Item = item, Position = 0, Total = overlaps.Count + 1, Date = date };
            position.Top = CalcTop(position);
            position.Height = CalcHeight(position);
            if (position.Bottom > PixelsInDay)
            {
                position.Height = PixelsInDay - position.Top;
            }

            // Remove overlaps that are not relevant
            overlaps.RemoveAll(o => o.Bottom <= position.Top);
            positions.Add(position);

            // Calculate the position
            for (var i = 1; i <= overlaps.Count; i++)
            {
                if (overlaps.Any(o => o.Position == i) == false)
                {
                    position.Position = i;
                }
            }

            if (position.Position == 0) position.Position = overlaps.Count + 1;

            overlaps.Add(position);
            var maxPosition = overlaps.Max(o => o.Position);
            foreach (var overlap in overlaps)
            {
                overlap.Total = maxPosition;
            }
        }

        // Calculate the total overlapping events
        foreach (var position in positions)
        {
            var max = positions.Where(p => p.Top < position.Bottom && p.Bottom > position.Top).Max(p => p.Total);

            if (max > position.Total)
            {
                position.Total = max;

                // Need to update overlapping events
                var overlappingPositions = positions.Where(p => p.Top < position.Bottom && p.Bottom > position.Top);
                foreach (var overlappedPosition in overlappingPositions)
                {
                    if (overlappedPosition.Total < max)
                    {
                        overlappedPosition.Total = max;
                    }
                }
            }
        }
        
        return positions;
    }

    private async Task ItemDropped(MudItemDropInfo<T> dropItem)
    {
        if (dropItem.Item == null) return;
        var item = dropItem.Item;
        var duration = item.End?.Subtract(item.Start) ?? TimeSpan.Zero;

        var ids = dropItem.DropzoneIdentifier.Split("_");
        if (DateTime.TryParse(ids[0], out var date))
        {
            var cell = int.Parse(ids[1]);
            var minutes = ((double)cell / CellsInDay) * MinutesInDay;
            date = date.AddMinutes(minutes);
        }
        else
        {
            var calendarItem = Cells.SelectMany(c => c.Items).FirstOrDefault(it => it.Id == ids[0]);
            if(calendarItem == null) return;
            date = calendarItem.Start;
        }

        // Update start and end time
        item.Start = date;
        if (item.End.HasValue)
        {
            item.End = item.Start.Add(duration);
        }

        Calendar.Refresh();

        await Calendar.ItemChanged.InvokeAsync(item);
    }

    private bool IsHourCell(int row)
    {
        return (int)Calendar.DayTimeInterval >= 60 || row % (60 / (int)Calendar.DayTimeInterval) == 0;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        _jsService?.Dispose();
    }
}