using Heron.MudCalendar.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace Heron.MudCalendar;

public abstract partial class DayWeekViewBase : CalendarViewBase, IAsyncDisposable
{
    private ElementReference _scrollDiv;
    private JsService? _jsService;

    private const int MinutesInDay = 24 * 60;
    private int PixelsInCell => Calendar.DayCellHeight;

    private int CellsInDay => MinutesInDay / (int)Calendar.DayTimeInterval;
    private int PixelsInDay => CellsInDay * PixelsInCell;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            await ScrollToDay();
        }
    }

    /// <summary>
    /// Styles added to each day.
    /// </summary>
    /// <param name="calendarCell">The cell.</param>
    /// <returns></returns>
    protected virtual string DayStyle(CalendarCell calendarCell)
    {
        return new StyleBuilder()
            .AddStyle("border", $"1px solid var(--mud-palette-{Calendar.Color.ToDescriptionString()})",
                calendarCell.Today && Calendar.HighlightToday)
            .Build();
    }

    protected virtual string EventStyle(ItemPosition position)
    {
        return new StyleBuilder()
            .AddStyle("position", "absolute")
            .AddStyle("top", $"{CalcTop(position)}px")
            .AddStyle("height", $"{CalcHeight(position)}px")
            .AddStyle("overflow", "hidden")
            .AddStyle("left",
                (((position.Position / (double)position.Total) - (1.0 / position.Total)) * 100).ToInvariantString() +
                "%")
            .AddStyle("width", (100 / position.Total) + "%")
            .Build();
    }

    protected virtual string CellHeightStyle()
    {
        return new StyleBuilder()
            .AddStyle("height", $"{Calendar.DayCellHeight}px")
            .Build();
    }

    protected virtual string TimelineStyle()
    {
        return new StyleBuilder()
            .AddStyle("position", "absolute")
            .AddStyle("width", "100%")
            .AddStyle("border", "1px solid var(--mud-palette-grey-default)")
            .AddStyle("top",
                $"{(int)((DateTime.Now.Subtract(DateTime.Today).TotalMinutes / MinutesInDay) * PixelsInDay)}px")
            .Build();
    }

    /// <summary>
    /// Method invoked when the user clicks on the hyper link in the cell.
    /// </summary>
    /// <param name="cell">The cell that was clicked.</param>
    /// <param name="row">The row that was clicked.</param>
    /// <returns></returns>
    protected virtual Task OnCellLinkClicked(CalendarCell cell, int row)
    {
        var date = cell.Date.AddHours(row / (60.0 / (int)Calendar.DayTimeInterval));
        return Calendar.CellClicked.InvokeAsync(date);
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

    private int CalcTop(ItemPosition position)
    {
        double minutes = 0;
        if (DateOnly.FromDateTime(position.Item.Start.Date) == position.Date)
        {
            minutes = position.Item.Start.Hour * 60 + position.Item.Start.Minute;
        }

        var percent = minutes / MinutesInDay;
        var top = PixelsInDay * percent;

        return (int)top;
    }

    private int CalcHeight(ItemPosition position)
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

        return (int)height;
    }

    private async Task ScrollToDay()
    {
        var startMinutes = (Calendar.DayStartTime.Hour * 60) + Calendar.DayStartTime.Minute;
        var percent = (double)startMinutes / MinutesInDay;
        var scrollTo = PixelsInDay * percent;

        _jsService ??= new JsService(JsRuntime);
        await _jsService.Scroll(_scrollDiv, (int)scrollTo);
    }

    protected virtual RenderFragment<CalendarItem> CellTemplate => Calendar.CellTemplate;

    private static IEnumerable<ItemPosition> CalcPositions(IEnumerable<CalendarItem> items, DateOnly date)
    {
        var positions = new List<ItemPosition>();
        var overlaps = new List<ItemPosition>();
        foreach (var item in items)
        {
            overlaps.RemoveAll(o => (o.Item.End ?? o.Item.Start.AddHours(1)) <= item.Start);

            // Create new position object
            var position = new ItemPosition { Item = item, Position = 0, Total = overlaps.Count + 1, Date = date };
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
            var max = positions.Where(p => p.Item.Start < (position.Item.End ?? position.Item.Start.AddHours(1))
                                           && (p.Item.End ?? p.Item.Start.AddHours(1)) > position.Item.Start)
                .Max(p => p.Total);
            position.Total = max;
        }

        return positions;
    }

    protected class ItemPosition
    {
        public CalendarItem Item { get; set; } = new();
        public int Position { get; set; }
        public int Total { get; set; }
        public DateOnly Date { get; set; }
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        if (_jsService != null)
        {
            await _jsService.DisposeAsync();
        }
    }
}