using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;

namespace Heron.MudCalendar;

public partial class EnumSwitch<T>
{
    private T _value = default!;

    [Parameter]
    public Color Color { get; set; } = Color.Primary;
    
    [Parameter]
    public IEnumerable<T>? AllowedValues { get; set; }

    [Parameter]
    public T Value
    {
        get => _value;
        set
        {
            if (_value.Equals(value)) return;
            _value = value;
            ValueChanged.InvokeAsync(value);
        }
    }

    [Parameter]
    public EventCallback<T> ValueChanged { get; set; }

    private string Classname =>
        new CssBuilder("mud-width-full")
            .AddClass("justify-end")
            .AddClass("d-flex", AllowedValues == null || AllowedValues.Count() > 1)
            .AddClass("d-none", AllowedValues != null && AllowedValues.Count() <= 1)
            .Build();

    private Type Type => typeof(T);
}