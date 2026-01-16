using System.Globalization;
using MudBlazor;

namespace Heron.MudCalendar;

public class CalendarDateRange : DateRange
{
    public CalendarDateRange(DateTime currentDay, CalendarView view, CultureInfo culture, DayOfWeek? firstDayOfWeek = null)
        : base(GetStart(view, currentDay, culture, firstDayOfWeek), GetEnd(view, currentDay, culture, firstDayOfWeek))
    {
    }

    private static DateTime GetStart(CalendarView view, DateTime currentDay, CultureInfo culture, DayOfWeek? firstDayOfWeek)
    {
        switch (view)
        {
            case CalendarView.Day:
                return currentDay.Date;
            case CalendarView.Week:
            case CalendarView.WorkWeek:
                return GetFirstWeekDate(currentDay, culture, firstDayOfWeek);
            case CalendarView.Month:
            default:
                return GetFirstMonthDate(currentDay, culture, firstDayOfWeek);
        }
    }

    private static DateTime GetEnd(CalendarView view, DateTime currentDay, CultureInfo culture, DayOfWeek? firstDayOfWeek)
    {
        switch (view)
        {
            case CalendarView.Day:
                return currentDay.Date;
            case CalendarView.Week:
                return GetLastWeekDate(currentDay, culture, firstDayOfWeek);
            case CalendarView.WorkWeek:
                return GetLastWorkWeekDate(currentDay, culture, firstDayOfWeek);
            case CalendarView.Month:
            default:
                return GetLastMonthDate(currentDay, culture, firstDayOfWeek);
        }
    }

    private static DateTime GetFirstMonthDate(DateTime day, CultureInfo culture, DayOfWeek? firstDayOfWeek)
    {
        // Get the year and month in the target calendar system
        var year = culture.Calendar.GetYear(day);
        var month = culture.Calendar.GetMonth(day);

        // Get the first day of the month in the target calendar
        var firstDayOfMonth = culture.Calendar.ToDateTime(year, month, 1, 0, 0, 0, 0);

        // Adjust to the start of the week
        firstDayOfMonth = firstDayOfMonth.AddDays(GetDayOfWeek(firstDayOfMonth, culture, firstDayOfWeek) * -1);

        return firstDayOfMonth;
    }

    private static DateTime GetLastMonthDate(DateTime day, CultureInfo culture, DayOfWeek? firstDayOfWeek)
    {

        // Get the year and month in the target calendar system
        var year = culture.Calendar.GetYear(day);
        var month = culture.Calendar.GetMonth(day);

        // Get the number of days in this month
        var daysInMonth = culture.Calendar.GetDaysInMonth(year, month);

        // Get the last day of the month in the target calendar
        var lastDayOfMonth = culture.Calendar.ToDateTime(year, month, daysInMonth, 0, 0, 0, 0);

        // Adjust to the end of the week
        lastDayOfMonth = lastDayOfMonth.AddDays(6 - GetDayOfWeek(lastDayOfMonth, culture, firstDayOfWeek));

        return lastDayOfMonth;
    }

    public static DateTime GetFirstWeekDate(DateTime day, CultureInfo culture, DayOfWeek? firstDayOfWeek)
    {
        // Get the first day of the week
        return day.AddDays(GetDayOfWeek(day, culture, firstDayOfWeek) * -1);
    }

    public static DateTime GetLastWeekDate(DateTime day, CultureInfo culture, DayOfWeek? firstDayOfWeek)
    {
        // Get the last day of the week
        return day.AddDays(6 - GetDayOfWeek(day, culture, firstDayOfWeek));
    }

    public static DateTime GetLastWorkWeekDate(DateTime day, CultureInfo culture, DayOfWeek? firstDayOfWeek)
    {
        // Get the last day of the work week
        return day.AddDays(4 - GetDayOfWeek(day, culture, firstDayOfWeek));
    }

    public static int GetDayOfWeek(DateTime date, CultureInfo culture, DayOfWeek? firstDayOfWeek = null)
    {
        // Get day as an integer. First day of the week = 0, last day = 6
        var firstDay = firstDayOfWeek ?? culture.DateTimeFormat.FirstDayOfWeek;
        var day = (int)date.DayOfWeek;
        day -= (int)firstDay;
        if (day < 0)
        {
            day = 7 + day;
        }

        return day;
    }
}