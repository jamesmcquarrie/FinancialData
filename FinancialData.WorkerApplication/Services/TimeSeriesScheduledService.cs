using FinancialData.Domain.Entities;
using FinancialData.Common.Extensions;
using FinancialData.Domain.Enums;
using FinancialData.WorkerApplication.Clients;
using FinancialData.WorkerApplication.Repositories;

namespace FinancialData.WorkerApplication.Services;

public class TimeSeriesScheduledService : ITimeSeriesScheduledService
{
    private ITimeSeriesClient _timeSeriesClient;
    private ITimeSeriesScheduledRepository _timeSeriesRepository;

    public TimeSeriesScheduledService(ITimeSeriesClient timeSeriesClient, ITimeSeriesScheduledRepository timeSeriesRepository)
    {
        _timeSeriesClient = timeSeriesClient;
        _timeSeriesRepository = timeSeriesRepository;
    }

    public async Task<Stock> GetStockAsync(string symbol, Interval interval, int outputSize)
    {
        var response = await _timeSeriesClient.GetStockAsync(symbol, interval, outputSize);

        var stock = new Stock
        {
            Metadata = response.Metadata
                .ToEntity(),
            TimeSeries = response.TimeSeries
                .Select(ts => ts.ToEntity())
                    .ToList()
        };

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
        var response = await _timeSeriesClient.GetTimeSeriesAsync(symbol, interval, outputSize);
        var timeSeries = response.Select(ts => ts.ToEntity());

        return timeSeries;
    }

    public async Task AddMultipleTimeSeriesToStockAsync(string symbol, Interval interval, IEnumerable<TimeSeries> timeseries)
    {
        await _timeSeriesRepository.AddTimeSeriesToStockAsync(symbol, interval, timeseries);
    }
}
