using MudBlazor;
using System.Diagnostics.CodeAnalysis;

namespace Heron.MudCalendar.UnitTests.Viewer.Services;

public class EventService<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T> where T:CalendarItem, new()
{
    private readonly Dictionary<DateTime, List<T>> _items = new();

    public List<T> GetEvents(DateRange dateRange)
    {
        if (dateRange.Start == null || dateRange.End == null) return new List<T>();

        var events = new List<T>();
        var month = new DateTime(dateRange.Start.Value.Year, dateRange.Start.Value.Month, 1);
        while (month <= dateRange.End.Value)
        {
            if (!_items.ContainsKey(month))
            {
                CreateEvents(month);
            }

            events.AddRange(_items[month]);

            month = month.AddMonths(1);
        }

        return events;
    }

    private void CreateEvents(DateTime month)
    {
        // Create 20 dummy events
        var events = new List<T>();
        var days = DateTime.DaysInMonth(month.Year, month.Month);
        var rnd = new Random();
        for (var i = 0; i < 20; i++)
        {
            var day = rnd.Next(1, days);
            var hour = rnd.Next(8, 16);
            var duration = rnd.Next(1, 8) / 2.0;

            var item = new T
            {
                Start = new DateTime(month.Year, month.Month, day, hour, 0, 0)
            };
            item.End = item.Start.AddHours(duration);
            item.Text = $"{item.Start.Day}_{item.Start.Month}";
            events.Add(item);
        }

        // Store in dictionary
        _items.Add(month, events);
    }
}