using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Heron.MudCalendar;

public abstract class CalendarViewBase : ComponentBase
{
    /// <summary>
    /// The color of the buttons and other items.
    /// </summary>
    [Parameter]
    public Color Color { get; set; } = Color.Primary;

    /// <summary>
    /// Gets or sets the day that the calendar is showing.
    /// </summary>
    [Parameter]
    public DateTime CurrentDay { get; set; }

    /// <summary>
    /// If true highlights today.
    /// </summary>
    [Parameter]
    public bool HighlightToday { get; set; } = true;

    /// <summary>
    /// Defines the cell content for the month view.
    /// </summary>
    [Parameter]
    public RenderFragment<CalendarItem>? CellTemplate { get; set; }

    /// <summary>
    /// The data to display in the month view.
    /// </summary>
    [Parameter]
    public IEnumerable<CalendarItem> Items { get; set; } = new List<CalendarItem>();
    
    /// <summary>
    /// Called when a cell is clicked.
    /// </summary>
    [Parameter]
    public EventCallback<DateTime> CellClicked { get; set; }

    protected List<CalendarCell> Cells = new();

    protected override void OnParametersSet()
    {
        Cells = BuildCells();
    }
    
    /// <summary>
    /// Builds a collection of cells that will be displayed in the month view.
    /// </summary>
    protected abstract List<CalendarCell> BuildCells();
}