using FinancialData.Domain.Entities;
using FinancialData.Domain.Enums;
using FinancialData.Worker.Application.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FinancialData.Infrastructure.Repositories;

public class TimeSeriesScheduledRepository : ITimeSeriesScheduledRepository
{
    private readonly FinancialDataContext _context;

    public TimeSeriesScheduledRepository(FinancialDataContext context)
    {
        _context = context;
    }

    public async Task<Stock> GetStockAsync(string symbol, Interval interval)
    {
        var stock = await _context.Stocks
            .Include(s => s.MetaData)
            .SingleOrDefaultAsync(s => 
                s.MetaData.Symbol == symbol && s.MetaData.Interval == interval.Name);

        return stock;
    }

    public async Task CreateStocksAsync(IEnumerable<Stock> stocks)
    {
        await _context.Stocks.AddRangeAsync(stocks);

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<TimeSeries>> GetTimeSeriesAsync(string symbol, Interval interval)
    {
        var timeseries = await _context.TimeSeries
            .Include(ts => ts.Stock.MetaData)
            .Where(ts => ts.Stock.MetaData.Symbol == symbol && ts.Stock.MetaData.Interval == interval.Name)
            .ToArrayAsync();

        return timeseries;
    }

    public async Task CreateTimeSeriesAsync(IEnumerable<TimeSeries> timeSeries)
    {
        await _context.TimeSeries.AddRangeAsync(timeSeries);

        await _context.SaveChangesAsync();
    }
}
