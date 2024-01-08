using System.Text.Json.Serialization;

namespace FinancialData.Common.Dtos;

public class StockDto
{
    public virtual MetaDataDto MetaData { get; set; }
    public virtual ICollection<TimeSeriesDto> TimeSeries { get; set; } = new List<TimeSeriesDto>();
}
