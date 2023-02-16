using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Heron.MudCalendar;

public abstract class CalendarViewBase : ComponentBase
{
    [CascadingParameter]
    public MudCalendar Calendar { get; set; } = new();

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