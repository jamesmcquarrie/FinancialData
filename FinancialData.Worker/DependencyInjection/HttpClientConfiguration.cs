using FinancialData.Worker.Application.Clients;
using FinancialData.Infrastructure.Options;
using System.Net.Http.Headers;
using System.Threading.RateLimiting;
using Microsoft.Extensions.Options;
using Polly;

namespace FinancialData.Worker.DependencyInjection;

public static class HttpClientConfiguration
{
    public static IServiceCollection AddTwelveDataTimeSeriesClient(this IServiceCollection services)
    {
        services.AddHttpClient<ITimeSeriesClient, TwelveDataTimeSeriesClient>((serviceProvider, client) =>
        {
            var timeSeriesClientOptions = serviceProvider.GetRequiredService<IOptions<TwelveDataClientOptions>>().Value;

            client.BaseAddress = new Uri(timeSeriesClientOptions.BaseUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("apikey", timeSeriesClientOptions.ApiKey);
            client.Timeout = TimeSpan.FromMinutes(timeSeriesClientOptions.TimeoutMinutes);
        })
        .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(2) })
        .SetHandlerLifetime(Timeout.InfiniteTimeSpan)
        .AddResilienceHandler("TwelveData Token Bucket Rate Limiter", (builder, context) =>
        {
            var tokenBucket = context.ServiceProvider.GetService<RateLimiter>();
            builder.AddRateLimiter(tokenBucket);
        });

        return services;
    }
}
