using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Heron.MudCalendar.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace Heron.MudCalendar;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T">The type of item displayed in this month view.</typeparam>
public partial class MonthView<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T> : CalendarViewBase<T>, IDisposable where T:CalendarItem
{
    private MudDropContainer<T>? _dropContainer;
    private ElementReference _monthGrid;

    private JsService? _jsService;

    private static CultureInfo? _uiCulture;
    private static string? _moreText;

    protected virtual int Columns => 7;
    protected virtual int Rows => Cells.Count / Columns;

    /// <summary>
    /// Classes added to main div of component.
    /// </summary>
    protected virtual string Classname =>
        new CssBuilder("mud-cal-month-table-body")
            .AddClass("mud-cal-month-layer")
            .AddClass("mud-cal-month-fixed-height", Calendar.MonthCellMinHeight == 0)
            .Build();

    /// <summary>
    /// Styles added to the main grid.
    /// </summary>
    protected virtual string GridStyle =>
        new StyleBuilder()
            .AddStyle("grid-template-columns", $"repeat({Columns}, minmax(10px, 1fr))")
            .AddStyle("grid-template-rows",
                $"repeat({Rows}, {(100.0 / Rows).ToInvariantString()}%)",
                Calendar.MonthCellMinHeight == 0)
            .Build();

    /// <summary>
    /// Styles added to the grid that holds the contents.
    /// </summary>
    protected virtual string ContentGridStyle =>
        new StyleBuilder()
            .AddStyle("grid-template-rows",
                $"repeat({Rows}, {(100.0 / Rows).ToInvariantString()}%)",
                Calendar.MonthCellMinHeight == 0)
            .Build();

    /// <summary>
    /// Styles added to each DropZone.
    /// </summary>
    protected virtual string DropZoneStyle =>
        new StyleBuilder()
            .AddStyle("height", "100%")
            .AddStyle("width", $"{(100.0 / Columns).ToInvariantString()}%")
            .Build();

    /// <summary>
    /// Styles used to position the calendar items.
    /// </summary>
    /// <param name="position">The position information</param>
    /// <returns>Style string</returns>
    protected virtual string EventStyle(ItemPosition<T> position)
    {
        var maxWidth = Calendar.EnableParallelItemClick ? 95.0 : 100.0;
        return new StyleBuilder()
            .AddStyle("position", "absolute")
            .AddStyle("top", $"{position.Top}px")
            .AddStyle("inset-inline-start", ((double)position.Left / Columns * 100).ToInvariantString() + "%")
            .AddStyle("width", ((double)position.Width / Columns * maxWidth).ToInvariantString() + "%")
            .Build();
    }

    /// <summary>
    /// Classes added to each month cell.
    /// </summary>
    protected virtual string CellClassname(CalendarCell<T> calendarCell) =>
        new CssBuilder()
            .AddClass("mud-cal-month-cell")
            .AddClass(Calendar.AdditionalDateTimeClassesFunc?.Invoke(calendarCell.Date, CalendarView.Month))
            .AddClass("mud-cal-month-link", AllowCellLinkClick(calendarCell))
            .Build();

    /// <summary>
    /// Classes added to the day number.
    /// </summary>
    /// <param name="calendarCell">The cell.</param>
    /// <returns></returns>
    protected virtual string DayClassname(CalendarCell<T> calendarCell)
    {
        return new CssBuilder()
            .AddClass("mud-cal-month-cell-title")
            .AddClass("mud-cal-month-cell-header")
            .AddClass("mud-cal-month-outside", calendarCell.Outside)
            .Build();
    }

    /// <summary>
    /// Styles added to each cell.
    /// </summary>
    /// <param name="calendarCell">The cell.</param>
    /// <param name="index">The cell index.</param>
    /// <returns></returns>
    protected virtual string DayStyle(CalendarCell<T> calendarCell, int index)
    {
        return new StyleBuilder()
            .AddStyle("border-right", "none",
                (index + 1) % Columns == 0 && !(calendarCell.Today && Calendar.HighlightToday))
            .AddStyle("border", $"1px solid var(--mud-palette-{Calendar.Color.ToDescriptionString()})",
                calendarCell.Today && Calendar.HighlightToday)
            .AddStyle("width", $"{(100.0 / Columns).ToInvariantString()}%")
            .Build();
    }

    protected virtual string RowStyle =>
        new StyleBuilder()
            .AddStyle("min-height", Calendar.MonthCellMinHeight + "px", Calendar.MonthCellMinHeight > 0)
            .Build();

    /// <summary>
    /// Method invoked when the user clicks on the hyperlink in the cell.
    /// </summary>
    /// <param name="cell">The cell that was clicked.</param>
    /// <returns></returns>
    protected virtual async Task OnCellLinkClicked(CalendarCell<T> cell)
    {
        if (AllowCellLinkClick(cell))
        {
            await Calendar.CellClicked.InvokeAsync(cell.Date);
        }
    }

    /// <summary>
    /// Determines if the click event is allowed on a cell.
    /// </summary>
    /// <param name="cell">The cell that was clicked.</param>
    /// <returns><c>true</c> if the cell can be clicked.</returns>
    protected virtual bool AllowCellLinkClick(CalendarCell<T> cell)
    {
        return (Calendar.CellClicked.HasDelegate || Calendar.CellRangeSelected.HasDelegate) && (Calendar.IsDateTimeDisabledFunc == null || !Calendar.IsDateTimeDisabledFunc(cell.Date, CalendarView.Month));
    }

    /// <summary>
    /// Method invoked when the user right-clicks on the hyperlink in the cell.
    /// </summary>
    /// <param name="mouseEventArgs">The mouse event args.</param>   
    /// <param name="cell">The cell that was clicked.</param>
    /// <returns></returns>
    protected virtual async Task OnCellLinkContextMenuClicked(MouseEventArgs mouseEventArgs, CalendarCell<T> cell)
    {
        if (AllowCellLinkContextMenuClick(cell))
        {
            await Calendar.CellContextMenuClicked.InvokeAsync(new CalendarClickEventArgs(mouseEventArgs,cell.Date));
        }
    }

    /// <summary>
    /// Determines if the right click event is allowed on a cell.
    /// </summary>
    /// <param name="cell">The cell that was clicked.</param>
    /// <returns><c>true</c> if the cell can be clicked.</returns>
    protected virtual bool AllowCellLinkContextMenuClick(CalendarCell<T> cell)
    {
        return Calendar.CellContextMenuClicked.HasDelegate && (Calendar.IsDateTimeDisabledFunc == null || !Calendar.IsDateTimeDisabledFunc(cell.Date, CalendarView.Month));
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        // Ensure that the order of items is correct
        _dropContainer?.Refresh();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        await PositionItems();

        if (firstRender)
        {
            if (Calendar.CellRangeSelected.HasDelegate)
            {
                _jsService ??= new JsService(JsRuntime);
                await _jsService.AddMultiSelect(1, Calendar._id);
                _jsService.OnCellsSelected += OnCellRangeSelected;
            }            
        }
    }

    /// <summary>
    /// Called when the user selects a range of cells in the calendar.
    /// </summary>
    /// <param name="sender">The source object that triggered the selection event.</param>
    /// <param name="selectedCells">A collection of selected cells, each represented as a tuple containing the date and row index.</param>
    /// <returns></returns>
    protected virtual async void OnCellRangeSelected(object? sender, IEnumerable<(DateTime date, int row)> selectedCells)
    {
        await Calendar.CellRangeSelected.InvokeAsync(new DateRange(selectedCells.First().date, selectedCells.Last().date.AddDays(1)));
    }

    protected virtual bool IsSelectable(CalendarCell<T> cell)
    {
        var date = cell.Date;
        return (Calendar.IsDateTimeDisabledFunc == null || !Calendar.IsDateTimeDisabledFunc(date, CalendarView.Month));
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

    protected Task ItemWidthChanged(T item, int days, CalendarCell<T> currentCell)
    {
        // If we are resizing an item that has spanned multiple weeks we need to add the days from previous weeks
        var previousDays = currentCell.Date - item.Start.Date;

        // Calculate end date from width
        var endTime = (item.End ?? item.Start.AddHours(1)).TimeOfDay;
        item.End = item.Start.Date.AddDays(days - 1) + previousDays + endTime;
        if (item.End <= item.Start) item.End = item.Start.AddHours(1);

        return Calendar.ItemChanged.InvokeAsync(item);
    }

    protected override List<CalendarCell<T>> BuildCells()
    {
        var cells = new List<CalendarCell<T>>();

        var calendar = Calendar.Culture.Calendar;
        int year = calendar.GetYear(Calendar.CurrentDay);
        int month = calendar.GetMonth(Calendar.CurrentDay);

        var monthStart = new DateTime(year, month, 1,calendar);

        int nextMonthYear = calendar.GetYear(Calendar.CurrentDay.AddMonths(1));
        int nextMonthMonth = calendar.GetMonth(Calendar.CurrentDay.AddMonths(1));
        var monthEnd = new DateTime(nextMonthYear, nextMonthMonth, 1, Calendar.Culture.Calendar)
            .AddDays(-1);

        var range = new CalendarDateRange(Calendar.CurrentDay.Date, CalendarView.Month, Calendar.Culture);
        if (range.Start == null || range.End == null) return cells;

        var date = range.Start.Value;
        var lastDate = range.End.Value;
        while (date <= lastDate)
        {
            var cell = BuildCell(date, monthStart, monthEnd);
            cells.Add(cell);

            // Next day
            date = date.AddDays(1);
        }

        return cells;
    }

    /// <summary>
    /// Builds a cell for the month view.
    /// </summary>
    /// <param name="date">The date of the cell.</param>
    /// <param name="monthStart">The first day of the month being shown.</param>
    /// <param name="monthEnd">The last day of the month being shown.</param>
    /// <returns></returns>
    protected virtual CalendarCell<T> BuildCell(DateTime date, DateTime monthStart, DateTime monthEnd)
    {
        // Set cell properties
        var cell = new CalendarCell<T> { Date = date };
        if (date.Date == DateTime.Today) cell.Today = true;
        if (date < monthStart || date > monthEnd)
        {
            cell.Outside = true;
        }

        // Need to select all items that start today or, if it's the first day of the week, all items that started in the previous week
        var firstDateOfThisWeek = CalendarDateRange.GetFirstWeekDate(date, Calendar.FirstDayOfWeek);
        cell.Items = Calendar.Items.Where(i =>
                i.Start.Date == date ||
                (date == firstDateOfThisWeek && i.Start.Date < firstDateOfThisWeek && i.End.HasValue &&
                 i.End.Value.Date >= date))
            .OrderByDescending(i => i.IsMultiDay)
            .ThenBy(i => i.Start)
            .ToList();

        return cell;
    }

    private async Task ItemDropped(MudItemDropInfo<T> dropItem)
    {
        if (dropItem.Item == null) return;
        var item = dropItem.Item;

        // Make sure it is a valid drop zone
        var id = dropItem.DropzoneIdentifier;
        if (!DateTime.TryParse(id, out _)) return;

        // Update start and end time
        var duration = item.End?.Subtract(item.Start) ?? TimeSpan.Zero;
        item.Start = DateTime.Parse(dropItem.DropzoneIdentifier).Add(item.Start.TimeOfDay);
        if (item.End.HasValue)
        {
            item.End = item.Start.Add(duration);
        }

        Calendar.Refresh();

        await Calendar.ItemChanged.InvokeAsync(item);
    }

    private Task PositionItems()
    {
        // Load localized text
        var moreText = LoadText();

        if (_jsService == null)
        {
            _jsService = new JsService(JsRuntime);
            if (Calendar.MoreClicked.HasDelegate)
            {
                _jsService.OnMoreClicked += (_, date) => Calendar.MoreClicked.InvokeAsync(date);
            }
        }
        return _jsService.PositionMonthItems(_monthGrid, moreText, Calendar.MonthCellMinHeight == 0);
    }

    private string LoadText()
    {
        if (_moreText != null && Equals(_uiCulture, Calendar.Culture)) return _moreText;

        var options = Options.Create(new LocalizationOptions { ResourcesPath = "Resources" });
        var factory = new ResourceManagerStringLocalizerFactory(options, NullLoggerFactory.Instance);
        var localizer = new StringLocalizer<MudCalendar<T>>(factory);

        _uiCulture = Calendar.Culture;
        _moreText = localizer["More"];

        return _moreText;
    }

    private int CalcWidth(ItemPosition<T> position, int cellIndex, DateOnly date)
    {
        // Get number of days
        var item = position.Item;
        var endDate = item.End ?? item.Start.AddHours(1);
        var days = endDate.Date - item.Start.Date;
        if (date.ToDateTime(TimeOnly.MinValue) > item.Start.Date)
        {
            days = endDate.Date - date.ToDateTime(TimeOnly.MinValue);
        }

        var width = days.Days + 1;

        var cellColumn = cellIndex % Columns;
        if (cellColumn + width > Columns) width = Columns - cellColumn;

        return width;
    }

    private RenderFragment<T> CellTemplate => Calendar.MonthTemplate ?? Calendar.CellTemplate;

    private IEnumerable<ItemPosition<T>> CalcPositions(IEnumerable<T> items, DateOnly date, int cellIndex)
    {
        var positions = new List<ItemPosition<T>>();

        foreach (var item in items)
        {
            // Create new position object
            var position = new ItemPosition<T>
            {
                Item = item,
                Position = 0,
                Total = 1,
                Date = date,
                Top = 36,
                Left = cellIndex % Columns
            };
            position.Width = CalcWidth(position, cellIndex, date);
            positions.Add(position);
        }

        return positions;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        _jsService?.Dispose();
    }
}