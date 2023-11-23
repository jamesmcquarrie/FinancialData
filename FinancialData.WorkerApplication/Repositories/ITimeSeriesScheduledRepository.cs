using FinancialData.Domain.Entities;
using FinancialData.Domain.Enums;

namespace FinancialData.WorkerApplication.Repositories;

public interface ITimeSeriesScheduledRepository
{
    public Task<Stock> GetStockAsync(string symbol, Interval interval);
    public Task CreateStocksAsync(IEnumerable<Stock> stocks);
    public Task AddTimeSeriesToStockAsync(string symbol, Interval interval, IEnumerable<TimeSeries> timeSeries);
}
