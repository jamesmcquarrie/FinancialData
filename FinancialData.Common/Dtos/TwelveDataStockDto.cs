using System.Text.Json.Serialization;

namespace FinancialData.Common.Dtos;

public class TwelveDataStockDto : StockDto
{
    [JsonPropertyName("meta")]
    public override MetaDataDto MetaData { get; set; }
    [JsonPropertyName("values")]
    public override ICollection<TimeSeriesDto> TimeSeries { get; set; } = new List<TimeSeriesDto>();
}
