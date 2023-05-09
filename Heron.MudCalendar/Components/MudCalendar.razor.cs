using Microsoft.AspNetCore.Components;
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
    /// Called when the View is changed.
    /// </summary>
    [Parameter]
    public EventCallback<CalendarView> ViewChanged { get; set; }
    
    /// <summary>
    /// Called when a cell is clicked.
    /// </summary>
    [Parameter]
    public EventCallback<DateTime> CellClicked { get; set; }

    private DateTime? PickerDate
    {
        get => CurrentDay;
        set => CurrentDay = value ?? DateTime.Today;
    }

    private CalendarDatePicker? _datePicker;

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

    private string ViewClassname =>
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

    /// <summary>
    /// Method invoked when the user changes the view.
    /// </summary>
    /// <param name="view">The new view that is being shown.</param>
    /// <returns></returns>
    protected virtual Task OnViewChanging(CalendarView view)
    {
        View = view;
        var viewTask = ViewChanged.InvokeAsync(View);
        var dateRangeTask = DateRangeChanged.InvokeAsync(new CalendarDateRange(CurrentDay, View));
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

        return DateRangeChanged.InvokeAsync(new CalendarDateRange(CurrentDay, View));
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
        
        return DateRangeChanged.InvokeAsync(new CalendarDateRange(CurrentDay, View));
    }

    private Task DatePickerDateChanged(DateTime? dateTime)
    {
        PickerDate = dateTime;
        return DateRangeChanged.InvokeAsync(new CalendarDateRange(dateTime ?? DateTime.Today, View));
    }

    private void OnDatePickerOpened()
    {
        _datePicker?.GoToDate(CurrentDay);
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