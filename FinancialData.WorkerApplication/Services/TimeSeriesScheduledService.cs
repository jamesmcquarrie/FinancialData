using FinancialData.Domain.Entities;
using FinancialData.Common.Extensions;
using FinancialData.Domain.Enums;
using FinancialData.WorkerApplication.Clients;
using FinancialData.WorkerApplication.Repositories;
using Microsoft.Extensions.Logging;

namespace FinancialData.WorkerApplication.Services;

public class TimeSeriesScheduledService : ITimeSeriesScheduledService
{
    private ILogger<TimeSeriesScheduledService> _logger;
    private ITimeSeriesClient _timeSeriesClient;
    private ITimeSeriesScheduledRepository _timeSeriesRepository;

    public TimeSeriesScheduledService(ILogger<TimeSeriesScheduledService> logger,
        ITimeSeriesClient timeSeriesClient,
        ITimeSeriesScheduledRepository timeSeriesRepository)
    {
        _logger = logger;
        _timeSeriesClient = timeSeriesClient;
        _timeSeriesRepository = timeSeriesRepository;
    }

    public async Task<Stock> GetStockAsync(string symbol, Interval interval, int outputSize)
    {
        Stock stock = null;

        try
        {
            var clientResult = await _timeSeriesClient.GetStockAsync(symbol, interval, outputSize);

            if (!clientResult.IsError)
            {
                stock = new Stock
                {
                    Metadata = clientResult.Result!
                        .Metadata
                        .ToEntity(),
                    TimeSeries = clientResult.Result!
                        .TimeSeries
                        .Select(ts => ts.ToEntity())
                            .ToList()
                };
            }
        }

        catch (TaskCanceledException ex)
        {
            _logger.LogError($"{ex.Message} - Please increase the timeout duration.");
            throw;
        }

        catch (Exception ex) 
        {
            _logger.LogError(ex, "Unexpected error encountered whilst retrieving symbol: {} interval: {} - EXCEPTION MESSAGE: {}", symbol, interval.Name, ex.Message);
            throw;
        }

        return stock;
    }

    public async Task CreateStocksAsync(IEnumerable<Stock> stocks)
    {
        var stocksList = new List<Stock>();

        foreach (var stock in stocks)
        {
            var stockExists = await _timeSeriesRepository.GetStockAsync(stock.Metadata.Symbol,
                Interval.FromName(
                    stock.Metadata.Interval));

            if (stockExists is null)
            {
                stocksList.Add(stock);
            }
        }

        if (stocksList.Any())
        {
            await _timeSeriesRepository.CreateStocksAsync(stocksList);
        }
    }

    public async Task<IEnumerable<TimeSeries>> GetTimeSeriesAsync(string symbol, Interval interval, int outputSize)
    {
        IEnumerable<TimeSeries> timeseries = null;

        try
        {
            var clientResult = await _timeSeriesClient.GetTimeSeriesAsync(symbol, interval, outputSize);

            if (!clientResult.IsError)
            {
                timeseries = clientResult.Result!
                    .Select(ts => ts.ToEntity());
            }
        }

        catch (TaskCanceledException ex)
        {
            _logger.LogError($"{ex.Message} - Please increase the timeout duration.");
            throw;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error encountered whilst retrieving symbol: {} interval: {} - EXCEPTION MESSAGE: {}", symbol, interval.Name, ex.Message);
            throw;
        }

        return timeseries;
    }

    public async Task AddMultipleTimeSeriesToStockAsync(string symbol, Interval interval, IEnumerable<TimeSeries> timeseries)
    {
        var timeseriesList = new List<TimeSeries>();

        foreach (var timeseriesItem in timeseries)
        {
            var timeseriesExists = await _timeSeriesRepository.GetTimeSeriesAsync(symbol, interval, timeseriesItem.Datetime);

            if (timeseriesExists is null)
            {
                timeseriesList.Add(timeseriesItem);
            }
        }

        await _timeSeriesRepository.AddTimeSeriesToStockAsync(symbol, interval, timeseriesList);
    }
}
