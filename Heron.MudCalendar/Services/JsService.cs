using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Heron.MudCalendar.Services;

public class JsService : IDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private DotNetObjectReference<JsService>? _this;

    public event EventHandler? OnLinkLoaded;

    public event EventHandler<DateTime>? OnMoreClicked;

    public event Func<object?, IEnumerable<(DateTime date, int row)>, Task>? OnCellsSelected;

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
        if (OnMoreClicked != null)
        {
            _this ??= DotNetObjectReference.Create(this);
        }
        
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("positionMonthItems", element, moreText, fixedHeight, _this);
    }

    [JSInvokable]
    public void MoreClicked(string date)
    {
        var moreDate = DateTime.Parse(date);
        OnMoreClicked?.Invoke(this, moreDate);
    }

    /// <summary>
    /// Initializes the multi-select functionality for the calendar grid.
    /// </summary>
    /// <param name="cellsInDay">The number of cells per day in the calendar view.</param>
    /// <param name="containerId">The DOM id of the calendar container (e.g., "calendar1-grid").</param>
    public async Task AddMultiSelect(int cellsInDay, string containerId)
    {
        // Create a .NET object reference for JS interop if it doesn't exist yet
        _this ??= DotNetObjectReference.Create(this);

        // Wait for the JS module to be loaded
        var module = await _moduleTask.Value;

        // Invoke the JavaScript function "addMultiSelect", passing the number of cells per day
        // and the .NET object reference for callbacks from JS to C#
        await module.InvokeVoidAsync("addMultiSelect", cellsInDay, containerId, _this);
    }

    [JSInvokable]
    public void CellsSelected(List<string> cellKeys)
    {
        // cellKeys are like "2025-08-15_12"
        var selected = cellKeys.Select(key =>
        {
            var parts = key.Split('_');
            var date = DateTime.Parse(parts[0]);
            var row = int.Parse(parts[1]);
            return (date, row);
        }).OrderBy(x => x.date).ThenBy(x => x.row).ToList();

        OnCellsSelected?.Invoke(this, selected);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        if (!_moduleTask.IsValueCreated) return;
        _cancellationTokenSource.Cancel();
        _moduleTask.Value.Dispose();
    }
}