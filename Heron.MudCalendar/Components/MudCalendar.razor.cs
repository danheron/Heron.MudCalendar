using System.Globalization;
using Heron.MudCalendar.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MudBlazor.Utilities;
using MudBlazor;
using CategoryAttribute = Heron.MudCalendar.Attributes.CategoryAttribute;
using CategoryTypes = Heron.MudCalendar.Attributes.CategoryTypes;

namespace Heron.MudCalendar;

public partial class MudCalendar : MudComponentBase
{
    /// <summary>
    /// The higher the number, the heavier the drop-shadow. 0 for no shadow.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Calendar.Appearance)]
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
    /// The height of the calendar component.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Calendar.Appearance)]
    public int Height { get; set; } = 700;

    /// <summary>
    /// If 0 the calendar will be fixed height. If set the month view will exapnd when necessary with this being the minimum height of each cell.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Calendar.Appearance)]
    public int MonthCellMinHeight { get; set; }
    
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
    /// If false then the prev/next buttons are not shown.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Calendar.Behavior)]
    public bool ShowPrevNextButtons { get; set; } = true;
    
    /// <summary>
    /// If false the the Datepicker is not shown.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Calendar.Behavior)]
    public bool ShowDatePicker { get; set; } = true;

    /// <summary>
    /// If true the Today button is shown.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Calendar.Behavior)]
    public bool ShowTodayButton { get; set; }

    /// <summary>
    /// Set the day start time for week/day views.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Calendar.Behavior)]
    public TimeOnly DayStartTime { get; set; } = new(8, 0);

    /// <summary>
    /// Set the time interval of cells in day and week view.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Calendar.Behavior)]
    public CalendarTimeInterval DayTimeInterval { get; set; } = CalendarTimeInterval.Minutes30;

    /// <summary>
    /// Set the height of each cell in day and week view.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Calendar.Behavior)]
    public int DayCellHeight { get; set; } = 36;

    /// <summary>
    /// If true then a line indicating the current time is shown in day and week view.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Calendar.Appearance)]
    public bool ShowCurrentTime { get; set; }

    /// <summary>
    /// If true then calendar items can be drag/dropped to different dates/times.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Calendar.Behavior)]
    public bool EnableDragItems { get; set; }

    /// <summary>
    /// If true then the user can change the duration of an item by resizing the item.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Calendar.Behavior)]
    public bool EnableResizeItems { get; set; }

    /// <summary>
    /// If true then use 24 hour clock, otherwise use 12 hour format (am/pm).
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Calendar.Appearance)]
    public bool Use24HourClock { get; set; } = true;
    
    /// <summary>
    /// Defines the cell content for the Month view.
    /// </summary>
    [Category(CategoryTypes.Calendar.Template)]
    [Parameter]
    public RenderFragment<CalendarItem>? MonthTemplate { get; set; }
    
    /// <summary>
    /// Defines the cell content for the Week view.
    /// </summary>
    [Category(CategoryTypes.Calendar.Template)]
    [Parameter]
    public RenderFragment<CalendarItem>? WeekTemplate { get; set; }
    
    /// <summary>
    /// Defines the cell content for the Day view.
    /// </summary>
    [Category(CategoryTypes.Calendar.Template)]
    [Parameter]
    public RenderFragment<CalendarItem>? DayTemplate { get; set; }

    /// <summary>
    /// The data to display in the Calendar.
    /// </summary>
    [Category(CategoryTypes.Calendar.Behavior)]
    [Parameter]
    public IEnumerable<CalendarItem> Items { get; set; } = new List<CalendarItem>();
    
    /// <summary>
    /// Called when the dates visible in the Calendar change.
    /// </summary>
    [Parameter]
    public EventCallback<DateRange> DateRangeChanged { get; set; }

    /// <summary>
    /// Called when an item is changed, for example by dragging or resizing the item.
    /// </summary>
    [Parameter]
    public EventCallback<CalendarItem> ItemChanged { get; set; }

    /// <summary>
    /// Called when the View is changed.
    /// </summary>
    [Parameter]
    public EventCallback<CalendarView> ViewChanged { get; set; }
    
    /// <summary>
    /// Called when a cell is clicked.
    /// </summary>
    [Parameter]
    public EventCallback<DateTime> CellClicked { get; set; }
    
    /// <summary>
    /// Called when a CalendarItem is clicked.
    /// </summary>
    [Parameter]
    public EventCallback<CalendarItem> ItemClicked { get; set; }

    private DateTime? PickerDate
    {
        get => CurrentDay;
        set => CurrentDay = value ?? DateTime.Today;
    }

    private CalendarDateRange? _currentDateRange;

    private CalendarDatePicker? _datePicker;
    
    private JsService? _jsService;

    private static CultureInfo? _uiCulture;
    private static string? _todayText;

    /// <summary>
    /// Classes added to main div of component.
    /// </summary>
    protected virtual string Classname =>
        new CssBuilder("mud-calendar")
            .AddClass("mud-cal-outlined", Outlined)
            .AddClass("mud-cal-square", Square)
            .AddClass($"mud-elevation-{Elevation}", !Outlined)
            .AddClass(Class)
            .Build();

    /// <summary>
    /// Styles added to main div of component.
    /// </summary>
    protected virtual string Styles =>
        new StyleBuilder("min-height", $"{Height}px")
            .AddStyle(Style)
            .Build();

    protected virtual string ViewClassname =>
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
            await ChangeDateRange();

            await SetLinks();
        }
    }
    
    /// <summary>
    /// Forces the component to be redrawn.
    /// </summary>
    /// <returns></returns>
    public void Refresh()
    {
        StateHasChanged();
    }

    /// <summary>
    /// Method invoked when the user changes the view.
    /// </summary>
    /// <param name="view">The new view that is being shown.</param>
    /// <returns></returns>
    protected virtual Task OnViewChanging(CalendarView view)
    {
        View = view;
        var viewTask = ViewChanged.InvokeAsync(View);
        var dateRangeTask = ChangeDateRange();
        return Task.WhenAll(viewTask, dateRangeTask);
    }

    /// <summary>
    /// Method invoked when the user clicks the next button.
    /// </summary>
    /// <returns></returns>
    protected virtual Task OnNextClicked()
    {
        CurrentDay = View switch
        {
            CalendarView.Day => CurrentDay.AddDays(1),
            CalendarView.Week => CurrentDay.AddDays(7),
            CalendarView.Month => CurrentDay.AddMonths(1),
            _ => CurrentDay
        };
        
        return ChangeDateRange();
    }

    /// <summary>
    /// Method invoked when the user clicks the previous button.
    /// </summary>
    /// <returns></returns>
    protected virtual Task OnPreviousClicked()
    {
        CurrentDay = View switch
        {
            CalendarView.Day => CurrentDay.AddDays(-1),
            CalendarView.Week => CurrentDay.AddDays(-7),
            CalendarView.Month => CurrentDay.AddMonths(-1),
            _ => CurrentDay
        };
        
        return ChangeDateRange();
    }

    /// <summary>
    /// Method invoked when the user clicks the today button.
    /// </summary>
    /// <returns></returns>
    protected virtual Task OnTodayClicked()
    {
        CurrentDay = DateTime.Today;

        return ChangeDateRange();
    }
    
    protected string DrawTodayText()
    {
        if (_todayText != null && Equals(_uiCulture, Thread.CurrentThread.CurrentUICulture)) return _todayText;

        var options = Options.Create(new LocalizationOptions { ResourcesPath = "Resources" });
        var factory = new ResourceManagerStringLocalizerFactory(options, NullLoggerFactory.Instance);
        var localizer = new StringLocalizer<MudCalendar>(factory);

        _uiCulture = Thread.CurrentThread.CurrentUICulture;
        _todayText = localizer["Today"];

        return _todayText;
    }

    private async Task SetLinks()
    {
        // Check if link is already set
        _jsService ??= new JsService(JsRuntime);
        var head = await _jsService.GetHeadContent();
        if (!string.IsNullOrEmpty(head) && head.Contains("Heron.MudCalendar.min.css")) return;

        // Add link
        await _jsService.AddLink("_content/Heron.MudCalendar/Heron.MudCalendar.min.css", "stylesheet");
    }

    private Task DatePickerDateChanged(DateTime? dateTime)
    {
        PickerDate = dateTime;
        return ChangeDateRange(new CalendarDateRange(dateTime ?? DateTime.Today, View));
    }

    private void OnDatePickerOpened()
    {
        _datePicker?.GoToDate(CurrentDay);
    }

    private async Task ChangeDateRange()
    {
        await ChangeDateRange(new CalendarDateRange(CurrentDay, View));
    }

    private async Task ChangeDateRange(CalendarDateRange dateRange)
    {
        if (dateRange != _currentDateRange)
        {
            _currentDateRange = dateRange;
            await DateRangeChanged.InvokeAsync(dateRange);
        }
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