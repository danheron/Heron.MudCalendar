using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.JSInterop;

namespace Heron.MudCalendar.UnitTests.Components;

public class ResizerTests : BunitTest
{
    [Test]
    public async Task DisposeAsync_DoesNotThrow_WhenCircuitDisconnected()
    {
        var moduleInterop = Context.JSInterop.SetupModule("./_content/Heron.MudCalendar/Heron.MudCalendar.min.js");
        var resizerInterop = moduleInterop.SetupModule("newResizer", _ => true);
        resizerInterop.SetupVoid("dispose", _ => true).SetException(new JSDisconnectedException("Circuit disconnected."));

        var cut = Context.Render<Resizer>();

        var act = async () => await cut.Instance.DisposeAsync();

        await act.Should().NotThrowAsync();
    }
}
