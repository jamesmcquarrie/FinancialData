﻿using FinancialData.WorkerApplication.Services;
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
        var dataMap = context.MergedJobDataMap;

        var symbols = dataMap.GetString("symbols");
        var interval = Interval.FromName(dataMap
            .GetString("interval"));
        var outputSize = dataMap.GetInt("outputSize");

        var deserializedSymbols = JsonSerializer.Deserialize<string[]>(symbols);
        var tasks = new List<Task<IEnumerable<TimeSeries>>>();

        foreach (var symbol in deserializedSymbols) 
        {
            tasks.Add(_timeSeriesService.GetTimeSeriesAsync(symbol, interval, outputSize));
        }

        var timeseriesList = await Task.WhenAll(tasks);

        for (int i = 0; i < deserializedSymbols.Length; i++) 
        {
            await _timeSeriesService.AddMultipleTimeSeriesToStockAsync(deserializedSymbols[i], interval, timeseriesList[i]);
        }
    }
}
