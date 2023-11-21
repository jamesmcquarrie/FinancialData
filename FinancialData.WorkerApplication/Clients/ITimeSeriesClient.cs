using FinancialData.Domain.Enums;
using FinancialData.Common.Dtos;

namespace FinancialData.WorkerApplication.Clients;

public interface ITimeSeriesClient
{
    public Task<StockDto> GetStockAsync(string symbol, Interval interval, int outputSize);
    public Task<IEnumerable<TimeSeriesDto>> GetTimeSeriesAsync(string symbol, Interval interval, int outputSize);
}
