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
                Start = GetFirstWeekDate();
                break;
            case CalendarView.Month:
            default:
                Start = GetFirstMonthDate();
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
                End = GetLastWeekDate();
                break;
            case CalendarView.Month:
            default:
                End = GetLastMonthDate();
                break;
        }
    }
    
    private DateTime GetFirstMonthDate()
    {
        // Get first day or the week for the first day of the month
        var date = new DateTime(_currentDay.Year, _currentDay.Month, 1);
        date = date.AddDays(GetDayOfWeek(date) * -1);
        return date;
    }

    private DateTime GetLastMonthDate()
    {
        // Get the last day of the week for the last day of the month
        var date = _currentDay.AddMonths(1);
        date = new DateTime(date.Year, date.Month, 1).AddDays(-1);
        date = date.AddDays(6 - GetDayOfWeek(date));
        return date;
    }

    private DateTime GetFirstWeekDate()
    {
        // Get first day of the week
        return _currentDay.AddDays(GetDayOfWeek(_currentDay) * -1);
    }

    private DateTime GetLastWeekDate()
    {
        // Get last day of the week
        return _currentDay.AddDays(6 - GetDayOfWeek(_currentDay));
    }

    private static int GetDayOfWeek(DateTime date)
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