namespace FinancialData.Common.Dtos;

public class MetaDataDto
{
    public required string Symbol { get; set; }
    public required string Type { get; set; }
    public required string Currency { get; set; }
    public required string Exchange { get; set; }
    public required string ExchangeTimezone { get; set; }
    public required string MicCode { get; set; }
    public required string Interval { get; set; }
}
