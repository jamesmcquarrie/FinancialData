using FinancialData.Worker.Application.Clients;
using FinancialData.Worker.Application.Repositories;
using FinancialData.Common.Configuration;
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

    public async Task<IEnumerable<Stock>> GetStocksAsync(IEnumerable<TimeSeriesArguments> timeseriesArgs)
    {
        List<Stock> stocks = new List<Stock>();

        try
        {
            foreach (var timeseriesArg in timeseriesArgs) 
            {
                var stock = await _timeSeriesRepository.GetStockAsync(timeseriesArg.Symbol, Interval.FromName(timeseriesArg.Interval));

                if (stock is null)
                {
                    var clientResult = await _timeSeriesClient.GetStockAsync(timeseriesArg.Symbol, Interval.FromName(timeseriesArg.Interval), timeseriesArg.OutputSize);

                    if (!clientResult.IsError)
                    {
                        _logger.LogInformation("Stock for symbol: {Symbol} interval: {Interval} retrieved from API", timeseriesArg.Symbol, timeseriesArg.Interval);

                        stock = new Stock
                        {
                            Metadata = clientResult.Payload!
                                .Metadata
                                .ToEntity(),
                            TimeSeries = clientResult.Payload!
                                .TimeSeries
                                .Select(ts => ts.ToEntity())
                                    .ToList()
                        };

                        stocks.Add(stock);
                    }

                    else
                    {
                        _logger.LogError("Stock for symbol: {Symbol} interval: {Interval} not retrieved from API - ERROR MESSAGE: {Message}", timeseriesArg.Symbol, timeseriesArg.Interval, clientResult.ErrorMessage);
                    }
                }

                else
                {
                    _logger.LogInformation("Stock for symbol: {Symbol} interval: {Interval} already exists in database", timeseriesArg.Symbol, timeseriesArg.Interval);
                }
            }

            return stocks;
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

    public async Task<IDictionary<TimeSeriesArguments, IEnumerable<TimeSeries>>> GetTimeSeriesAsync(IEnumerable<TimeSeriesArguments> timeseriesArgs)
    {
        try
        {
            var tasks = timeseriesArgs.Select(async arg =>
            {
                var result = await _timeSeriesClient.GetTimeSeriesAsync(arg.Symbol, Interval.FromName(arg.Interval), arg.OutputSize);
                return (Arg: arg, Result: result);
            });

            var clientResults = await Task.WhenAll(tasks);

            var timeseriesDictionary = new Dictionary<TimeSeriesArguments, IEnumerable<TimeSeries>>();

            foreach (var result in clientResults)
            {
                if (!result.Result.IsError)
                {
                    _logger.LogInformation("Timeseries for symbol: {Symbol} interval: {Interval} retrieved from API", result.Arg.Symbol, result.Arg.Interval);

                    var existingTimeseries = await _timeSeriesRepository.GetTimeSeriesAsync(result.Arg.Symbol, Interval.FromName(result.Arg.Interval));
                    
                    var newTimeseries = result.Result.Payload!
                        .Select(newTs => newTs.ToEntity())
                        .Where(newTs => !existingTimeseries
                            .Any(oldTs => oldTs.Datetime == newTs.Datetime));

                    if (newTimeseries.Any())
                    {
                        timeseriesDictionary[result.Arg] = newTimeseries;
                    }

                    else
                    {
                        _logger.LogInformation("No new Timeseries data for symbol: {Symbol} interval: {Interval}", result.Arg.Symbol, result.Arg.Interval);
                    }
                }

                else
                {
                    _logger.LogError("Timeseries for symbol: {Symbol} interval: {Interval} not retrieved from API - ERROR MESSAGE: {Message}", result.Arg.Symbol, result.Arg.Interval, result.Result.ErrorMessage);
                }
            }

            return timeseriesDictionary;
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

    public async Task AddTimeSeriesToStockAsync(string symbol, Interval interval, IEnumerable<TimeSeries> timeseries)
    {
        await _timeSeriesRepository.AddTimeSeriesToStockAsync(symbol, interval, timeseries);

        _logger.LogInformation("Timeseries for symbol: {Symbol} interval: {Interval} has been persisted to database", symbol, interval.Name);
    }
}
