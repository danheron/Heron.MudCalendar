using System.Globalization;
using MudBlazor;

namespace Heron.MudCalendar;

public class CalendarDateRange : DateRange
{
    public CalendarView View { get; }

    private readonly DateTime _currentDay;

    public CalendarDateRange(DateTime currentDay, CalendarView view)
    {
        _currentDay = currentDay;
        View = view;
        
        SetStart();
        SetEnd();
    }

    private void SetStart()
    {
        switch (View)
        {
            case CalendarView.Day:
                Start = _currentDay.Date;
                break;
            case CalendarView.Week:
                Start = GetFirstWeekDate(_currentDay);
                break;
            case CalendarView.Month:
            default:
                Start = GetFirstMonthDate(_currentDay);
                break;
        }
    }

    private void SetEnd()
    {
        switch (View)
        {
            case CalendarView.Day:
                End = _currentDay.Date;
                break;
            case CalendarView.Week:
                End = GetLastWeekDate(_currentDay);
                break;
            case CalendarView.Month:
            default:
                End = GetLastMonthDate(_currentDay);
                break;
        }
    }
    
    public static DateTime GetFirstMonthDate(DateTime day)
    {
        // Get first day or the week for the first day of the month
        var date = new DateTime(day.Year, day.Month, 1);
        date = date.AddDays(GetDayOfWeek(date) * -1);
        return date;
    }

    public static DateTime GetLastMonthDate(DateTime day)
    {
        // Get the last day of the week for the last day of the month
        var date = day.AddMonths(1);
        date = new DateTime(date.Year, date.Month, 1).AddDays(-1);
        date = date.AddDays(6 - GetDayOfWeek(date));
        return date;
    }

    public static DateTime GetFirstWeekDate(DateTime day)
    {
        // Get first day of the week
        return day.AddDays(GetDayOfWeek(day) * -1);
    }

    public static DateTime GetLastWeekDate(DateTime day)
    {
        // Get last day of the week
        return day.AddDays(6 - GetDayOfWeek(day));
    }

    public static int GetDayOfWeek(DateTime date)
    {
        // Get day as integer - first day of week = 0 .. last day = 6
        var firstDay = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
        var day = (int)date.DayOfWeek;
        day -= (int)firstDay;
        if (day < 0)
        {
            day = 7 + day;
        }

        return day;
    }
}