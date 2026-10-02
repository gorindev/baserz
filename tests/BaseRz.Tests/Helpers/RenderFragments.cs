using Microsoft.AspNetCore.Components;

namespace BaseRz.Tests.Helpers;

public static class RenderFragments
{
    public static RenderFragment Text(string text) => builder => builder.AddContent(0, text);
}
