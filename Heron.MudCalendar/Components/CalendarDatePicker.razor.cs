using Microsoft.AspNetCore.Components;
using System.Globalization;

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

            var culture = Culture ?? CultureInfo.CurrentCulture;
            var dateTimeFormat = culture.DateTimeFormat;

            switch (View)
            {
                case CalendarView.Day:
                    return Date.Value.ToString("d MMM yyyy", culture);

                case CalendarView.Week:
                case CalendarView.WorkWeek:
                    var range = new CalendarDateRange(Date.Value, View, culture, FirstDayOfWeek);
                    if (range.End == null || range.Start == null) return null;

                    if (range.Start.Value.Month == range.End.Value.Month)
                    {
                        return string.Format(culture,
                            "{0:d} - {1:d} {1:MMM yyyy}",
                            range.Start, range.End);
                    }
                    else
                    {
                        return string.Format(culture,
                            "{0:d MMM} - {1:d MMM yyyy}",
                            range.Start, range.End);
                    }

                case CalendarView.Month:
                default:
                    return Date.Value.ToString("MMMM yyyy", culture);
            }
        }
    }
}