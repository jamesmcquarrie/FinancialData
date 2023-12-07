using FinancialData.Domain.Entities;
using FinancialData.Common.Extensions;
using FinancialData.Domain.Enums;
using FinancialData.WorkerApplication.Clients;
using FinancialData.WorkerApplication.Repositories;
using Microsoft.Extensions.Logging;
using FinancialData.Common.Configuration;
using FinancialData.Common.Dtos;
using FinancialData.WorkerApplication.Abstractions;

namespace FinancialData.WorkerApplication.Services;

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
                        _logger.LogInformation("Stock for symbol: {} interval: {} retrieved from API", timeseriesArg.Symbol, timeseriesArg.Interval);

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
                        _logger.LogError("Stock for symbol: {} interval: {} not retrieved from API - ERROR MESSAGE: {}", timeseriesArg.Symbol, timeseriesArg.Interval, clientResult.ErrorMessage);
                    }
                }

                else
                {
                    _logger.LogInformation("Stock for symbol: {} interval: {} already exists in database", timeseriesArg.Symbol, timeseriesArg.Interval);
                }
            }

            return stocks;
        }

        catch (TaskCanceledException ex)
        {
            _logger.LogError($"{ex.Message} - Please increase the timeout duration.");
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

    public async Task<Dictionary<TimeSeriesArguments, IEnumerable<TimeSeries>>> GetTimeSeriesAsync(IEnumerable<TimeSeriesArguments> timeseriesArgs)
    {
        try
        {
            var tasks = new List<Task<(TimeSeriesArguments Arg, ClientResult<IEnumerable<TimeSeriesDto>> Result)>>();

            foreach (var timeseriesArg in timeseriesArgs)
            {
                var task = _timeSeriesClient.GetTimeSeriesAsync(timeseriesArg.Symbol, Interval.FromName(timeseriesArg.Interval), timeseriesArg.OutputSize)
                    .ContinueWith(task => (Arg: timeseriesArg, Result: task.Result));

                tasks.Add(task);
            }

            var clientResults = await Task.WhenAll(tasks);

            var timeseriesDictionary = new Dictionary<TimeSeriesArguments, IEnumerable<TimeSeries>>();

            foreach (var result in clientResults)
            {
                if (!result.Result.IsError)
                {
                    _logger.LogInformation("Timeseries for symbol: {} interval: {} retrieved from API", result.Arg.Symbol, result.Arg.Interval);

                    var timeseriesExists = await _timeSeriesRepository.GetTimeSeriesAsync(result.Arg.Symbol, Interval.FromName(result.Arg.Interval));

                    foreach (var timeseriesItem in result.Result.Payload)
                    {
                        var exists = timeseriesExists.Any(ts =>
                            ts.Datetime == timeseriesItem.Datetime);

                        if (!exists)
                        {
                            timeseriesDictionary[result.Arg] = result.Result.Payload!
                                .Select(ts => ts.ToEntity());
                        }
                    }
                }

                else
                {
                    _logger.LogError("Timeseries for symbol: {} interval: {} not retrieved from API - ERROR MESSAGE: {}", result.Arg.Symbol, result.Arg.Interval, result.Result.ErrorMessage);
                }
            }

            return timeseriesDictionary;
        }

        catch (TaskCanceledException ex)
        {
            _logger.LogError($"{ex.Message} - Please increase the timeout duration.");
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

        _logger.LogInformation("Timeseries for symbol: {} interval: {} has been persisted to database", symbol, interval.Name);
    }
}
