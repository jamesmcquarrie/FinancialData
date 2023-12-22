using FinancialData.Worker.Application.Services;
using FinancialData.Common.Configuration;
using FinancialData.Domain.Enums;
using FinancialData.Domain.Entities;
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
            var timeseriesArgsString = dataMap.GetString("timeseriesArguments");
            var timeseriesArgs = JsonSerializer.Deserialize<TimeSeriesArguments[]>(timeseriesArgsString);
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
}
