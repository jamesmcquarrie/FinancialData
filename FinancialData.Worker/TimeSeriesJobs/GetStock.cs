using FinancialData.Domain.Entities;
using FinancialData.Domain.Enums;
using FinancialData.WorkerApplication.Services;
using Quartz;
using System.Text.Json;

namespace FinancialData.Worker.TimeSeriesJobs;

public class GetStock : IJob
{
    private readonly ILogger<GetStock> _logger;
    private readonly ITimeSeriesScheduledService _timeSeriesService;

    public GetStock(ILogger<GetStock> logger,
        ITimeSeriesScheduledService timeSeriesService)
    {
        _logger = logger;
        _timeSeriesService = timeSeriesService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var dataMap = context.MergedJobDataMap;

        var symbols = dataMap.GetString("symbols");
        var interval = Interval.FromName(dataMap
            .GetString("interval"));
        var outputSize = dataMap.GetInt("outputSize");

        var deserializedSymbols = JsonSerializer.Deserialize<string[]>(symbols);
        var tasks = new List<Task<Stock>>();

        foreach(string symbol in deserializedSymbols)
        {
            tasks.Add(_timeSeriesService.GetStockAsync(symbol, interval, outputSize));
        }

        var stocks = await Task.WhenAll(tasks);

        await _timeSeriesService.CreateStocksAsync(stocks);

        //_logger.LogInformation("{0} stock with interval: {1} has been created", symbol, interval);
    }
}
