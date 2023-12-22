using FinancialData.Worker.Application.Services;
using FinancialData.Common.Configuration;
using FinancialData.Domain.Entities;
using FinancialData.Domain.Enums;
using System.Text.Json;
using Quartz;

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
            var timeseriesArgsString = dataMap.GetString("timeseriesArguments");
            var timeseriesArgs = JsonSerializer.Deserialize<TimeSeriesArguments[]>(timeseriesArgsString);
            var timeSeries = new List<TimeSeries>();

            foreach (var arg in timeseriesArgs)
            {
                var timeSeriesForArg = await _timeSeriesService.GetTimeSeriesAsync(arg.Symbol, Interval.FromName(arg.Interval), arg.OutputSize);
                timeSeries.AddRange(timeSeriesForArg);
            }

            if (timeSeries.Any())
            {
                await _timeSeriesService.CreateTimeSeriesAsync(timeSeries);
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
