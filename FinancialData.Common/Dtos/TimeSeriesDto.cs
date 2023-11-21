namespace FinancialData.Common.Dtos;

public class TimeSeriesDto
{
    public required string Datetime { get; set; }
    public required string High { get; set; }
    public required string Low { get; set; }
    public required string Open { get; set; }
    public required string Close { get; set; }
    public required string Volume { get; set; }
}
