using FinancialData.Domain.Entities;
using FinancialData.Domain.Enums;

namespace FinancialData.WorkerApplication.Services;

public interface ITimeSeriesScheduledService
{
    Task<Stock> GetStockAsync(string symbol, Interval interval, int outputSize);
    Task CreateStocksAsync(IEnumerable<Stock> stocks);
    Task<IEnumerable<TimeSeries>> GetTimeSeriesAsync(string symbol, Interval interval, int outputSize);
    Task AddMultipleTimeSeriesToStockAsync(string symbol, Interval interval, IEnumerable<TimeSeries> timeseries);
}
