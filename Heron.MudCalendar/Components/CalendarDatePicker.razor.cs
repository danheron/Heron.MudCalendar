using Microsoft.AspNetCore.Components;

namespace Heron.MudCalendar;

public partial class CalendarDatePicker
{
    [Parameter]
    public CalendarView View { get; set; }
   
    protected virtual string? DateRangeText
    {
        get
        {
            if (!Date.HasValue) return null;

            switch (View)
            {
                case CalendarView.Day:
                    return Date.Value.ToString("d MMM yyyy", Culture);

                case CalendarView.Week:
                case CalendarView.WorkWeek:
                    var range = new CalendarDateRange(Date.Value, View, Culture, FirstDayOfWeek);
                    if (range.End == null || range.Start == null) return null;

                    if (range.Start.Value.Month == range.End.Value.Month)
                    {
                        return string.Format(Culture,
                            "{0:dd} - {1:dd} {1:MMM yyyy}",
                            range.Start, range.End);
                    }
                    else
                    {
                        return string.Format(Culture,
                            "{0:dd MMM} - {1:dd MMM yyyy}",
                            range.Start, range.End);
                    }

                case CalendarView.Month:
                default:
                    return Date.Value.ToString("MMMM yyyy", Culture);
            }
        }
    }
}