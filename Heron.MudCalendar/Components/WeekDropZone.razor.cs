using Heron.MudCalendar.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using System.Diagnostics.CodeAnalysis;

namespace Heron.MudCalendar;

public partial class WeekDropZone<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T> : IDisposable where T : CalendarItem
{
    [CascadingParameter]
    public MudCalendar<T> Calendar { get; set; } = new();
    
    private readonly string _id = Guid.NewGuid().ToString();

    private JsService? _jsService;
    
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    
    [Parameter]
    public ItemPosition<T>? Position { get; set; }
    
    [Parameter]
    public string? Style { get; set; }

    private string Classname =>
        new CssBuilder("mud-cal-drop-item")
            .AddClass($"mud-cal-{_id}")
            .Build();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        
        if (firstRender)
        {
            _jsService ??= new JsService(JsRuntime);
            await _jsService.AddDragHandler($"mud-cal-{_id}", Position?.Width ?? 1);
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        _jsService?.Dispose();
    }
}