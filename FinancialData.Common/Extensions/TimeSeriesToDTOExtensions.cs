using FinancialData.Domain.Entities;
using FinancialData.Common.Dtos;

namespace FinancialData.Common.Extensions;

public static class TimeSeriesToDTOExtensions
{
    public static MetadataDto ToDto(this Metadata metaData)
    {
        return new MetadataDto
        {
            Symbol = metaData.Symbol,
            Type = metaData.Type,
            Currency = metaData.Currency,
            Exchange = metaData.Exchange,
            ExchangeTimezone = metaData.ExchangeTimezone,
            Interval = metaData.Interval
        };
    }

    public static TimeSeriesDto ToDto(this TimeSeries timeSeries)
    {
        return new TimeSeriesDto
        {
            Datetime = timeSeries.Datetime,
            High = timeSeries.High,
            Low = timeSeries.Low,
            Open = timeSeries.Open,
            Close = timeSeries.Close,
            Volume = timeSeries.Volume,
        };
    }
}
