namespace FinancialData.Common.Dtos;

public class MetadataDto
{
    public required string Symbol { get; set; }
    public required string Type { get; set; }
    public required string Currency { get; set; }
    public required string Exchange { get; set; }
    public required string ExchangeTimezone { get; set; }
    public required string Interval { get; set; }
}
