using FinancialData.Domain.Entities;

namespace FinancialData.Domain.Repositories;

public interface ITimeSeriesRepository
{
    Task<Stock> GetStockAsync(int id, int timeseriesOutputSize);
    Task<IEnumerable<TimeSeries>> GetTimeseriesAsync(int stockId, int timeseriesOutputSize);
    Task<MetaData> GetMetaDataAsync(int id);
}
