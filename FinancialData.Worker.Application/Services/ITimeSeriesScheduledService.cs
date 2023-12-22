using FinancialData.Domain.Entities;
using FinancialData.Domain.Enums;

namespace FinancialData.Worker.Application.Services;

public interface ITimeSeriesScheduledService
{
    Task<Stock> GetStockAsync(string symbol, Interval interval, int outputSize);
    Task CreateStocksAsync(IEnumerable<Stock> stocks);
    Task<IEnumerable<TimeSeries>> GetTimeSeriesAsync(string symbol, Interval interval, int outputSize);
    Task CreateTimeSeriesAsync(IEnumerable<TimeSeries> timeseries);
}
