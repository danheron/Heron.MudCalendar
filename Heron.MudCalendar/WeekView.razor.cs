using System.Globalization;
using Heron.MudCalendar.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace Heron.MudCalendar;

public partial class WeekView<T> : IAsyncDisposable where T : CalendarItem
{
    [Parameter]
    public CalendarView View { get; set; } = CalendarView.Week;
    
    [Parameter]
    public Color Color { get; set; } = Color.Primary;
    
    [Parameter]
    public DateTime CurrentDay { get; set; }
    
    [Parameter]
    public bool HighlightToday { get; set; } = true;
    
    [Parameter]
    public RenderFragment<T>? CellTemplate { get; set; }
    
    [Parameter]
    public IEnumerable<T> Items { get; set; } = new List<T>();

    private List<CalendarCell<T>> _cells = new();
    private ElementReference _scrollDiv;
    private JsService? _jsService;
    
    private const int DayStartTime = 8;
    
    private const int MinutesInDay = 24 * 60;
    private const int PixelsInDay = 48 * 36;

    protected override void OnParametersSet()
    {
        if (View == CalendarView.Week)
        {
            BuildCellsWeek();   
        }
        else
        {
            BuildCellsDay();
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            await ScrollToDay();
        }
    }

    private string DayStyle(CalendarCell<T> calendarCell)
    {
        return new StyleBuilder()
            .AddStyle("border", $"1px solid var(--mud-palette-{Color.ToDescriptionString()})", calendarCell.Today && HighlightToday)
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
        var percent = (double)(DayStartTime * 60) / MinutesInDay;
        var scrollTo = PixelsInDay * percent;

        _jsService ??= new JsService(JsRuntime);
        await _jsService.Scroll(_scrollDiv, (int)scrollTo);
    }

    private void BuildCellsWeek()
    {
        var cells = new List<CalendarCell<T>>();
        var range = new CalendarDateRange(CurrentDay, View);
        if (range.Start != null && range.End != null)
        {
            var date = range.Start.Value;
            var lastDate = range.End.Value;
            while (date <= lastDate)
            {
                var cell = new CalendarCell<T> { Date = date };
                if (date.Date == DateTime.Today) cell.Today = true;
            
                cell.Items = Items.Where(i => i.Start >= date && i.Start < date.AddDays(1)).ToList();
            
                cells.Add(cell);
            
                // Next day
                date = date.AddDays(1);
            }
        }

        _cells = cells;
    }
    
    private void BuildCellsDay()
    {
        var cell = new CalendarCell<T> { Date = CurrentDay };
        if (CurrentDay.Date == DateTime.Today) cell.Today = true;
        
        cell.Items = Items.Where(i => i.Start >= CurrentDay && i.Start < CurrentDay.AddDays(1)).ToList();

        _cells = new List<CalendarCell<T>> { cell };
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