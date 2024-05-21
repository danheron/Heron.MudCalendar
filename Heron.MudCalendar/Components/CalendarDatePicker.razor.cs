using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Heron.MudCalendar;

public partial class CalendarDatePicker
{
    [Parameter]
    public CalendarView View { get; set; }

    protected virtual string? DateRangeText
    {
        get
        {
            switch (View)
            {
                case CalendarView.Day:
                    return Date?.ToString("dd MMM yyyy");

                case CalendarView.Week:
                case CalendarView.WorkWeek:
                    if (!Date.HasValue) return null;
                    var range = new CalendarDateRange(Date.Value, View, FirstDayOfWeek);
                    return range.End != null && range.Start != null && range.Start.Value.Month == range.End.Value.Month ? 
                        $"{range.Start:dd} - {range.End:dd} {range.End.Value:MMM yyyy}" : 
                        $"{range.Start:dd} {range.Start:MMM} - {range.End:dd} {range.End?.ToString("MMM yyyy")}";
                
                case CalendarView.Month:
                default:
                    return Date?.ToString("MMMM yyyy");
            }
        }
    }
}