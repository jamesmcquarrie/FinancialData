using FinancialData.Domain.Enums;
using Quartz;
using FinancialData.WorkerApplication.Services;
using System.Text.Json;

namespace FinancialData.Worker.TimeSeries;

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
        var tasks = new List<Task>();

        foreach(string symbol in deserializedSymbols)
        {
            tasks.Add(_timeSeriesService.CreateStockAsync(symbol, interval, outputSize));
        }

        await Task.WhenAll(tasks);

        //_logger.LogInformation("{0} stock with interval: {1} has been created", symbol, interval);
    }
}
