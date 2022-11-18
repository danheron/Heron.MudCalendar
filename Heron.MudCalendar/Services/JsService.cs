using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Heron.MudCalendar.Services;

internal class JsService : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

    public JsService(IJSRuntime jsRuntime)
    {
        _moduleTask = new Lazy<Task<IJSObjectReference>>(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/Heron.MudCalendar/Heron.MudCalendar.js").AsTask());
    }

    public async Task Scroll(ElementReference element, int top)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("scroll", element, top);
    }
    
    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        
        if (_moduleTask.IsValueCreated)
        {
            var module = await _moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}