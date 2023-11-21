using System.Text.Json.Serialization;

namespace FinancialData.Common.Dtos;

public class StockDto
{
    [JsonPropertyName("meta")]
    public required MetadataDto Metadata { get; set; }
    [JsonPropertyName("values")]
    public required ICollection<TimeSeriesDto> TimeSeries { get; set; }
}
