using FinancialData.Common.Configuration;
using FinancialData.Domain.Entities;
using FinancialData.Domain.Enums;

namespace FinancialData.Worker.Application.Services;

public interface ITimeSeriesScheduledService
{
    Task<IEnumerable<Stock>> GetStocksAsync(IEnumerable<TimeSeriesArguments> timeSeriesArgs);
    Task CreateStocksAsync(IEnumerable<Stock> stocks);
    Task<IDictionary<TimeSeriesArguments, IEnumerable<TimeSeries>>> GetTimeSeriesAsync(IEnumerable<TimeSeriesArguments> timeSeriesArgs);
    Task AddTimeSeriesToStockAsync(string symbol, Interval interval, IEnumerable<TimeSeries> timeseries);
}
