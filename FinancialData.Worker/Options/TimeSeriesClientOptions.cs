namespace FinancialData.Worker.Options;

public class TimeSeriesClientOptions
{
    public required string BaseUrl { get; set; }
    public int TimeoutMinutes { get; set; }
}
