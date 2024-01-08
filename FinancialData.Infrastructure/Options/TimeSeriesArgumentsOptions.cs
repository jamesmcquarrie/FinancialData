namespace FinancialData.Infrastructure.Options;

public class TimeSeriesArgumentsOptions
{
    public required string ArgumentsPath { get; set; }
    public int DelayMinutes { get; set; }
}
