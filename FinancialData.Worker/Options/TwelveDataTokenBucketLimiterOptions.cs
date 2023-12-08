namespace FinancialData.Worker.Options;

public class TwelveDataTokenBucketLimiterOptions
{
    public int TokenLimit { get; set; }
    public int TokensPerPeriod { get; set; }
    public int ReplenishmentPeriodMinutes { get; set; }
    public int QueueLimit { get; set; }
    public string QueueProcessingOrder { get; set; } = "OldestFirst";
    public bool AutoReplenishment { get; set; }
}
