using BaseRz.Core.Components;
using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace BaseRz.Tests.Helpers.Probes;

public sealed record ProbeCascade(string Name);

public sealed class StatefulProbe : BaseRzComponent
{
    private readonly ControllableState<bool> _open = new();

    [CascadingParameter] public ProbeCascade? Cascade { get; set; }

    [Parameter] public bool Open { get; set; }

    [Parameter] public EventCallback<bool> OpenChanged { get; set; }

    [Parameter] public bool DefaultOpen { get; set; }

    [Parameter] public bool Disabled { get; set; }

    public bool IsOpen => _open.Value;

    public bool IsControlled => _open.IsControlled;

    public bool WasSet(string parameterName) => IsParameterSet(parameterName);

    protected override string DefaultElement => "button";

    protected override void OnParametersSet()
    {
        _open.Sync(IsParameterSet(nameof(Open)), Open, DefaultOpen, OpenChanged);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Element);
        builder.AddMultipleAttributes(1, AdditionalAttributes);
        builder.AddDataState(2, _open.Value);
        builder.AddDataDisabled(3, Disabled);
        if (Cascade is not null)
        {
            builder.AddAttribute(4, "data-cascade", Cascade.Name);
        }

        builder.AddAttribute(5, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, ToggleAsync));
        builder.AddAttribute(6, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, HandleKeyDownAsync));
        builder.AddContent(7, ChildContent);
        builder.CloseElement();
    }

    private Task HandleKeyDownAsync(KeyboardEventArgs args) =>
        args.Key is "Enter" or " " ? ToggleAsync() : Task.CompletedTask;

    private async Task ToggleAsync()
    {
        if (Disabled)
        {
            return;
        }

        await _open.SetAsync(!_open.Value);
    }
}
