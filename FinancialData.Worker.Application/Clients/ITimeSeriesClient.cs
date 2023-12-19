using FinancialData.Domain.Enums;
using FinancialData.Common.Dtos;
using FinancialData.Common.Abstractions;

namespace FinancialData.Worker.Application.Clients;

public interface ITimeSeriesClient
{
    Task<ServiceResult<StockDto>> GetStockAsync(string symbol, Interval interval, int outputSize);
    Task<ServiceResult<IEnumerable<TimeSeriesDto>>> GetTimeSeriesAsync(string symbol, Interval interval, int outputSize);
}
