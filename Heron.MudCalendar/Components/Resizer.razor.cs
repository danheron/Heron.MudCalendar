using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Heron.MudCalendar;

public partial class Resizer : IAsyncDisposable
{
    private readonly string _id = Guid.NewGuid().ToString();

    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;
    private IJSObjectReference? _resizer;
    private DotNetObjectReference<Resizer>? _this;
    
    [Parameter]
    public EventCallback<int> HeightChanged { get; set; }

    public Resizer()
    {
        _moduleTask = new Lazy<Task<IJSObjectReference>>(
            () =>
                JsRuntime.InvokeAsync<IJSObjectReference>("import",
                    "./_content/Heron.MudCalendar/Heron.MudCalendar.min.js").AsTask());
    }

    [JSInvokable]
    public Task ResizeFinished(int newHeight)
    {
        return HeightChanged.InvokeAsync(newHeight);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            await CreateResizer();
        }
    }

    private async Task CreateResizer()
    {
        _this ??= DotNetObjectReference.Create(this);
        var module = await _moduleTask.Value;
        _resizer = await module.InvokeAsync<IJSObjectReference>("newResizer", _id, 36, _this);
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        _this?.Dispose();
        if (_resizer != null)
        {
            await _resizer.DisposeAsync();
        }
        if (_moduleTask.IsValueCreated)
        {
            var module = await _moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}