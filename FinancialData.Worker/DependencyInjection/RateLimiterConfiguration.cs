using FinancialData.Worker.Options;
using Microsoft.Extensions.Options;
using System.Threading.RateLimiting;

namespace FinancialData.Worker.DependencyInjection;

public static class RateLimiterConfiguration
{
    public static IServiceCollection AddTwelveDataTokenBucketRateLimiter(this IServiceCollection services, IConfiguration configuration)
    {
        var twelveDataTokenBucketLimiterOptions = configuration
            .GetRequiredSection(nameof(TwelveDataTokenBucketLimiterOptions))
            .Get<TwelveDataTokenBucketLimiterOptions>();

        var tokenBucketLimiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
        {
            TokenLimit = twelveDataTokenBucketLimiterOptions.TokenLimit,
            TokensPerPeriod = twelveDataTokenBucketLimiterOptions.TokensPerPeriod,
            ReplenishmentPeriod = TimeSpan.FromMinutes(twelveDataTokenBucketLimiterOptions.ReplenishmentPeriodMinutes),
            QueueLimit = twelveDataTokenBucketLimiterOptions.QueueLimit,
            QueueProcessingOrder = Enum.Parse<QueueProcessingOrder>(twelveDataTokenBucketLimiterOptions.QueueProcessingOrder),
            AutoReplenishment = twelveDataTokenBucketLimiterOptions.AutoReplenishment,
        });

        services.AddSingleton<RateLimiter>(tokenBucketLimiter);
        
        return services;
    }
}
