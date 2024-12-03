using System.Globalization;
using Heron.MudCalendar.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace Heron.MudCalendar;

public partial class MonthView : CalendarViewBase, IDisposable
{
    private MudDropContainer<CalendarItem>? _dropContainer;

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
            .AddClass("mud-cal-month-fixed-height", Calendar.MonthCellMinHeight == 0)
            .Build();

    /// <summary>
    /// Styles added to the main grid.
    /// </summary>
    protected virtual string GridStyle =>
        new StyleBuilder()
            .AddStyle("grid-template-columns", $"repeat({Columns}, minmax(10px, 1fr))")
            .AddStyle("grid-template-rows",
                $"repeat({Cells.Count / Columns}, {100.0 / (Cells.Count / (double)Columns)}%)",
                Calendar.MonthCellMinHeight == 0)
            .Build();

    /// <summary>
    /// Styles added to the grid that holds the contents.
    /// </summary>
    protected virtual string ContentGridStyle =>
        new StyleBuilder()
            .AddStyle("grid-template-rows",
                $"repeat({Cells.Count / Columns}, {100.0 / (Cells.Count / (double)Columns)}%)",
                Calendar.MonthCellMinHeight == 0)
            .Build();

    /// <summary>
    /// Styles added to each DropZone.
    /// </summary>
    protected virtual string DropZoneStyle =>
        new StyleBuilder()
            .AddStyle("height", "100%")
            .AddStyle("width", $"{100.0 / Columns}%")
            .Build();

    /// <summary>
    /// Styles used to position the calendar items.
    /// </summary>
    /// <param name="position">The position information</param>
    /// <returns>Style string</returns>
    protected virtual string EventStyle(ItemPosition position)
    {
        return new StyleBuilder()
            .AddStyle("position", "absolute")
            .AddStyle("top", $"{position.Top}px")
            .AddStyle("left", ((double)position.Left / Columns * 100).ToInvariantString() + "%")
            .AddStyle("width", ((double)position.Width / Columns * 100).ToInvariantString() + "%")
            .Build();
    }

    /// <summary>
    /// Classes added to each month cell.
    /// </summary>
    protected virtual string CellClassname =>
        new CssBuilder()
            .AddClass("mud-cal-month-cell")
            .AddClass("mud-cal-month-link", Calendar.CellClicked.HasDelegate)
            .Build();

    /// <summary>
    /// Classes added to the day number.
    /// </summary>
    /// <param name="calendarCell">The cell.</param>
    /// <returns></returns>
    protected virtual string DayClassname(CalendarCell calendarCell)
    {
        return new CssBuilder()
            .AddClass("mud-cal-month-cell-title")
            .AddClass("mud-cal-month-outside", calendarCell.Outside)
            .Build();
    }

    /// <summary>
    /// Styles added to each cell.
    /// </summary>
    /// <param name="calendarCell">The cell.</param>
    /// <param name="index">The cell index.</param>
    /// <returns></returns>
    protected virtual string DayStyle(CalendarCell calendarCell, int index)
    {
        return new StyleBuilder()
            .AddStyle("border-right", "none",
                (index + 1) % Columns == 0 && !(calendarCell.Today && Calendar.HighlightToday))
            .AddStyle("border", $"1px solid var(--mud-palette-{Calendar.Color.ToDescriptionString()})",
                calendarCell.Today && Calendar.HighlightToday)
            .AddStyle("min-height", Calendar.MonthCellMinHeight + "px", Calendar.MonthCellMinHeight > 0)
            .Build();
    }

    /// <summary>
    /// Method invoked when the user clicks on the hyperlink in the cell.
    /// </summary>
    /// <param name="cell">The cell that was clicked.</param>
    /// <returns></returns>
    protected virtual async Task OnCellLinkClicked(CalendarCell cell)
    {
        if (Calendar.CellClicked.HasDelegate)
        {
            await Calendar.CellClicked.InvokeAsync(cell.Date);
        }
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

    protected Task ItemWidthChanged(CalendarItem item, int days, CalendarCell currentCell)
    {
        // If we are resizing an item that has spanned multiple weeks we need to add the days from previous weeks
        var previousDays = currentCell.Date - item.Start.Date;

        // Calculate end date from width
        var endTime = (item.End ?? item.Start.AddHours(1)).TimeOfDay;
        item.End = item.Start.Date.AddDays(days - 1) + previousDays + endTime;
        if (item.End <= item.Start) item.End = item.Start.AddHours(1);

        return Calendar.ItemChanged.InvokeAsync(item);
    }

    protected override List<CalendarCell> BuildCells()
    {
        var cells = new List<CalendarCell>();
        var monthStart = new DateTime(Calendar.CurrentDay.Year, Calendar.CurrentDay.Month, 1);
        var monthEnd = new DateTime(Calendar.CurrentDay.AddMonths(1).Year, Calendar.CurrentDay.AddMonths(1).Month, 1)
            .AddDays(-1);

        var range = new CalendarDateRange(Calendar.CurrentDay.Date, CalendarView.Month);
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
    protected virtual CalendarCell BuildCell(DateTime date, DateTime monthStart, DateTime monthEnd)
    {
        // Set cell properties
        var cell = new CalendarCell { Date = date };
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

    private async Task ItemDropped(MudItemDropInfo<CalendarItem> dropItem)
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

        _jsService ??= new JsService(JsRuntime);
        return _jsService.PositionMonthItems(moreText);
    }

    private string LoadText()
    {
        if (_moreText != null && Equals(_uiCulture, Thread.CurrentThread.CurrentUICulture)) return _moreText;

        var options = Options.Create(new LocalizationOptions { ResourcesPath = "Resources" });
        var factory = new ResourceManagerStringLocalizerFactory(options, NullLoggerFactory.Instance);
        var localizer = new StringLocalizer<MudCalendar>(factory);

        _uiCulture = Thread.CurrentThread.CurrentUICulture;
        _moreText = localizer["More"];

        return _moreText;
    }

    private int CalcWidth(ItemPosition position, int cellIndex, DateOnly date)
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

    private RenderFragment<CalendarItem> CellTemplate => Calendar.MonthTemplate ?? Calendar.CellTemplate;

    private IEnumerable<ItemPosition> CalcPositions(IEnumerable<CalendarItem> items, DateOnly date, int cellIndex)
    {
        var positions = new List<ItemPosition>();

        foreach (var item in items)
        {
            // Create new position object
            var position = new ItemPosition
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