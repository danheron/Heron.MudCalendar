using Heron.MudCalendar.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace Heron.MudCalendar;

public partial class WeekDropZone : IAsyncDisposable
{
    private string _id = Guid.NewGuid().ToString();

    private JsService? _jsService;
    
    [Parameter]
    public CalendarItem? Item { get; set; }
    
    [Parameter]
    public string? Style { get; set; }

    private string Classname =>
        new CssBuilder("mud-cal-week-drop-item")
            .AddClass($"mud-cal-{_id}")
            .Build();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            _jsService ??= new JsService(JsRuntime);
            await _jsService.AddDragHandler($"mud-cal-{_id}");
        }
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        
        if (_jsService != null)
        {
            await _jsService.DisposeAsync();
        }
    }
}