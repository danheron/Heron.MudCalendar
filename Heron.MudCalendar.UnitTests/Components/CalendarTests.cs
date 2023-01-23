using Heron.MudCalendar.UnitTests.Viewer.TestComponents.Calendar;

namespace Heron.MudCalendar.UnitTests.Components;

public class CalendarTests : BunitTest
{
    [Test]
    public void SimpleTest()
    {
        var cut = Context.RenderComponent<CalendarTest>();

        var div = cut.Find("div.mud-calendar");
    }
}
