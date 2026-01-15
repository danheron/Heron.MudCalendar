using Heron.MudCalendar.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Utilities;
using System.Diagnostics.CodeAnalysis;
using EnumExtensions = Heron.MudCalendar.Extensions.EnumExtensions;

namespace Heron.MudCalendar;

public abstract partial class DayWeekViewBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T> : CalendarViewBase<T>, IDisposable where T : CalendarItem
{
    private ElementReference _scrollDiv;
    private JsService? _jsService;

    private int PixelsInCell => Calendar.DayCellHeight;

    private int CellsInDay => MinutesInDay / (int)Calendar.DayTimeInterval;
    private int PixelsInDay => CellsInDay * PixelsInCell;

    protected virtual int DaysInView => 7;
    protected virtual CalendarView View => CalendarView.Week;
    protected virtual string HeaderClassname => string.Empty;
    protected virtual string GridClassname => string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Calendar.ScrollToTimeEvent += async (time) => await ScrollToDay(time);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            if (Calendar.CellRangeSelected.HasDelegate)
            {
                _jsService ??= new JsService(JsRuntime);
                await _jsService.AddMultiSelect(CellsInDay, Calendar.Id);
                _jsService.OnCellsSelected += OnCellRangeSelected;
            }

            await ScrollToDay();
        }
    }

    protected virtual bool IsSelectable(CalendarCell<T> cell, int row)
    {
        var date = cell.Date.AddMinutes((row + InvisibleRows) * (int)Calendar.DayTimeInterval);
        return (Calendar.IsDateTimeDisabledFunc == null || !Calendar.IsDateTimeDisabledFunc(date, View));
    }

    protected int MinutesInDay => (int)Math.Round((LastTime - FirstTime).TotalMinutes);
    protected int InvisibleRows => (int)(FirstTime.ToTimeSpan().TotalMinutes / (int)Calendar.DayTimeInterval);
    protected int InvisibleMinutes => (int)FirstTime.ToTimeSpan().TotalMinutes;
    
    private TimeOnly FirstTime => RoundTimeToInterval(Calendar.MinVisibleTime ?? new TimeOnly());
    private TimeOnly LastTime => RoundTimeToInterval(Calendar.MaxVisibleTime ?? TimeOnly.MaxValue);

    private TimeOnly RoundTimeToInterval(TimeOnly time)
    {
        var minutes = time.ToTimeSpan().TotalMinutes;
        var closestMinute = Math.Round(minutes / (int)Calendar.DayTimeInterval) * (int)Calendar.DayTimeInterval;
        return closestMinute >= 1440 ? TimeOnly.MaxValue : TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(closestMinute));
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
    /// Styles for the cell where the time is displayed.
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
            .AddClass(Calendar.AdditionalDateTimeClassesFunc?.Invoke(cell.Date.AddMinutes((row + InvisibleRows) * (int)Calendar.DayTimeInterval), View))
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
    /// Method invoked when the user clicks on the hyperlink in the cell.
    /// </summary>
    /// <param name="cell">The cell that was clicked.</param>
    /// <param name="row">The row that was clicked.</param>
    /// <returns></returns>
    protected virtual async Task OnCellLinkClicked(CalendarCell<T> cell, int row)
    {
        if (AllowCellLinkClick(cell, row))
        {
            var date = cell.Date.AddMinutes((row + InvisibleRows) * (int)Calendar.DayTimeInterval);
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
        var date = cell.Date.AddMinutes((row + InvisibleRows) * (int)Calendar.DayTimeInterval);
        return (Calendar.CellClicked.HasDelegate || Calendar.CellRangeSelected.HasDelegate) && (Calendar.IsDateTimeDisabledFunc == null || !Calendar.IsDateTimeDisabledFunc(date, View));
    }

    /// <summary>
    /// Called when the user selects a range of cells in the calendar.
    /// </summary>
    /// <param name="sender">The source object that triggered the selection event.</param>
    /// <param name="selectedCells">A collection of selected cells, each represented as a tuple containing the date and row index.</param>
    /// <returns></returns>
    protected virtual async Task OnCellRangeSelected(object? sender, IEnumerable<(DateTime date, int row)> selectedCells)
    {
        var selectedCellsList = selectedCells.ToList();
        var start = selectedCellsList.First();
        var end = selectedCellsList.Last();
        var dateStart = start.date.AddMinutes(start.row * (int)Calendar.DayTimeInterval);
        var dateEnd = end.date.AddMinutes((end.row + 1) * (int)Calendar.DayTimeInterval);
        await Calendar.CellRangeSelected.InvokeAsync(new DateRange(dateStart, dateEnd));
    }

    /// <summary>
    /// Method invoked when the user right-clicks on the hyperlink in the cell.
    /// </summary>
    /// <param name="mouseEventArgs">The mouse event arguments.</param>
    /// <param name="cell">The cell that was clicked.</param>
    /// <param name="row">The row that was clicked.</param>
    /// <returns></returns>
    protected virtual async Task OnCellLinkContextMenuClicked(MouseEventArgs mouseEventArgs, CalendarCell<T> cell, int row)
    {
        if (AllowCellLinkContextMenuClick(cell, row))
        {
            var date = cell.Date.AddMinutes((row + InvisibleRows) * (int)Calendar.DayTimeInterval);
            await Calendar.CellContextMenuClicked.InvokeAsync(new CalendarClickEventArgs(mouseEventArgs,date));
        }
    }

    /// <summary>
    /// Determines if the context menu event is allowed on a cell.
    /// </summary>
    /// <param name="cell">The cell that was clicked.</param>
    /// <param name="row">The row that was clicked.</param>
    /// <returns><c>true</c> if the cell can be clicked.</returns>
    protected virtual bool AllowCellLinkContextMenuClick(CalendarCell<T> cell, int row)
    {
        var date = cell.Date.AddMinutes((row + InvisibleRows) * (int)Calendar.DayTimeInterval);
        return Calendar.CellContextMenuClicked.HasDelegate && (Calendar.IsDateTimeDisabledFunc == null || !Calendar.IsDateTimeDisabledFunc(date, View));
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
    /// Method invoked when the user right-clicks on the calendar item.
    /// </summary>
    /// <param name="mouseEventArgs">The mouse event args.</param>
    /// <param name="item">The calendar item that was clicked.</param>
    /// <returns></returns>
    protected virtual async Task OnItemContextMenuClicked(MouseEventArgs mouseEventArgs, T item)
    {
        if (AllowItemContextMenuClick(item))
        {
            await Calendar.ItemContextMenuClicked.InvokeAsync(new CalendarItemClickEventArgs<T>(mouseEventArgs, item));
        }
    }

    /// <summary>
    /// Determines if the right-click event is allowed on an item.
    /// </summary>
    /// <param name="item">The calendar item that was clicked.</param>
    /// <returns><c>true</c> if the item can be right-clicked.</returns>
    protected virtual bool AllowItemContextMenuClick(T item)
    {
        return Calendar.ItemContextMenuClicked.HasDelegate;
    }

    /// <summary>
    /// Creates a string with the time to be displayed.
    /// </summary>
    /// <param name="row">The current row in the table.</param>
    /// <returns></returns>
    protected virtual string DrawTime(int row)
    {
        var hour = (row + InvisibleRows) / (60.0 / (double)Calendar.DayTimeInterval);
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
        return (int)Math.Floor(minutes / (int)Calendar.DayTimeInterval) - InvisibleRows;
    }
    
    /// <summary>
    /// Adjusts the end time of a calendar item based on the specified number of intervals.
    /// </summary>
    /// <param name="item">The calendar item whose height is changing.</param>
    /// <param name="intervals">The number of intervals by which the item's end time should be extended.</param>
    /// <returns>A task representing the asynchronous operation of invoking the item changed event.</returns>
    protected Task ItemHeightChanged(T item, int intervals)
    {
        var dates = (item.Start, item.End);
        
        // Calculate end time from height
        var minutes = intervals * (int)Calendar.DayTimeInterval;
        item.End = item.Start.AddMinutes(minutes);

        return dates != (item.Start, item.End) ? Calendar.ItemChanged.InvokeAsync(item) : Task.CompletedTask;
    }

    private double TimelinePosition()
    {
        var minutes = DateTime.Now.Subtract(DateTime.Today).TotalMinutes -
                      (TimelineRow() + InvisibleRows) * (int)Calendar.DayTimeInterval;
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

        var percent = (minutes - InvisibleMinutes) / MinutesInDay;
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
        start -= InvisibleMinutes;

        var end = start + 60;
        if (position.Item.End.HasValue)
        {
            end = MinutesInDay;
            if (DateOnly.FromDateTime(position.Item.End.Value.Date) == position.Date)
            {
                end = position.Item.End.Value.Hour * 60 + position.Item.End.Value.Minute;
            }
        }
        end -= InvisibleMinutes;

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

    private async Task ScrollToDay(TimeOnly? time = null)
    {
        int startMinutes;
        if (time != null)
        {
            startMinutes = (time.Value.Hour * 60) + time.Value.Minute;
        }
        else if (Calendar.AutoScrollToCurrentTime)
        {
            startMinutes = (DateTime.Now.Hour * 60) + DateTime.Now.Minute;
        }
        else
        {
            startMinutes = (Calendar.DayStartTime.Hour * 60) + Calendar.DayStartTime.Minute;
        }
        var percent = (double)(startMinutes - InvisibleMinutes) / MinutesInDay;
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

            // Create a new position object
            var position = new ItemPosition<T> { Item = item, Position = 0, Total = overlaps.Count + 1, Date = date };
            position.Top = CalcTop(position);
            position.Height = CalcHeight(position);
            if (position.Bottom > PixelsInDay)
            {
                position.Height = PixelsInDay - position.Top;
            }
            
            // Don't attempt to render items that won't be visible
            if (position.Top < 0 || position.Height < 1) continue;

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
        var dates = (item.Start, item.End);
        var duration = item.End?.Subtract(item.Start) ?? TimeSpan.Zero;

        var ids = dropItem.DropzoneIdentifier.Split("_");
        if (DateTime.TryParse(ids[0], out var date))
        {
            var cell = int.Parse(ids[1]);
            var minutes = (double)cell / CellsInDay * MinutesInDay + InvisibleMinutes;
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

        if (dates != (item.Start, item.End))
        {
            await Calendar.ItemChanged.InvokeAsync(item);
        }
    }

    private bool IsHourCell(int row)
    {
        return (int)Calendar.DayTimeInterval >= 60 || (row + InvisibleRows) % (60 / (int)Calendar.DayTimeInterval) == 0;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        
        if (_jsService != null)
        {
            _jsService.OnCellsSelected -= OnCellRangeSelected;
        }

        _jsService?.Dispose();
    }
}
