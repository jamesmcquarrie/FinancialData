using FinancialData.Domain.Enums;
using FinancialData.Common.Dtos;

namespace FinancialData.WorkerApplication.Clients;

public interface ITimeSeriesClient
{
    Task<StockDto> GetStockAsync(string symbol, Interval interval, int outputSize);
    Task<IEnumerable<TimeSeriesDto>> GetTimeSeriesAsync(string symbol, Interval interval, int outputSize);
}
