namespace FinancialData.Domain.Entities;

public class Metadata
{
    public int Id { get; set; }
    public required string Symbol { get; set; }
    public required string Type { get; set; }
    public required string Currency {  get; set; }
    public required string Exchange { get; set; }
    public required string ExchangeTimezone { get; set; }
    public required string MicCode { get; set; }
    public required string Interval { get; set; }
    public int StockId { get; set; }
    public Stock? Stock { get; set; }
}
