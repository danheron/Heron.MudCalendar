using Heron.MudCalendar.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace Heron.MudCalendar;

public abstract partial class DayWeekViewBase : CalendarViewBase, IAsyncDisposable
{
    private ElementReference _scrollDiv;
    private JsService? _jsService;
    
    private const int DayStartTime = 8;
    
    private const int MinutesInDay = 24 * 60;
    private const int PixelsInDay = 48 * 36;

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
            .AddStyle("border", $"1px solid var(--mud-palette-{Calendar.Color.ToDescriptionString()})", calendarCell.Today && Calendar.HighlightToday)
            .Build();
    }

    private string EventStyle(ItemPosition position)
    {
        return new StyleBuilder()
            .AddStyle("position", "absolute")
            .AddStyle("top", $"{CalcTop(position.Item)}px")
            .AddStyle("height", $"{CalcHeight(position.Item)}px")
            .AddStyle("overflow", "hidden")
            .AddStyle("left", ((position.Position / (double)position.Total) - (1.0 / position.Total)) * 100 + "%")
            .AddStyle("width", (100 / position.Total) + "%" )
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
        var date = cell.Date.AddHours(row / 2.0);
        return Calendar.CellClicked.InvokeAsync(date);
    }

    private static int CalcTop(CalendarItem item)
    {
        double minutes = item.Start.Hour * 60 + item.Start.Minute;
        var percent = minutes / MinutesInDay;
        var top = PixelsInDay * percent;

        return (int)top;
    }

    private static int CalcHeight(CalendarItem item)
    {
        double minutes = 60;
        if (item.End.HasValue)
        {
            var start = item.Start.Hour * 60 + item.Start.Minute;
            var end = item.End.Value.Hour * 60 + item.End.Value.Minute;
            minutes = end - start;
        }
        var percent = minutes / MinutesInDay;
        var height = PixelsInDay * percent;

        return (int)height;
    }

    private async Task ScrollToDay()
    {
        const double percent = (double)(DayStartTime * 60) / MinutesInDay;
        const double scrollTo = PixelsInDay * percent;

        _jsService ??= new JsService(JsRuntime);
        await _jsService.Scroll(_scrollDiv, (int)scrollTo);
    }

    protected virtual RenderFragment<CalendarItem> CellTemplate => Calendar.CellTemplate;

    private static IEnumerable<ItemPosition> CalcPositions(IEnumerable<CalendarItem> items)
    {
        var positions = new List<ItemPosition>();
        var overlaps = new List<ItemPosition>();
        foreach (var item in items)
        {
            overlaps.RemoveAll(o => (o.Item.End ?? o.Item.Start.AddHours(0.5)) <= item.Start);

            // Create new position object
            var position = new ItemPosition { Item = item, Position = 0, Total = overlaps.Count + 1 };
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
            var max = positions.Where(p => p.Item.Start < (position.Item.End ?? position.Item.Start.AddHours(0.5)) 
                                            && (p.Item.End ?? p.Item.Start.AddHours(0.5)) > position.Item.Start).Max(p => p.Total);
            position.Total = max;
        }

        return positions;
    }

    private class ItemPosition
    {
        public CalendarItem Item { get; set; } = new();
        public int Position { get; set; }
        public int Total { get; set; }
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