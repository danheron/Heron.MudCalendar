using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;
using MudBlazor;
using CategoryAttribute = Heron.MudCalendar.Attributes.CategoryAttribute;
using CategoryTypes = Heron.MudCalendar.Attributes.CategoryTypes;

namespace Heron.MudCalendar;

public partial class MudCalendar<T> : MudComponentBase where T : CalendarItem
{
    /// <summary>
    /// The higher the number, the heavier the drop-shadow. 0 for no shadow.
    /// </summary>
    [Parameter]
    [Attributes.Category(CategoryTypes.Calendar.Appearance)]
    public int Elevation { get; set; } = 1;
    
    /// <summary>
    /// If true, border-radius is set to 0.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Calendar.Appearance)]
    public bool Square { get; set; }
    
    /// <summary>
    /// If true, table will be outlined.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Calendar.Appearance)]
    public bool Outlined { get; set; }

    /// <summary>
    /// The color of the buttons and other items.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Calendar.Appearance)]
    public Color Color { get; set; } = Color.Primary;

    /// <summary>
    /// The variant to use for buttons.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Calendar.Appearance)]
    public Variant ButtonVariant { get; set; } = Variant.Filled;
    
    
    /// <summary>
    /// Gets or sets the day that the calendar is showing.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Calendar.Behavior)]
    public DateTime CurrentDay { get; set; }
    
    /// <summary>
    /// Gets or sets the view (day, week, month) being shown.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Calendar.Behavior)]
    public CalendarView View { get; set; } = CalendarView.Month;

    /// <summary>
    /// If true highlights today.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Calendar.Behavior)]
    public bool HighlightToday { get; set; } = true;

    /// <summary>
    /// If false the day view is not shown.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Calendar.Behavior)]
    public bool ShowDay { get; set; } = true;

    /// <summary>
    /// If false the week view is not shown.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Calendar.Behavior)]
    public bool ShowWeek { get; set; } = true;

    /// <summary>
    /// If false the month view is not shown.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Calendar.Behavior)]
    public bool ShowMonth { get; set; } = true;
    
    /// <summary>
    /// Defines the cell content for the Month view.
    /// </summary>
    [Category(CategoryTypes.Calendar.Template)]
    [Parameter]
    public RenderFragment<T>? MonthTemplate { get; set; }
    
    /// <summary>
    /// Defines the cell content for the Week view.
    /// </summary>
    [Category(CategoryTypes.Calendar.Template)]
    [Parameter]
    public RenderFragment<T>? WeekTemplate { get; set; }
    
    /// <summary>
    /// Defines the cell content for the Day view.
    /// </summary>
    [Category(CategoryTypes.Calendar.Template)]
    [Parameter]
    public RenderFragment<T>? DayTemplate { get; set; }

    /// <summary>
    /// The data to display in the Calendar.
    /// </summary>
    [Category(CategoryTypes.Calendar.Behavior)]
    [Parameter]
    public IEnumerable<T> Items { get; set; } = new List<T>();
    
    /// <summary>
    /// Called when the dates visible in the Calendar change.
    /// </summary>
    [Parameter]
    public EventCallback<DateRange> DateRangeChanged { get; set; }
    
    /// <summary>
    /// Called when the View is changed.
    /// </summary>
    [Parameter]
    public EventCallback<CalendarView> ViewChanged { get; set; }
    
    [Parameter]
    public EventCallback<DateTime> CellClicked { get; set; }

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