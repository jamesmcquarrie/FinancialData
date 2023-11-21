using FinancialData.Domain.Enums;

namespace FinancialData.WorkerApplication.Services;

public interface ITimeSeriesScheduledService
{
    Task CreateStockAsync(string symbol, Interval interval, int outputSize);
    Task AddTimeSeriesAsync(string symbol, Interval interval, int outputSize);
}
