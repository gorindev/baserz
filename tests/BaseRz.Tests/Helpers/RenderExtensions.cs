using Bunit;

using Microsoft.AspNetCore.Components;

namespace BaseRz.Tests.Helpers;

public static class RenderExtensions
{
    public static IRenderedComponent<TComponent> RenderWithCascading<TComponent, TValue>(
        this BunitContext context,
        TValue value,
        Action<ComponentParameterCollectionBuilder<TComponent>>? configure = null)
        where TComponent : IComponent
    {
        return context.Render<TComponent>(parameters =>
        {
            parameters.AddCascadingValue(value);
            configure?.Invoke(parameters);
        });
    }
}
