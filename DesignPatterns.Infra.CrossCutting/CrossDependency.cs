#region

using CircuitBreaker.CrossCutting.Extensions;
using CircuitBreaker.Domain.Interfaces.Services;
using CircuitBreaker.Service;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace CircuitBreaker.CrossCutting;

public static class CrossDependency
{
    public static IServiceCollection SetupDependencyInjection(this IServiceCollection services)
    {
        services.SetupServicesOptions();
        services.AddSingleton(PollyExtension.CreatePolicy());
        services.AddTransient<ICircuitBreakerService2, CircuitBreakerService2>(); 
        
        return services;
    }

    private static void SetupServicesOptions(this IServiceCollection services)
    {
        services
            .AddHttpClient<ICircuitBreakerService, CircuitBreakerService>()
            .SetHandlerLifetime(TimeSpan.FromMinutes(30))
            .AddPolicyHandler(PollyExtension.GetRetryPolicy())
            .AddPolicyHandler(PollyExtension.GetCircuitBreakerPolicy()); 
    }
}