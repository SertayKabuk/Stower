using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Reflection;

namespace Stower
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStower(this IServiceCollection services, Action<IServiceProvider, StowerOptions> configureOptions, params Assembly[] assembliesToScan)
        {
            services.AddOptions<StowerOptions>().Configure<IServiceProvider>((options, resolver) => configureOptions(resolver, options));

            services.TryAddSingleton<ServiceFactory>(p => p.GetService);
            services.TryAddSingleton(typeof(IStower), typeof(Stower));

            ServiceRegistrar.AddStowerClasses(services, assembliesToScan);

            return services;
        }

        public static IServiceCollection AddStower(this IServiceCollection services, Action<StowerOptions> configureOptions, params Assembly[] assembliesToScan)
        {
            return services.AddStower((_, options) => configureOptions(options), assembliesToScan);
        }
    }
}
