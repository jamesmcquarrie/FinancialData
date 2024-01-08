using FinancialData.Worker.Application.Repositories;
using FinancialData.Worker.Application.Clients;
using FinancialData.Common.Extensions;
using FinancialData.Domain.Entities;
using FinancialData.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace FinancialData.Worker.Application.Services;

public class TimeSeriesScheduledService : ITimeSeriesScheduledService
{
    private readonly ILogger<TimeSeriesScheduledService> _logger;
    private readonly ITimeSeriesClient _timeSeriesClient;
    private readonly ITimeSeriesScheduledRepository _timeSeriesRepository;

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
        try
        {
            var stock = await _timeSeriesRepository.GetStockAsync(symbol, interval);

            if (stock is not null)
            {
                _logger.LogInformation("Stock for symbol: {Symbol} interval: {Interval} already exists in database", symbol, interval.Name);

                return null;
            }

            var clientResult = await _timeSeriesClient.GetStockAsync(symbol, interval, outputSize);

            if (!clientResult.IsError)
            {
                _logger.LogInformation("Stock for symbol: {Symbol} interval: {Interval} retrieved from API", symbol, interval);

                stock = new Stock
                {
                    MetaData = clientResult.Payload!
                        .MetaData
                        .ToEntity(),
                    TimeSeries = clientResult.Payload!
                        .TimeSeries
                        .Select(ts => ts.ToEntity())
                            .ToList()
                };
            }

            else
            {
                _logger.LogError("Stock for symbol: {Symbol} interval: {Interval} not retrieved from API - ERROR MESSAGE: {Message}", symbol, interval.Name, clientResult.ErrorMessage);
            }

            return stock;
        }

        catch (TaskCanceledException ex)
        {
            _logger.LogError("{Message} - Please increase the timeout duration", ex.Message);
            throw;
        }

        catch (HttpRequestException ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }

        catch (Exception ex) 
        {
            _logger.LogError(ex, "UNEXPECTED ERROR");
            throw;
        }
    }

    public async Task CreateStocksAsync(IEnumerable<Stock> stocks)
    {
        await _timeSeriesRepository.CreateStocksAsync(stocks);

        _logger.LogInformation("Stocks have been persisted to database");
    }

    public async Task<IEnumerable<TimeSeries>> GetTimeSeriesAsync(string symbol, Interval interval, int outputSize)
    {
        try
        {
            var clientResult = await _timeSeriesClient.GetTimeSeriesAsync(symbol, interval, outputSize);

            var newTimeseries = Array.Empty<TimeSeries>();

            if (!clientResult.IsError)
            {
                _logger.LogInformation("Timeseries for symbol: {Symbol} interval: {Interval} retrieved from API", symbol, interval.Name);

                var existingTimeseries = await _timeSeriesRepository.GetTimeSeriesAsync(symbol, interval);

                newTimeseries = clientResult.Payload!
                    .Select(newTs => newTs.ToEntity())
                    .Where(newTs => !existingTimeseries
                        .Any(oldTs => oldTs.Datetime == newTs.Datetime))
                    .ToArray();

                if (newTimeseries.Any())
                {
                    var stockId = existingTimeseries.First().StockId;

                    foreach (var timeSeries in newTimeseries)
                    {
                        timeSeries.StockId = stockId;
                    }
                }

                else
                {
                    _logger.LogInformation("No new Timeseries data for symbol: {Symbol} interval: {Interval}", symbol, interval.Name);
                }
            }

            else
            {
                _logger.LogError("Timeseries for symbol: {Symbol} interval: {Interval} not retrieved from API - ERROR MESSAGE: {Message}", symbol, interval.Name, clientResult.ErrorMessage);
            }

            return newTimeseries;
        }

        catch (TaskCanceledException ex)
        {
            _logger.LogError($"{ex.Message} - Please increase the timeout duration.");
            throw;
        }

        catch (HttpRequestException ex)
        {
            _logger.LogError(ex.Message);
            throw;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "UNEXPECTED ERROR");
            throw;
        }
    }

    public async Task CreateTimeSeriesAsync(IEnumerable<TimeSeries> timeseries)
    {
        await _timeSeriesRepository.CreateTimeSeriesAsync(timeseries);

        _logger.LogInformation("Timeseries have been persisted to database");
    }
}
