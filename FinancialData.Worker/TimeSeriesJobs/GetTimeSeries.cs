using FinancialData.Worker.Application.Services;
using FinancialData.Worker.Options;
using FinancialData.Common.Configuration;
using FinancialData.Domain.Entities;
using FinancialData.Domain.Enums;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Quartz;

namespace FinancialData.Worker.TimeSeriesJobs;

public class GetTimeSeries : IJob
{
    private readonly ILogger<GetTimeSeries> _logger;
    private readonly ITimeSeriesScheduledService _timeSeriesService;
    private readonly TimeSeriesArgumentsOptions _timeSeriesArgumentsOptions;
    private const string TimeSeriesArgumentsFile = "timeseriesArguments.json";

    public GetTimeSeries(ILogger<GetTimeSeries> logger,
        ITimeSeriesScheduledService timeSeriesService,
        IOptions<TimeSeriesArgumentsOptions> timeSeriesArgumentsOptions)
    {
        _logger = logger;
        _timeSeriesService = timeSeriesService;
        _timeSeriesArgumentsOptions = timeSeriesArgumentsOptions.Value;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var timeseriesArgs = GetTimeSeriesArguments();

            var timeseries = new List<TimeSeries>();

            foreach (var arg in timeseriesArgs)
            {
                var timeSeriesForArg = await _timeSeriesService.GetTimeSeriesAsync(arg.Symbol, Interval.FromName(arg.Interval), arg.OutputSize);
                timeseries.AddRange(timeSeriesForArg);
            }

            if (timeseries.Any())
            {
                await _timeSeriesService.CreateTimeSeriesAsync(timeseries);
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

    private IEnumerable<TimeSeriesArguments> GetTimeSeriesArguments()
    {
        var path = Path.Combine(_timeSeriesArgumentsOptions.ArgumentsPath, TimeSeriesArgumentsFile);
        var timeseriesArgumentsString = File.ReadAllText(path);
        var timeseriesArgs = JsonSerializer.Deserialize<TimeSeriesArguments[]>(timeseriesArgumentsString);

        return timeseriesArgs;
    }
}
