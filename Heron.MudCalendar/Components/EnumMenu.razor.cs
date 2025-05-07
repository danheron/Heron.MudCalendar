using System.Globalization;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Heron.MudCalendar;

public partial class EnumMenu<T>
{
    [Parameter]
    public Color Color { get; set; } = Color.Primary;

    [Parameter]
    public Variant Variant { get; set; } = Variant.Filled;
    
    [Parameter]
    public IEnumerable<T>? AllowedValues { get; set; }

    [Parameter]
    public T Value { get; set; } = default!;

    [Parameter]
    public EventCallback<T> ValueChanged { get; set; }
    
    private async Task ButtonClicked(T newValue)
    {
        if (!newValue.Equals(Value))
        {
            Value = newValue;
            await ValueChanged.InvokeAsync(newValue);
        }
    }
    
    private static Type Type => typeof(T);
}