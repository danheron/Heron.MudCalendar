using System.Threading.Tasks;
using FluentAssertions;
using Heron.MudCalendar.Services;
using Microsoft.JSInterop;

namespace Heron.MudCalendar.UnitTests.Services;

public class JsServiceTests : Components.BunitTest
{
    [Test]
    public async Task DisposeAsync_DoesNotThrow_WhenCircuitDisconnected()
    {
        var moduleInterop = Context.JSInterop.SetupModule("./_content/Heron.MudCalendar/Heron.MudCalendar.min.js");
        var multiSelectInterop = moduleInterop.SetupModule("newMultiSelect", _ => true);
        multiSelectInterop.SetupVoid("dispose", _ => true).SetException(new JSDisconnectedException("Circuit disconnected."));

        var service = new JsService(Context.JSInterop.JSRuntime);
        await service.AddMultiSelect(7, "container-id");

        var act = async () => await service.DisposeAsync();

        await act.Should().NotThrowAsync();
    }
}
