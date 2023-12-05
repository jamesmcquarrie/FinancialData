using FinancialData.Domain.Entities;
using FinancialData.Domain.Enums;
using FinancialData.WorkerApplication.Services;
using System.Text.Json;
using Quartz;

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
        try
        {
            var dataMap = context.MergedJobDataMap;

            var interval = Interval.FromName(dataMap
               .GetString("interval"));
            var symbols = dataMap.GetString("symbols");
            var outputSize = dataMap.GetInt("outputSize");

            var deserializedSymbols = JsonSerializer.Deserialize<string[]>(symbols);
            var tasks = new List<Task<Stock>>();

            foreach (string symbol in deserializedSymbols)
            {
                var stock = _timeSeriesService.GetStockAsync(symbol, interval, outputSize);
                if (stock is not null) 
                {
                    tasks.Add(stock);
                }
            }

            var stocks = await Task.WhenAll(tasks);

            await _timeSeriesService.CreateStocksAsync(stocks);
        }

        catch (Exception ex) 
        {
            _logger.LogError("--- Error in job!");
            var wrappedException = new JobExecutionException(ex);
            wrappedException.UnscheduleAllTriggers = true;
            throw wrappedException;
        }
    }
}
