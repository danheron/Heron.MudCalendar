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
                    return Date.Value.ToString("d MMM yyyy", GetCulture());

                case CalendarView.Week:
                case CalendarView.WorkWeek:
                    var range = new CalendarDateRange(Date.Value, View, GetCulture(), FirstDayOfWeek);
                    if (range.End == null || range.Start == null) return null;

                    if (GetCulture().Calendar.GetMonth(range.Start.Value) == GetCulture().Calendar.GetMonth(range.End.Value))
                    {
                        return string.Format(GetCulture(),
                            "{0:dd} - {1:dd} {1:MMM yyyy}",
                            range.Start, range.End);
                    }
                    else
                    {
                        return string.Format(GetCulture(),
                            "{0:dd MMM} - {1:dd MMM yyyy}",
                            range.Start, range.End);
                    }

                case CalendarView.Month:
                default:
                    return Date.Value.ToString("MMMM yyyy", GetCulture());
            }
        }
    }
}