using Microsoft.AspNetCore.Components.Web;

namespace Heron.MudCalendar;

/// <summary>
/// Represents the information related to a <see cref="MudCalendar{T}.ItemContextMenuClicked"/> event.
/// </summary>
/// <typeparam name="T">The type of calendar item.</typeparam>
public class CalendarItemClickEventArgs<T> : EventArgs where T : CalendarItem
{
    /// <summary>
    /// The coordinates of the pointer for this click.
    /// </summary>
    public MouseEventArgs MouseEventArgs { get; }

    /// <summary>
    /// The calendar item that was clicked.
    /// </summary>
    public T Item { get; }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="mouseEventArgs">The coordinates of the pointer for this click.</param>
    /// <param name="item">The calendar item that was clicked.</param>
    public CalendarItemClickEventArgs(MouseEventArgs mouseEventArgs, T item)
    {
        MouseEventArgs = mouseEventArgs;
        Item = item;
    }
}