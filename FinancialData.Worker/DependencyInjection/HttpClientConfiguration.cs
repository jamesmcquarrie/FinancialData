using FinancialData.Worker.Application.Clients;
using FinancialData.Worker.Options;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using Polly;
using System.Net.Http.Headers;
using System.Threading.RateLimiting;

namespace FinancialData.Worker.DependencyInjection;

public static class HttpClientConfiguration
{
    public static IServiceCollection AddTimeSeriesClient(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure options
        services.ConfigureOptions<TimeSeriesClientOptionsSetup>();

        services.AddHttpClient<ITimeSeriesClient, TimeSeriesClient>((serviceProvider, client) =>
        {
            var timeSeriesClientOptions = serviceProvider.GetRequiredService<IOptions<TimeSeriesClientOptions>>().Value;
            if (timeSeriesClientOptions == null)
            {
                throw new InvalidOperationException("TimeSeriesClientOptions must be configured.");
            }

            client.BaseAddress = new Uri(timeSeriesClientOptions.BaseUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("apikey", configuration["ApiKey"]);
            client.Timeout = TimeSpan.FromMinutes(timeSeriesClientOptions.TimeoutMinutes);
        })
        .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(2) })
        .SetHandlerLifetime(Timeout.InfiniteTimeSpan)
        .AddResilienceHandler("TwelveData Token Bucket Rate Limiter", static (ResiliencePipelineBuilder<HttpResponseMessage> builder,
            ResilienceHandlerContext context) =>
        {
            var tokenBucket = context.ServiceProvider.GetService<RateLimiter>();
            builder.AddRateLimiter(tokenBucket);
        });

        return services;
    }
}
