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
        _logger.LogInformation("\nITimeSeriesClient.GetStockAsync called for symbol: {} interval: {}\n", symbol, interval.Name);

        var endpoint = TimeSeriesEndpointBuilder.BuildTimeSeriesEndpoint(symbol, interval, outputSize);
        HttpResponseMessage response = null;
        string stockDtoString = null;
        StockDto stockDto = null;

        try
        {
            _logger.LogInformation("api call made at {}", DateTime.Now.ToString());
            response = await _httpClient.GetAsync(endpoint);
            stockDtoString = await response.Content.ReadAsStringAsync();
            stockDto = JsonSerializer.Deserialize<StockDto>(stockDtoString, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });
        }

        catch (Exception ex) 
        {
            _logger.LogError(ex, "error encountered for symbol: {} interval: {} - EXCEPTION MESSAGE: {}", symbol, interval.Name, ex.Message);
            _logger.LogError(stockDtoString);
        }

        _logger.LogInformation("symbol: {} interval: {} retrieved", symbol, interval.Name);
        return stockDto;
    }

    public async Task<IEnumerable<TimeSeriesDto>> GetTimeSeriesAsync(string symbol, Interval interval, int outputSize)
    {
        _logger.LogInformation("\nITimeSeriesClient.GetTimeSeriesAsync called for symbol: {} interval: {}\n", symbol, interval.Name);

        var endpoint = TimeSeriesEndpointBuilder.BuildTimeSeriesEndpoint(symbol, interval, outputSize);
        HttpResponseMessage response = null;
        string stockDtoString = null;
        StockDto stockDto = null;

        try
        {
            _logger.LogInformation("api call made at {}", DateTime.Now.ToString());
            response = await _httpClient.GetAsync(endpoint);
            stockDtoString = await response.Content.ReadAsStringAsync();
            stockDto = JsonSerializer.Deserialize<StockDto>(stockDtoString, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "error encountered for symbol: {} interval: {} - EXCEPTION MESSAGE: {}", symbol, interval.Name, ex.Message);
            _logger.LogError(stockDtoString);
        }

        return stockDto.TimeSeries;
    }
}
