using FinancialData.Domain.Entities;
using FinancialData.Common.Dtos;
using System.Globalization;

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
            MicCode = metaData.MicCode,
            Interval = metaData.Interval
        };
    }

    public static TimeSeriesDto ToDto(this TimeSeries timeSeries)
    {
        return new TimeSeriesDto
        {
            Datetime = timeSeries.Datetime.ToShortDateString(),
            High = timeSeries.High.ToString(CultureInfo.InvariantCulture),
            Low = timeSeries.Low.ToString(CultureInfo.InvariantCulture),
            Open = timeSeries.Open.ToString(CultureInfo.InvariantCulture),
            Close = timeSeries.Close.ToString(CultureInfo.InvariantCulture),
            Volume = timeSeries.Volume.ToString(CultureInfo.InvariantCulture),
        };
    }
}
