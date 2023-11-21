using FinancialData.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using FinancialData.Domain.Enums;
using FinancialData.WorkerApplication.Repositories;

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
        // Fetch the Stock entity from the database
        var stock = await _context.Stocks.Include(s => s.Metadata)
            .FirstOrDefaultAsync(s => s.Metadata.Symbol == symbol && s.Metadata.Interval == interval.Name);

        return stock;
    }

    public async Task CreateStockAsync(Stock stock)
    {
        // Add the Stock entity to the database
        await _context.Stocks.AddAsync(stock);
        await _context.SaveChangesAsync();
    }

    public async Task AddTimeSeriesToStockAsync(string symbol, Interval interval, List<TimeSeries> timeSeries)
    {
        // Fetch the Stock entity from the database
        var stock = await _context.Stocks.Include(s => s.Metadata)
            .FirstOrDefaultAsync(s => s.Metadata.Symbol == symbol && s.Metadata.Interval == interval.Name);

        ((List<TimeSeries>)stock.TimeSeries).AddRange(timeSeries);

        await _context.SaveChangesAsync();
    }
}
