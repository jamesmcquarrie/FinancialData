using FinancialData.Domain.Entities;
using FinancialData.Common.Dtos;

namespace FinancialData.Common.Extensions;

public static class TimeSeriesToEntityExtensions
{
    public static Metadata ToEntity(this MetadataDto metaDataDto)
    {
        return new Metadata
        {
            Symbol = metaDataDto.Symbol,
            Type = metaDataDto.Type,
            Currency = metaDataDto.Currency,
            Exchange = metaDataDto.Exchange,
            ExchangeTimezone = metaDataDto.ExchangeTimezone,
            Interval = metaDataDto.Interval,
        };
    }

    public static TimeSeries ToEntity(this TimeSeriesDto timeSeriesDto)
    {
        return new TimeSeries
        {
            Datetime = timeSeriesDto.Datetime,
            High = timeSeriesDto.High,
            Low = timeSeriesDto.Low,
            Open = timeSeriesDto.Open,
            Close = timeSeriesDto.Close,
            Volume = timeSeriesDto.Volume,
        };
    }
}
