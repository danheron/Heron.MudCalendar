using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;

namespace Heron.MudCalendar;

public abstract class CalendarViewBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T> : ComponentBase where T:CalendarItem
{
    [CascadingParameter]
    public MudCalendar<T> Calendar { get; set; } = new();

    protected List<CalendarCell<T>> Cells = new();

    protected override void OnParametersSet()
    {
        Cells = BuildCells();
    }
    
    /// <summary>
    /// Builds a collection of cells that will be displayed in the month view.
    /// </summary>
    protected abstract List<CalendarCell<T>> BuildCells();
}