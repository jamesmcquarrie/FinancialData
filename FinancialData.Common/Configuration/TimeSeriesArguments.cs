namespace FinancialData.Common.Configuration;

public class TimeSeriesArguments
{
    public required string Symbol { get; set; }
    public required string Interval { get; set; }
    public int OutputSize { get; set; }
}
