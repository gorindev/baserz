using Bunit;

using Microsoft.Extensions.DependencyInjection;

namespace BaseRz.Tests.Helpers;

public class BaseRzTestContext : BunitContext
{
    public BaseRzTestContext()
    {
        Services.AddBaseRz();
        JSInterop.Mode = JSRuntimeMode.Loose;
    }
}
