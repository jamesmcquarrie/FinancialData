using FinancialData.Domain.Entities;
using FinancialData.Domain.Enums;

namespace FinancialData.WorkerApplication.Repositories;

public interface ITimeSeriesScheduledRepository
{
    Task<Stock> GetStockAsync(string symbol, Interval interval);
    Task CreateStocksAsync(IEnumerable<Stock> stocks);
    Task<IEnumerable<TimeSeries>> GetTimeSeriesAsync(string symbol, Interval interval);
    Task AddTimeSeriesToStockAsync(string symbol, Interval interval, IEnumerable<TimeSeries> timeSeries);
}
