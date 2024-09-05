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

/// <summary>
///  Calendar component for MudBlazor.
/// </summary>
public partial class MudCalendar : MudComponentBase
{
    /// <summary>
    /// The size of the drop shadow.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>1</c>.  A higher number creates a heavier drop shadow.  Use a value of <c>0</c> for no shadow.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Calendar.Appearance)]
    public int Elevation { get; set; } = 1;
    
    /// <summary>
    /// Disables rounded corners.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Calendar.Appearance)]
    public bool Square { get; set; }
    
    /// <summary>
    /// Shows an outline around the calendar.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Calendar.Appearance)]
    public bool Outlined { get; set; }

    /// <summary>
    /// The color of the buttons and other items.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>Color.Primary</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Calendar.Appearance)]
    public Color Color { get; set; } = Color.Primary;

    /// <summary>
    /// The variant to use for buttons.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>Variant.Filled</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Calendar.Appearance)]
    public Variant ButtonVariant { get; set; } = Variant.Filled;

    /// <summary>
    /// The height of the calendar component.
    /// </summary>
    /// /// <remarks>
    /// Defaults to <c>700</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Calendar.Appearance)]
    public int Height { get; set; } = 700;

    /// <summary>
    /// Gets or sets the minimum height of a cell.
    /// </summary>
    /// /// <remarks>
    /// Defaults to <c>0</c>. If 0 the calendar will be fixed height. If set the month view will expand when necessary with this being the minimum height of each cell.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Calendar.Appearance)]
    public int MonthCellMinHeight { get; set; }
    
    /// <summary>
    /// Gets or sets the day that the calendar is showing.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>Today</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Calendar.Behavior)]
    public DateTime CurrentDay { get; set; }

    /// <summary>
    /// Gets or sets the first day of the week that the calendar is showing in Week View.
    /// This value is also used for the Work Week View if <see cref="FirstDayOfWorkWeek"/> is not set.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Calendar.Behavior)]
    public DayOfWeek? FirstDayOfWeek { get; set; } = null;

    /// <summary>
    /// Gets or sets the first day of the week that the calendar is showing in Work Week View.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Calendar.Behavior)]
    public DayOfWeek? FirstDayOfWorkWeek { get; set; } = null;

    /// <summary>
    /// Gets or sets the view (day, week, month) being shown.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>CalendarView.Month</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Calendar.Behavior)]
    public CalendarView View { get; set; } = CalendarView.Month;

    /// <summary>
    /// If true highlights today.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Calendar.Behavior)]
    public bool HighlightToday { get; set; } = true;

    /// <summary>
    /// If false the day view is not shown.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Calendar.Behavior)]
    public bool ShowDay { get; set; } = true;

    /// <summary>
    /// If false the week view is not shown.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Calendar.Behavior)]
    public bool ShowWeek { get; set; } = true;

    /// <summary>
    /// If false the work week view is not shown.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Calendar.Behavior)]
    public bool ShowWorkWeek { get; set; } = false;

    /// <summary>
    /// If false the month view is not shown.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Calendar.Behavior)]
    public bool ShowMonth { get; set; } = true;

    /// <summary>
    /// If false then the prev/next buttons are not shown.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Calendar.Behavior)]
    public bool ShowPrevNextButtons { get; set; } = true;
    
    /// <summary>
    /// If false the the Datepicker is not shown.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Calendar.Behavior)]
    public bool ShowDatePicker { get; set; } = true;

    /// <summary>
    /// If true the Today button is shown.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Calendar.Behavior)]
    public bool ShowTodayButton { get; set; }

    /// <summary>
    /// Set the day start time for week/day views.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>08:00</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Calendar.Behavior)]
    public TimeOnly DayStartTime { get; set; } = new(8, 0);

    /// <summary>
    /// Set the time interval of cells in day and week view.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>CalendarTimeInterval.Minutes30</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Calendar.Behavior)]
    public CalendarTimeInterval DayTimeInterval { get; set; } = CalendarTimeInterval.Minutes30;

    /// <summary>
    /// Set the height of each cell in day and week view.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>36</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Calendar.Behavior)]
    public int DayCellHeight { get; set; } = 36;

    /// <summary>
    /// Set a minimum height for calendar items in day/week views.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>0</c>. This property can be helpful if there are items with a very short duration.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Calendar.Behavior)]
    public int DayItemMinHeight { get; set; }

    /// <summary>
    /// If true then a line indicating the current time is shown in day and week view.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Calendar.Appearance)]
    public bool ShowCurrentTime { get; set; }

    /// <summary>
    /// If true then calendar items can be drag/dropped to different dates/times.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Calendar.Behavior)]
    public bool EnableDragItems { get; set; }

    /// <summary>
    /// If true then the user can change the duration of an item by resizing the item.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Calendar.Behavior)]
    public bool EnableResizeItems { get; set; }

    /// <summary>
    /// If true then use 24 hour clock, otherwise use 12 hour format (am/pm).
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Calendar.Appearance)]
    public bool Use24HourClock { get; set; } = true;
    
    /// <summary>
    /// Defines the cell content for the Month view.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Category(CategoryTypes.Calendar.Template)]
    [Parameter]
    public RenderFragment<CalendarItem>? MonthTemplate { get; set; }
    
    /// <summary>
    /// Defines the cell content for the Week view.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Category(CategoryTypes.Calendar.Template)]
    [Parameter]
    public RenderFragment<CalendarItem>? WeekTemplate { get; set; }
    
    /// <summary>
    /// Defines the cell content for the Day view.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Category(CategoryTypes.Calendar.Template)]
    [Parameter]
    public RenderFragment<CalendarItem>? DayTemplate { get; set; }
    
    /// <summary>
    /// Custom content to appear in the toolbar of the component.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Calendar.Behavior)]
    public RenderFragment? ToolbarContent { get; set; }

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
    /// Called when the current day changes.
    /// </summary>
    [Parameter]
    public EventCallback<DateTime> CurrentDayChanged { get; set; }

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
        if (CurrentDay == default)
        {
            CurrentDay = DateTime.Today;   
        }
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        
        // Ensure that current view is allowed
        if ((View == CalendarView.Day && !ShowDay)
            || (View == CalendarView.Week && !ShowWeek)
            || (View == CalendarView.WorkWeek && !ShowWorkWeek)
            || (View == CalendarView.Month && !ShowMonth))
        {
            if (ShowMonth) View = CalendarView.Month;
            if (ShowWeek) View = CalendarView.Week;
            if (ShowWorkWeek) View = CalendarView.WorkWeek;
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
    /// Method to return the first day of the week based on the current view and parameters set on the component.
    /// </summary>
    /// <returns>DayOfWeek or null</returns>
    /// <inheritdoc cref="GetFirstDayOfWeekByCalendarView(CalendarView)"/>
    public DayOfWeek? GetFirstDayOfWeekByCalendarView(CalendarView view)
    {
        return view == CalendarView.WorkWeek
            ? (FirstDayOfWorkWeek ?? FirstDayOfWeek)
            : FirstDayOfWeek;
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
    protected virtual async Task OnNextClicked()
    {
        CurrentDay = View switch
        {
            CalendarView.Day => CurrentDay.AddDays(1),
            CalendarView.Week => CurrentDay.AddDays(7),
            CalendarView.WorkWeek => CurrentDay.AddDays(7),
            CalendarView.Month => CurrentDay.AddMonths(1),
            _ => CurrentDay
        };
        
        await CurrentDayChanged.InvokeAsync(CurrentDay);
        
        await ChangeDateRange();
    }

    /// <summary>
    /// Method invoked when the user clicks the previous button.
    /// </summary>
    /// <returns></returns>
    protected virtual async Task OnPreviousClicked()
    {
        CurrentDay = View switch
        {
            CalendarView.Day => CurrentDay.AddDays(-1),
            CalendarView.Week => CurrentDay.AddDays(-7),
            CalendarView.WorkWeek => CurrentDay.AddDays(-7),
            CalendarView.Month => CurrentDay.AddMonths(-1),
            _ => CurrentDay
        };

        await CurrentDayChanged.InvokeAsync(CurrentDay);
        
        await ChangeDateRange();
    }

    /// <summary>
    /// Method invoked when the user clicks the today button.
    /// </summary>
    /// <returns></returns>
    protected virtual async Task OnTodayClicked()
    {
        if (CurrentDay == DateTime.Today) return;
        
        CurrentDay = DateTime.Today;

        await CurrentDayChanged.InvokeAsync(CurrentDay);
        
        await ChangeDateRange();
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

    private async Task DatePickerDateChanged(DateTime? dateTime)
    {
        var dateChanged = dateTime.HasValue && dateTime != CurrentDay;
        
        PickerDate = dateTime;
        
        if (dateChanged) await CurrentDayChanged.InvokeAsync(CurrentDay);
        
        await ChangeDateRange(new CalendarDateRange(dateTime ?? DateTime.Today, View, GetFirstDayOfWeekByCalendarView(View)));
    }

    private void OnDatePickerOpened()
    {
        _datePicker?.GoToDate(CurrentDay);
    }

    private async Task ChangeDateRange()
    {
        await ChangeDateRange(new CalendarDateRange(CurrentDay, View, GetFirstDayOfWeekByCalendarView(View)));
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
        if (ShowWorkWeek) list.Add(CalendarView.WorkWeek);
        if (ShowMonth) list.Add(CalendarView.Month);
        return list;
    }
}