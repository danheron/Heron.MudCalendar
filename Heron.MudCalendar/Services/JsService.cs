using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Heron.MudCalendar.Services;

public class JsService : IDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public JsService(IJSRuntime jsRuntime)
    {
        _moduleTask = new Lazy<Task<IJSObjectReference>>(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", _cancellationTokenSource.Token, "./_content/Heron.MudCalendar/Heron.MudCalendar.min.js").AsTask());
    }

    public async Task Scroll(ElementReference element, int top)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("scroll", element, top);
    }
    
    public async Task<string> GetHeadContent()
    {
        var module = await _moduleTask.Value;
        return await module.InvokeAsync<string>("getHeadContent");
    }

    public async Task AddLink(string href, string rel)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("addLink", href, rel);
    }
    
    public async Task AddDragHandler(string id)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("addDragHandler", id);
    }

    public async Task HideOverflows(string className, string moreText)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("hideOverflows", className, moreText);
    }
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);

        if (!_moduleTask.IsValueCreated) return;
        _cancellationTokenSource.Cancel();
        _moduleTask.Value.Dispose();
    }
}