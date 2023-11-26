using System.Threading.RateLimiting;

namespace FinancialData.Worker;

class RateLimiterHandler : DelegatingHandler
{
    private ILogger<RateLimiterHandler> _logger;
    private readonly RateLimiter _rateLimiter;

    public RateLimiterHandler(ILogger<RateLimiterHandler> logger,
        RateLimiter limiter)
    {
        _logger = logger;
        _rateLimiter = limiter;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var stats = _rateLimiter.GetStatistics();


        _logger.LogDebug(
            "Available Permits: {}\n" +
            "Current Queue: {}\n" +
            "Total Successful Leases: {}\n" +
            "Total Failed Leases: {}",
            stats.CurrentAvailablePermits.ToString(),
            stats.CurrentQueuedCount.ToString(),
            stats.TotalSuccessfulLeases.ToString(),
            stats.TotalFailedLeases.ToString());

        return await base.SendAsync(request, cancellationToken);
    }
}