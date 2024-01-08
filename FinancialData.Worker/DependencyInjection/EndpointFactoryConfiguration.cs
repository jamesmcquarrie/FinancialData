using FinancialData.Worker.Application.Factories;
using FinancialData.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace FinancialData.Worker.DependencyInjection;

public static class EndpointFactoryConfiguration
{
    public static IServiceCollection AddTwelveDataEndpointFactory(this IServiceCollection services)
    {
        services.AddSingleton<TimeSeriesEndpointFactory>();

        services.AddSingleton((serviceProvider) =>
        {
            var endpointFactory = serviceProvider.GetRequiredService<TimeSeriesEndpointFactory>();
            var timeSeriesClientOptions = serviceProvider.GetRequiredService<IOptions<TwelveDataClientOptions>>().Value;

            var twelveDataEndpointFactory = endpointFactory.Create(timeSeriesClientOptions.ApiVersion);

            return twelveDataEndpointFactory;
        });

        return services;
    }
}
