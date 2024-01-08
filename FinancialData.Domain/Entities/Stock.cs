namespace FinancialData.Domain.Entities;

public class Stock
{
    public int Id { get; set; }
    public ICollection<TimeSeries> TimeSeries { get; set; } = new List<TimeSeries>();
    public MetaData? MetaData { get; set; }
}
