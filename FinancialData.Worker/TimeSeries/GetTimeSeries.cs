using FinancialData.WorkerApplication.Services;
using FinancialData.Domain.Enums;
using Quartz;

namespace FinancialData.Worker.TimeSeries;

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
        var dataMap = context.MergedJobDataMap;

        var symbol = dataMap.GetString("symbol");
        var interval = Interval.FromName(dataMap
            .GetString("interval"));
        var outputSize = dataMap.GetInt("outputSize");

        await _timeSeriesService.AddTimeSeriesAsync(symbol, interval, outputSize);
        _logger.LogInformation("new timeseries data has been added to the {0} stock with interval: {1}", symbol, interval);
    }
}
