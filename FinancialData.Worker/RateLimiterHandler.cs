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



        //var limiter = new TokenBucketRateLimiter(rateLimiterOptions);

        //using RateLimitLease lease = await limiter.WaitAsync(5);
        //var lease = await _rateLimiter.WaitAsync(1, cancellationToken); // Acquire a token

        //if (!lease.IsAcquired)
        //{
        //    Handle the case where a token couldn't be acquired
        //     For example, return a TooManyRequests response or delay and retry
        //}

        //    try
        //    {
        //        var response = await base.SendAsync(request, cancellationToken);
        //        return response;
        //    }
        //    finally
        //    {
        //        lease.Dispose(); // Release the token
        //    }


        var stats = _rateLimiter.GetStatistics();

        _logger.LogInformation(
            "Available Permits: {}\n" +
            "Current Queue: {}\n" +
            "Total Successful Leases: {}\n" +
            "Total Failed Leases: {}",
            stats.CurrentAvailablePermits.ToString(),
            stats.CurrentQueuedCount.ToString(),
            stats.TotalSuccessfulLeases.ToString(),
            stats.TotalFailedLeases.ToString());

        return await base.SendAsync(request, cancellationToken);
        //var response = new HttpResponseMessage(System.Net.HttpStatusCode.TooManyRequests);
        //if (lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        //{
        //    response.Headers.Add(HeaderNames.RetryAfter, ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo));
        //}
        //return response;
    }
}