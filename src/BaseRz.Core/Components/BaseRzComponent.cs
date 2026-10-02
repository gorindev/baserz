using BaseRz.Core.Utilities;

using Microsoft.AspNetCore.Components;

namespace BaseRz.Core.Components;

/// <summary>
/// Shared base for BaseRz roots and parts: attribute pass-through, child content, polymorphic element via
/// <see cref="As"/>, a stable per-instance id, and tracking of which parameters the caller supplied.
/// </summary>
public abstract class BaseRzComponent : ComponentBase
{
    private HashSet<string> _suppliedParameters = new(StringComparer.Ordinal);
    private string? _generatedId;

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }

    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Overrides the rendered element name (for example <c>"a"</c> or <c>"section"</c>).</summary>
    [Parameter] public string? As { get; set; }

    protected virtual string DefaultElement => "div";

    protected string Element => string.IsNullOrWhiteSpace(As) ? DefaultElement : As;

    protected virtual string IdPrefix => "baserz";

    /// <summary>The caller's <c>id</c> attribute when supplied, otherwise an id generated once per instance.</summary>
    protected string Id =>
        AdditionalAttributes is not null
        && AdditionalAttributes.TryGetValue("id", out var value)
        && value is string callerId
        && !string.IsNullOrWhiteSpace(callerId)
            ? callerId
            : _generatedId ??= IdGenerator.Next(IdPrefix);

    protected string? CallerClass => Css.CallerClass(AdditionalAttributes);

    protected Dictionary<string, object>? AttributesWithoutClass => Css.AttributesWithoutClass(AdditionalAttributes);

    public override Task SetParametersAsync(ParameterView parameters)
    {
        var supplied = new HashSet<string>(StringComparer.Ordinal);
        foreach (var parameter in parameters)
        {
            if (!parameter.Cascading)
            {
                supplied.Add(parameter.Name);
            }
        }

        _suppliedParameters = supplied;
        return base.SetParametersAsync(parameters);
    }

    /// <summary>
    /// Whether the parent passed <paramref name="parameterName"/> in the latest parameter set. Used to tell
    /// controlled (<c>Open</c> supplied) from uncontrolled (<c>DefaultOpen</c> only) usage.
    /// </summary>
    protected bool IsParameterSet(string parameterName) => _suppliedParameters.Contains(parameterName);
}
