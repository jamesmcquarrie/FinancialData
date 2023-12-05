using FinancialData.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using FinancialData.WorkerApplication.Repositories;
using FinancialData.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace FinancialData.Infrastructure.Repositories;

public class TimeSeriesScheduledRepository : ITimeSeriesScheduledRepository
{
    private readonly ILogger<TimeSeriesScheduledRepository> _logger;
    private readonly FinancialDataContext _context;

    public TimeSeriesScheduledRepository(ILogger<TimeSeriesScheduledRepository> logger, 
        FinancialDataContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Stock> GetStockAsync(string symbol, Interval interval)
    {
        var stock = await _context.Stocks
            .Include(s => s.Metadata)
            .FirstOrDefaultAsync(s => 
                s.Metadata.Symbol == symbol && s.Metadata.Interval == interval.Name);

        _logger.LogInformation("Stock for symbol: {} interval: {} has been retrieved from database", symbol, interval.Name);

        return stock;
    }

    public async Task CreateStocksAsync(IEnumerable<Stock> stocks)
    {
        await _context.Stocks.AddRangeAsync(stocks);

        await _context.SaveChangesAsync();

        _logger.LogInformation("Stocks have been persisted to database");
    }

    public async Task<TimeSeries> GetTimeSeriesAsync(string symbol, Interval interval, string datetime)
    {
        var stock = await _context.Stocks
            .Include(s => s.Metadata)
            .FirstOrDefaultAsync(s =>
                s.Metadata.Symbol == symbol && s.Metadata.Interval == interval.Name);

        var timeseries = stock.TimeSeries
            .FirstOrDefault(ts => ts.Datetime == datetime);

        _logger.LogInformation("Timeseries for symbol: {} interval: {} has been retrieved from database", symbol, interval.Name);

        return timeseries;
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

        _logger.LogInformation("Timeseries for symbol: {} interval: {} has been persisted to database", symbol, interval.Name);
    }
}
