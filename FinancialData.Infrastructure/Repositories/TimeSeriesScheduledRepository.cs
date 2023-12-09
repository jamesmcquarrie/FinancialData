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
            .Include(s => s.Metadata)
            .FirstOrDefaultAsync(s => 
                s.Metadata.Symbol == symbol && s.Metadata.Interval == interval.Name);

        return stock;
    }

    public async Task CreateStocksAsync(IEnumerable<Stock> stocks)
    {
        await _context.Stocks.AddRangeAsync(stocks);

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<TimeSeries>> GetTimeSeriesAsync(string symbol, Interval interval)
    {
        var stock = await _context.Stocks
           .Include(s => s.Metadata)
           .Include(s => s.TimeSeries)
           .FirstOrDefaultAsync(s =>
               s.Metadata.Symbol == symbol && s.Metadata.Interval == interval.Name);

        return stock.TimeSeries;
    }

    public async Task AddTimeSeriesToStockAsync(string symbol, Interval interval, IEnumerable<TimeSeries> timeSeries)
    {
        var stock = await _context.Stocks
            .Include(s => s.Metadata)
            .FirstOrDefaultAsync(s =>
                s.Metadata.Symbol == symbol && s.Metadata.Interval == interval.Name);

        foreach (var timeSeriesItem in timeSeries) 
        {
            stock.TimeSeries.Add(timeSeriesItem);
        }

        await _context.SaveChangesAsync();
    }
}
