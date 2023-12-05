using Microsoft.Extensions.Logging;
using System.Threading.RateLimiting;

namespace FinancialData.WorkerApplication.Handlers;

public class DebugRateLimiterHandler : DelegatingHandler
{
    private ILogger<DebugRateLimiterHandler> _logger;
    private readonly RateLimiter _rateLimiter;

    public DebugRateLimiterHandler(ILogger<DebugRateLimiterHandler> logger,
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