using BaseRz.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class BaseRzServiceCollectionExtensions
{
    /// <summary>
    /// Registers BaseRz services. Safe to call more than once; later calls configure the same options instance.
    /// </summary>
    public static IServiceCollection AddBaseRz(this IServiceCollection services, Action<BaseRzOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = services
            .Where(static descriptor => descriptor.ServiceType == typeof(BaseRzOptions))
            .Select(static descriptor => descriptor.ImplementationInstance)
            .OfType<BaseRzOptions>()
            .FirstOrDefault();

        if (options is null)
        {
            options = new BaseRzOptions();
            services.AddSingleton(options);
        }

        configure?.Invoke(options);
        return services;
    }
}
