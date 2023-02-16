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
    
    private string EventStyle(CalendarItem item)
    {
        return new StyleBuilder()
            .AddStyle("position", "absolute")
            .AddStyle("top", $"{CalcTop(item)}px")
            .AddStyle("height", $"{CalcHeight(item)}px")
            .AddStyle("width", "100%")
            .AddStyle("overflow", "hidden")
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

    private int CalcTop(CalendarItem item)
    {
        double minutes = item.Start.Hour * 60 + item.Start.Minute;
        var percent = minutes / MinutesInDay;
        var top = PixelsInDay * percent;

        return (int)top;
    }

    private int CalcHeight(CalendarItem item)
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

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        
        if (_jsService != null)
        {
            await _jsService.DisposeAsync();
        }
    }
    
    protected virtual RenderFragment<CalendarItem> CellTemplate => Calendar.CellTemplate;
}