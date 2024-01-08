using FinancialData.Worker.Application.Services;
using FinancialData.Infrastructure.Options;
using FinancialData.Common.Configuration;
using FinancialData.Domain.Enums;
using FinancialData.Domain.Entities;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Quartz;

namespace FinancialData.Worker.TimeSeriesJobs;

public class GetStock : IJob
{
    private readonly ILogger<GetStock> _logger;
    private readonly ITimeSeriesScheduledService _timeSeriesService;
    private readonly TimeSeriesArgumentsOptions _timeSeriesArgumentsOptions;
    private const string TimeSeriesArgumentsFile = "timeseriesArguments.json";

    public GetStock(ILogger<GetStock> logger,
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

            var stocks = new List<Stock>();

            foreach (var arg in timeseriesArgs)
            {
                var stock = await _timeSeriesService.GetStockAsync(arg.Symbol, Interval.FromName(arg.Interval), arg.OutputSize);
                if (stock is not null) stocks.Add(stock);
            }

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

    private IEnumerable<TimeSeriesArguments> GetTimeSeriesArguments()
    {
        var path = Path.Combine(_timeSeriesArgumentsOptions.ArgumentsPath, TimeSeriesArgumentsFile);
        var timeseriesArgumentsString = File.ReadAllText(path);
        var timeseriesArgs = JsonSerializer.Deserialize<TimeSeriesArguments[]>(timeseriesArgumentsString);

        return timeseriesArgs;
    }
}
