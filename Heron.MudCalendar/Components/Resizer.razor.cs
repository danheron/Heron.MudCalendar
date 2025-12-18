using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Utilities;

namespace Heron.MudCalendar;

public partial class Resizer : IAsyncDisposable
{
    private readonly string _id = Guid.NewGuid().ToString();

    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private IJSObjectReference? _resizer;
    private DotNetObjectReference<Resizer>? _this;

    [Parameter]
    public bool ResizeX { get; set; }

    [Parameter]
    public string ContainerClass { get; set; } = string.Empty;
    
    [Parameter]
    public int CellCount { get; set; }

    [Parameter]
    public EventCallback<int> SizeChanged { get; set; }

    private string Classname =>
        new CssBuilder()
            .AddClass("mud-cal-resizer-x", ResizeX)
            .AddClass("mud-cal-resizer-y", !ResizeX)
            .Build();

    public Resizer()
    {
        _moduleTask = new Lazy<Task<IJSObjectReference>>(() => JsRuntime.InvokeAsync<IJSObjectReference>(
            "import", _cancellationTokenSource.Token, "./_content/Heron.MudCalendar/Heron.MudCalendar.min.js").AsTask());
    }
    
    public override async Task SetParametersAsync(ParameterView parameters)
    {
        // Check if the CellCount parameter is changing
        parameters.TryGetValue(nameof(CellCount), out int cellCount);
        var needUpdate = cellCount != CellCount;

        // Update parameters
        await base.SetParametersAsync(parameters);
        
        // If the CellCount has changed, then the JS object needs to be updated
        if (needUpdate && _resizer != null)
        {
            await _resizer.InvokeVoidAsync("updateCellCount", cellCount);
        }
    }

    [JSInvokable]
    public Task ResizeFinished(int newSize)
    {
        return SizeChanged.InvokeAsync(newSize);
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
        _resizer = await module.InvokeAsync<IJSObjectReference>("newResizer", _id, ContainerClass, CellCount, ResizeX, _this);
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        _this?.Dispose();
        _this = null;
        if (_resizer != null)
        {
            await _resizer.DisposeAsync();
            _resizer = null;
        }

        if (_moduleTask.IsValueCreated)
        {
            await _cancellationTokenSource.CancelAsync();
            _moduleTask.Value.Dispose();
        }
    }
}