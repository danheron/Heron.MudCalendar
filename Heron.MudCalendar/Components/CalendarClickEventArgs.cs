using System;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor;

#nullable enable

/// <summary>
/// Represents the information related to a <see cref="MudCalendar.CellContextMenuClicked"/> event.
/// </summary>
public class CalendarClickEventArgs : EventArgs
{
    /// <summary>
    /// The coordinates of the pointer for this click.
    /// </summary>
    public MouseEventArgs MouseEventArgs { get; }

    /// <summary>
    /// The date and time corresponding to the cell which was clicked.
    /// </summary>
    public DateTime DateTime { get; }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="mouseEventArgs">The coordinates of the pointer for this click.</param>
    /// <param name="dateTime">The date and time corresponding to the cell which was clicked.</param>
    public CalendarClickEventArgs(MouseEventArgs mouseEventArgs, DateTime dateTime)
    {
        MouseEventArgs = mouseEventArgs;
        DateTime = dateTime;
    }
}
