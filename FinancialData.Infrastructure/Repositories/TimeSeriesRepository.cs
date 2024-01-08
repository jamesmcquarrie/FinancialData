using FinancialData.Domain.Entities;
using FinancialData.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FinancialData.Infrastructure.Repositories;

public class TimeSeriesRepository : ITimeSeriesRepository
{
    private FinancialDataContext _context;

    public TimeSeriesRepository(FinancialDataContext context)
    {
        _context = context;
    }

    public async Task<Stock> GetStockAsync(int id, int timeseriesOutputSize = 5)
    {
        var stock = await _context.Stocks
            .Where(s  => s.Id == id)
            .Include(s => s.MetaData)
            .Include(s => s.TimeSeries
                .OrderByDescending(ts => ts.Datetime)
                .Take(timeseriesOutputSize))
            .SingleOrDefaultAsync();
            
        return stock;
    }

    public async Task<IEnumerable<TimeSeries>> GetTimeseriesAsync(int stockId, int timeseriesOutputSize = 5)
    {
        var timeseries = await _context.TimeSeries
            .Where(s => s.StockId == stockId)
            .OrderByDescending(ts => ts.Datetime)
            .Take(timeseriesOutputSize)
            .ToArrayAsync();

        return timeseries;
    }

    public async Task<MetaData> GetMetaDataAsync(int id)
    {
        var metadata = await _context.MetaData
            .SingleOrDefaultAsync(m => m.Id == id);

        return metadata;
    }
}
