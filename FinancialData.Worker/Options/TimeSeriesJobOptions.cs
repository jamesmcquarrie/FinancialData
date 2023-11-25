namespace FinancialData.Worker.Options;

public class TimeSeriesJobOptions
{
    public required string Interval { get; set; }
    public int OutputSize { get; set; }
    public int DelayMinutes { get; set; }
}
