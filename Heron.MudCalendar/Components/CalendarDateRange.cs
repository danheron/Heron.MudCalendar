using System.Globalization;
using MudBlazor;

namespace Heron.MudCalendar;

public class CalendarDateRange : DateRange
{
    public CalendarView View { get; }

    private readonly DateTime _currentDay;
    private static CultureInfo _culture = CultureInfo.InvariantCulture;
    private readonly Calendar _calendar;

    public CalendarDateRange(DateTime currentDay, CalendarView view, CultureInfo culture, DayOfWeek? firstDayOfWeek = null)
    {
        _culture = culture;
        _calendar = culture.Calendar;
        _currentDay = currentDay;
        View = view;
        
        SetStart(firstDayOfWeek);
        SetEnd(firstDayOfWeek);
    }

    private void SetStart(DayOfWeek? firstDayOfWeek)
    {
        switch (View)
        {
            case CalendarView.Day:
                Start = _currentDay.Date;
                break;
            case CalendarView.Week:
            case CalendarView.WorkWeek:
                Start = GetFirstWeekDate(_currentDay, firstDayOfWeek);
                break;
            case CalendarView.Month:
            default:
                Start = GetFirstMonthDate(_currentDay, firstDayOfWeek);
                break;
        }
    }

    private void SetEnd(DayOfWeek? firstDayOfWeek)
    {
        switch (View)
        {
            case CalendarView.Day:
                End = _currentDay.Date;
                break;
            case CalendarView.Week:
                End = GetLastWeekDate(_currentDay, firstDayOfWeek);
                break;
            case CalendarView.WorkWeek:
                End = GetLastWorkWeekDate(_currentDay, firstDayOfWeek);
                break;
            case CalendarView.Month:
            default:
                End = GetLastMonthDate(_currentDay, firstDayOfWeek);
                break;
        }
    }
    
    public DateTime GetFirstMonthDate(DateTime day, DayOfWeek? firstDayOfWeek)
    {
        // Get the year and month in the target calendar system
        int year = _calendar.GetYear(day);
        int month = _calendar.GetMonth(day);

        // Get the first day of the month in the target calendar
        DateTime firstDayOfMonth = _calendar.ToDateTime(year, month, 1, 0, 0, 0, 0);

        // Adjust to the start of the week
        firstDayOfMonth = firstDayOfMonth.AddDays(GetDayOfWeek(firstDayOfMonth, firstDayOfWeek) * -1);

        return firstDayOfMonth;
    }
    
    public DateTime GetLastMonthDate(DateTime day, DayOfWeek? firstDayOfWeek)
    {
        
        // Get the year and month in the target calendar system
        int year = _calendar.GetYear(day);
        int month = _calendar.GetMonth(day);

        // Get the number of days in this month
        int daysInMonth = _calendar.GetDaysInMonth(year, month);

        // Get the last day of the month in the target calendar
        DateTime lastDayOfMonth = _calendar.ToDateTime(year, month, daysInMonth, 0, 0, 0, 0);

        // Adjust to the end of the week
        lastDayOfMonth = lastDayOfMonth.AddDays(6 - GetDayOfWeek(lastDayOfMonth, firstDayOfWeek));

        return lastDayOfMonth;
    }

    public static DateTime GetFirstWeekDate(DateTime day, DayOfWeek? firstDayOfWeek)
    {
        // Get first day of the week
        return day.AddDays(GetDayOfWeek(day, firstDayOfWeek) * -1);
    }

    public static DateTime GetLastWeekDate(DateTime day, DayOfWeek? firstDayOfWeek)
    {
        // Get last day of the week
        return day.AddDays(6 - GetDayOfWeek(day, firstDayOfWeek));
    }

    public static DateTime GetLastWorkWeekDate(DateTime day, DayOfWeek? firstDayOfWeek)
    {
        // Get last day of the work week
        return day.AddDays(4 - GetDayOfWeek(day, firstDayOfWeek));
    }

    public static int GetDayOfWeek(DateTime date, DayOfWeek? firstDayOfWeek = null)
    {
        // Get day as integer - first day of week = 0 .. last day = 6
        var firstDay = firstDayOfWeek ?? _culture.DateTimeFormat.FirstDayOfWeek;
        var day = (int)date.DayOfWeek;
        day -= (int)firstDay;
        if (day < 0)
        {
            day = 7 + day;
        }

        return day;
    }
}