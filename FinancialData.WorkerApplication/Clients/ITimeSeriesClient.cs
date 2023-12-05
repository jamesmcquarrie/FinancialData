using FinancialData.Domain.Enums;
using FinancialData.Common.Dtos;
using FinancialData.WorkerApplication.Abstractions;

namespace FinancialData.WorkerApplication.Clients;

public interface ITimeSeriesClient
{
    Task<ClientResult<StockDto>> GetStockAsync(string symbol, Interval interval, int outputSize);
    Task<ClientResult<IEnumerable<TimeSeriesDto>>> GetTimeSeriesAsync(string symbol, Interval interval, int outputSize);
}
