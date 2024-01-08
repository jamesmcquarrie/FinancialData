using FinancialData.Common.Abstractions;
using FinancialData.Common.Dtos;

namespace FinancialData.Api.Application.Services;

public interface ITimeSeriesService
{
    Task<ServiceResult<StockDto>> GetStockAsync(int id, int timeseriesOutputSize);
    Task<ServiceResult<IEnumerable<TimeSeriesDto>>> GetTimeSeriesAsync(int id, int timeseriesOutputSize);
    Task<ServiceResult<MetaDataDto>> GetMetaDataAsync(int id);
}
