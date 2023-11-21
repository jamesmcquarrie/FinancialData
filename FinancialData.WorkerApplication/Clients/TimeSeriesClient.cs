using FinancialData.Domain.Enums;
using FinancialData.Common.Utilities;
using FinancialData.Common.Dtos;
using System.Text.Json;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace FinancialData.WorkerApplication.Clients;

public class TimeSeriesClient : ITimeSeriesClient
{
    private readonly ILogger<TimeSeriesClient> _logger;
    private HttpClient _httpClient;

    public TimeSeriesClient(ILogger<TimeSeriesClient> logger,
        HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task<StockDto> GetStockAsync(string symbol, Interval interval, int outputSize)
    {
        var endpoint = TimeSeriesEndpointBuilder.BuildTimeSeriesEndpoint(symbol, interval, outputSize);
        var response = await _httpClient.GetFromJsonAsync<StockDto>(endpoint, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });
        _logger.LogInformation("{0} called at {1}", endpoint, DateTime.Now.ToString());

        return response;
    }

    public async Task<IEnumerable<TimeSeriesDto>> GetTimeSeriesAsync(string symbol, Interval interval, int outputSize)
    {
        var endpoint = TimeSeriesEndpointBuilder.BuildTimeSeriesEndpoint(symbol, interval, outputSize);
        var response = await _httpClient.GetFromJsonAsync<StockDto>(endpoint, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });

        return response.TimeSeries;
    }
}
