using System.Globalization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Heron.MudCalendar.Enums;

public static class CalendarViewName
{
    private static Dictionary<CalendarView, string>? _viewNames;
    private static CultureInfo? _uiCulture;

    public static string GetText(CalendarView view)
    {
        if (_viewNames == null || _uiCulture == null || !Equals(_uiCulture, CultureInfo.CurrentUICulture)) ReadLocalizedNames();
        return _viewNames == null ? string.Empty : _viewNames[view];
    }

    private static void ReadLocalizedNames()
    {
        var options = Options.Create(new LocalizationOptions { ResourcesPath = "Resources" });
        var factory = new ResourceManagerStringLocalizerFactory(options, NullLoggerFactory.Instance);
        var localizer = new StringLocalizer<CalendarView>(factory);

        _uiCulture = CultureInfo.CurrentUICulture;
        
        _viewNames = new Dictionary<CalendarView, string>
        {
            [CalendarView.Month] = localizer["Month"],
            [CalendarView.Week] = localizer["Week"],
            [CalendarView.WorkWeek] = localizer["WorkWeek"],
            [CalendarView.Day] = localizer["Day"]
        };
    }
}