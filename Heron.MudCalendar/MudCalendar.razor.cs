using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace Heron.MudCalendar;

public partial class MudCalendar<T> : MudComponentBase where T : CalendarItem
{
    [Parameter]
    public int Elevation { get; set; } = 1;
    
    [Parameter]
    public bool Square { get; set; }
    
    [Parameter]
    public bool Outlined { get; set; }

    [Parameter]
    public Color Color { get; set; } = Color.Primary;

    [Parameter]
    public Variant ButtonVariant { get; set; } = Variant.Filled;
    
    [Parameter]
    public DateTime CurrentDay { get; set; }
    
    [Parameter]
    public CalendarView View { get; set; } = CalendarView.Month;

    [Parameter]
    public bool HighlightToday { get; set; } = true;

    [Parameter]
    public bool ShowDay { get; set; } = true;

    [Parameter]
    public bool ShowWeek { get; set; } = true;

    [Parameter]
    public bool ShowMonth { get; set; } = true;
    
    [Parameter]
    public RenderFragment<T>? MonthTemplate { get; set; }
    
    [Parameter]
    public RenderFragment<T>? WeekTemplate { get; set; }
    
    [Parameter]
    public RenderFragment<T>? DayTemplate { get; set; }

    [Parameter]
    public IEnumerable<T> Items { get; set; } = new List<T>();
    
    [Parameter]
    public EventCallback<DateRange> DateRangeChanged { get; set; }
    
    [Parameter]
    public EventCallback<CalendarView> ViewChanged { get; set; }

    private DateTime? PickerDate
    {
        get => CurrentDay;
        set => CurrentDay = value ?? DateTime.Today;
    }

    protected string Classname =>
        new CssBuilder("mud-calendar")
            .AddClass("mud-cal-outlined", Outlined)
            .AddClass("mud-cal-square", Square)
            .AddClass($"mud-elevation-{Elevation}", !Outlined)
            .AddClass(Class)
            .Build();

    protected string ViewClassname =>
        new CssBuilder("flex-grow-1")
            .AddClass("d-none", AllowedViews().Count == 0)
            .Build();

    protected override void OnInitialized()
    {
        CurrentDay = DateTime.Today;
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        
        // Ensure that current view is allowed
        if ((View == CalendarView.Day && !ShowDay)
            || (View == CalendarView.Week && !ShowWeek)
            || (View == CalendarView.Month && !ShowMonth))
        {
            if (ShowMonth) View = CalendarView.Month;
            if (ShowWeek) View = CalendarView.Week;
            if (ShowDay) View = CalendarView.Day;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            await DateRangeChanged.InvokeAsync(new CalendarDateRange(CurrentDay, View));
        }
    }

    private Task OnViewChange(CalendarView view)
    {
        View = view;
        var viewTask = ViewChanged.InvokeAsync(View);
        var dateRangeTask = DateRangeChanged.InvokeAsync(new CalendarDateRange(CurrentDay, View));
        return Task.WhenAll(viewTask, dateRangeTask);
    }

    private Task Next()
    {
        CurrentDay = View switch
        {
            CalendarView.Day => CurrentDay.AddDays(1),
            CalendarView.Week => CurrentDay.AddDays(7),
            CalendarView.Month => CurrentDay.AddMonths(1),
            _ => CurrentDay
        };

        return DateRangeChanged.InvokeAsync(new CalendarDateRange(CurrentDay, View));
    }

    private Task Previous()
    {
        CurrentDay = View switch
        {
            CalendarView.Day => CurrentDay.AddDays(-1),
            CalendarView.Week => CurrentDay.AddDays(-7),
            CalendarView.Month => CurrentDay.AddMonths(-1),
            _ => CurrentDay
        };
        
        return DateRangeChanged.InvokeAsync(new CalendarDateRange(CurrentDay, View));
    }

    private List<CalendarView> AllowedViews()
    {
        var list = new List<CalendarView>();
        if (ShowDay) list.Add(CalendarView.Day);
        if (ShowWeek) list.Add(CalendarView.Week);
        if (ShowMonth) list.Add(CalendarView.Month);
        return list;
    }
}