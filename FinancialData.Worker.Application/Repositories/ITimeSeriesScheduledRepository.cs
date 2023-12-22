using FinancialData.Domain.Entities;
using FinancialData.Domain.Enums;

namespace FinancialData.Worker.Application.Repositories;

public interface ITimeSeriesScheduledRepository
{
    Task<Stock> GetStockAsync(string symbol, Interval interval);
    Task CreateStocksAsync(IEnumerable<Stock> stocks);
    Task<IEnumerable<TimeSeries>> GetTimeSeriesAsync(string symbol, Interval interval);
    Task CreateTimeSeriesAsync(IEnumerable<TimeSeries> timeSeries);
}
