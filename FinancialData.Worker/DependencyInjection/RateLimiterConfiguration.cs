using FinancialData.Worker.Options;
using Microsoft.Extensions.Options;
using System.Threading.RateLimiting;

namespace FinancialData.Worker.DependencyInjection;

public static class RateLimiterConfiguration
{
    public static IServiceCollection AddTwelveDataTokenBucketRateLimiter(this IServiceCollection services)
    {
        services.AddSingleton<RateLimiter>((serviceProvider) =>
        {
            var twelveDataTokenBucketLimiterOptions = serviceProvider.GetRequiredService<IOptions<TwelveDataTokenBucketLimiterOptions>>().Value;

            var tokenBucketLimiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
            {
                TokenLimit = twelveDataTokenBucketLimiterOptions.TokenLimit,
                TokensPerPeriod = twelveDataTokenBucketLimiterOptions.TokensPerPeriod,
                ReplenishmentPeriod = TimeSpan.FromMinutes(twelveDataTokenBucketLimiterOptions.ReplenishmentPeriodMinutes),
                QueueLimit = twelveDataTokenBucketLimiterOptions.QueueLimit,
                QueueProcessingOrder = twelveDataTokenBucketLimiterOptions.QueueProcessingOrder,
                AutoReplenishment = twelveDataTokenBucketLimiterOptions.AutoReplenishment,
            });

            return tokenBucketLimiter;
        });
        
        return services;
    }
}
