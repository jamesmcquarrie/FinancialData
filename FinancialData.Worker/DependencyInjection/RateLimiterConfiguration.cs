using FinancialData.Worker.Options;
using Microsoft.Extensions.Options;
using System.Threading.RateLimiting;

namespace FinancialData.Worker.DependencyInjection;

public static class RateLimiterConfiguration
{
    public static IServiceCollection AddTwelveDataTokenBucketRateLimiter(this IServiceCollection services)
    {
        services.ConfigureOptions<TwelveDataTokenBucketLimiterOptionsSetup>();

        services.AddSingleton<RateLimiter>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<TwelveDataTokenBucketLimiterOptions>>().Value;
            if (options == null)
            {
                throw new InvalidOperationException("ApplicationTokenBucketLimiterOptions must be configured.");
            }

            return new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
            {
                TokenLimit = options.TokenLimit,
                TokensPerPeriod = options.TokensPerPeriod,
                ReplenishmentPeriod = TimeSpan.FromMinutes(options.ReplenishmentPeriodMinutes),
                QueueLimit = options.QueueLimit,
                QueueProcessingOrder = Enum.Parse<QueueProcessingOrder>(options.QueueProcessingOrder),
                AutoReplenishment = options.AutoReplenishment,
            });
        });
        
        return services;
    }
}
