using Heron.MudCalendar.Enums;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;

namespace Heron.MudCalendar;

public partial class EnumSwitch<T>
{
    [Parameter]
    public Color Color { get; set; } = Color.Primary;

    [Parameter]
    public IEnumerable<T>? AllowedValues { get; set; }

    [Parameter]
    public T Value { get; set; } = default!;

    [Parameter]
    public EventCallback<T> ValueChanged { get; set; }

    [Parameter]
    public Variant Variant { get; set; } = Variant.Filled;

    private bool _isTransparent => Variant == Variant.Text;

    private async Task ButtonClicked(T newValue)
    {
        if (!newValue.Equals(Value))
        {
            Value = newValue;
            await ValueChanged.InvokeAsync(newValue);
        }
    }

    private string Classname =>
        new CssBuilder("mud-width-full")
            .AddClass("justify-end")
            .AddClass("d-flex", AllowedValues == null || AllowedValues.Count() > 1)
            .AddClass("d-none", AllowedValues != null && AllowedValues.Count() <= 1)
            .Build();

    private static Type Type => typeof(T);

    private static string? Label(T value)
    {
        return typeof(T) == typeof(CalendarView) ?
            // Bit of a hack as generic types don't work with StringLocalizer
            @CalendarViewName.GetText((CalendarView)Enum.ToObject(Type, value)) :
            @Enum.GetName(Type, value);
    }
}