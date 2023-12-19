using FinancialData.Domain.Entities;
using FinancialData.Common.Dtos;
using System.Globalization;

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
            MicCode = metaDataDto.MicCode,
            Interval = metaDataDto.Interval,
        };
    }

    public static TimeSeries ToEntity(this TimeSeriesDto timeSeriesDto)
    {
        return new TimeSeries
        {
            Datetime = Convert.ToDateTime(timeSeriesDto.Datetime, CultureInfo.InvariantCulture),
            High = Convert.ToDouble(timeSeriesDto.High, CultureInfo.InvariantCulture),
            Low = Convert.ToDouble(timeSeriesDto.Low, CultureInfo.InvariantCulture),
            Open = Convert.ToDouble(timeSeriesDto.Open, CultureInfo.InvariantCulture),
            Close = Convert.ToDouble(timeSeriesDto.Close, CultureInfo.InvariantCulture),
            Volume = Convert.ToInt32(timeSeriesDto.Volume, CultureInfo.InvariantCulture)
        };
    }
}
