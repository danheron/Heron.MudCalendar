using System.Diagnostics.CodeAnalysis;

namespace Heron.MudCalendar;

/// <summary>
/// Context passed to <see cref="MudCalendar{T}.MonthWeekSummaryTemplate"/> for each week (row) of the month view.
/// </summary>
/// <typeparam name="T">The type of item displayed in the calendar.</typeparam>
public class CalendarWeekSummary<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T> where T : CalendarItem
{
    /// <summary>
    /// The week number of the displayed week.
    /// </summary>
    public int WeekNumber { get; set; }

    /// <summary>
    /// The first day shown in the week row.
    /// </summary>
    public DateTime WeekStart { get; set; }

    /// <summary>
    /// The last day shown in the week row.
    /// </summary>
    public DateTime WeekEnd { get; set; }

    /// <summary>
    /// All items that overlap the displayed week row, including multi-day items spanning into the week.
    /// </summary>
    public IReadOnlyList<T> Items { get; set; } = new List<T>();
}
