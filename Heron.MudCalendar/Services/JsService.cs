using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Heron.MudCalendar.Services;

public class JsService : IDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private DotNetObjectReference<JsService>? _this;

    public event EventHandler? OnLinkLoaded;

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
        if (OnLinkLoaded != null)
        {
            _this ??= DotNetObjectReference.Create(this);
        }
        
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("addLink", href, rel, _this);
    }

    [JSInvokable]
    public void LinkLoaded()
    {
        OnLinkLoaded?.Invoke(this, EventArgs.Empty);
    }
    
    public async Task AddDragHandler(string id, int width)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("addDragHandler", id, width);
    }

    public async Task PositionMonthItems(ElementReference element, string moreText, bool fixedHeight)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("positionMonthItems", element, moreText, fixedHeight);
    }
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);

        if (!_moduleTask.IsValueCreated) return;
        _cancellationTokenSource.Cancel();
        _moduleTask.Value.Dispose();
    }
}