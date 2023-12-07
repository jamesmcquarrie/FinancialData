using FinancialData.WorkerApplication.Services;
using FinancialData.Domain.Enums;
using System.Text.Json;
using Quartz;
using FinancialData.Common.Configuration;

namespace FinancialData.Worker.TimeSeriesJobs;

public class GetTimeSeries : IJob
{
    private readonly ILogger<GetTimeSeries> _logger;
    private readonly ITimeSeriesScheduledService _timeSeriesService;

    public GetTimeSeries(ILogger<GetTimeSeries> logger,
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

            var timeseriesDictionary = await _timeSeriesService.GetTimeSeriesAsync(timeseriesArgs);

            foreach (var entry in timeseriesDictionary) 
            {
                var arg = entry.Key;
                var timeseries = entry.Value;

                await _timeSeriesService.AddTimeSeriesToStockAsync(arg.Symbol, Interval.FromName(arg.Interval), timeseries);
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
