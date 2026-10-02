using BaseRz.Core;

using Microsoft.Extensions.DependencyInjection;

namespace BaseRz.Tests.DependencyInjection;

public class ServiceCollectionTests
{
    [Fact]
    public void AddBaseRz_RegistersRequiredServices()
    {
        var services = new ServiceCollection();

        var returned = services.AddBaseRz(options => options.DefaultDirection = Direction.Rtl);
        services.AddBaseRz();

        Assert.Same(services, returned);
        Assert.Single(services, static descriptor => descriptor.ServiceType == typeof(BaseRzOptions));

        using var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<BaseRzOptions>();
        Assert.Equal(Direction.Rtl, options.DefaultDirection);
        Assert.Same(options, provider.GetRequiredService<BaseRzOptions>());
    }

    [Fact]
    public void AddBaseRz_DefaultsToLtr()
    {
        using var provider = new ServiceCollection().AddBaseRz().BuildServiceProvider();

        Assert.Equal(Direction.Ltr, provider.GetRequiredService<BaseRzOptions>().DefaultDirection);
    }
}
