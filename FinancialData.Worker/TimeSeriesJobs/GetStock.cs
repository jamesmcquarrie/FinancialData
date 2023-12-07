using FinancialData.WorkerApplication.Services;
using System.Text.Json;
using Quartz;
using FinancialData.Common.Configuration;

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
            var timeseriesArgsString = dataMap.GetString("timeseriesOptions");
            var timeseriesArgs = JsonSerializer.Deserialize<TimeSeriesArguments[]>(timeseriesArgsString);

            var stocks = await _timeSeriesService.GetStocksAsync(timeseriesArgs);

            if (stocks.Any())
            {
                await _timeSeriesService.CreateStocksAsync(stocks);
            }
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
