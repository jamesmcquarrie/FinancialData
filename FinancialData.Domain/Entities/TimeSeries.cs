namespace FinancialData.Domain.Entities;

public class TimeSeries
{
    public int Id { get; set; }
    public DateTime Datetime { get; set; }
    public double High { get; set; }
    public double Low { get; set; }
    public double Open { get; set; }
    public double Close { get; set; }
    public double Volume { get; set; }
    public int StockId { get; set; }
    public Stock? Stock { get; set; }
}
