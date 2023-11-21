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

    public async Task CreateStockAsync(string symbol, Interval interval, int outputSize)
    {
        var response = await _timeSeriesClient.GetStockAsync(symbol, interval, outputSize);

        var stock = await _timeSeriesRepository.GetStockAsync(symbol, interval);

        if (stock is null)
        {
            var newStock = new Stock
            {
                Metadata = response.Metadata.ToEntity(),
                TimeSeries = response.TimeSeries.Select(ts => ts.ToEntity()).ToList()
            };

            await _timeSeriesRepository.CreateStockAsync(newStock);
        }
    }

    public async Task AddTimeSeriesAsync(string symbol, Interval interval, int outputSize)
    {
        var response = await _timeSeriesClient.GetTimeSeriesAsync(symbol, interval, outputSize);
        var timeSeries = response.Select(ts => ts.ToEntity()).ToList();

        await _timeSeriesRepository.AddTimeSeriesToStockAsync(symbol, interval, timeSeries);
    }
}
