using FinancialData.Domain.Enums;
using FinancialData.Common.Utilities;
using FinancialData.Common.Dtos;
using System.Text.Json;
using System.Net;
using System.Net.Http.Json;
using FinancialData.WorkerApplication.Abstractions;
using FinancialData.WorkerApplication.StatusMessages;
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

    public async Task<ClientResult<StockDto>> GetStockAsync(string symbol, Interval interval, int outputSize)
    {
        var endpoint = TimeSeriesEndpointBuilder.BuildTimeSeriesEndpoint(symbol, interval, outputSize);
        var response = await _httpClient.GetAsync(endpoint);

        var result = HandleResponse<StockDto>(response);

        if (!result.IsError) 
        {
            var stockDto = await response.Content
                .ReadFromJsonAsync<StockDto>(new JsonSerializerOptions()
                { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });

            _logger.LogInformation("Stock for symbol: {} interval: {} retrieved", symbol, interval.Name);

            result.Result = stockDto;
        }

        else
        {
            _logger.LogError("Stock for symbol: {} interval: {} not retrieved - ERROR MESSAGE: {}", symbol, interval.Name, result.ErrorMessage);
        }

        return result;
    }

    public async Task<ClientResult<IEnumerable<TimeSeriesDto>>> GetTimeSeriesAsync(string symbol, Interval interval, int outputSize)
    {
        var endpoint = TimeSeriesEndpointBuilder.BuildTimeSeriesEndpoint(symbol, interval, outputSize);
        var response = await _httpClient.GetAsync(endpoint);

        var result = HandleResponse<IEnumerable<TimeSeriesDto>>(response);

        if (!result.IsError) 
        {
            var stockDto = await response.Content
                .ReadFromJsonAsync<StockDto>(new JsonSerializerOptions()
                { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });

            _logger.LogInformation("Timeseries for symbol: {} interval: {} retrieved", symbol, interval.Name);

            result.Result = stockDto!.TimeSeries;
        }

        else
        {
            _logger.LogError("Timeseries for symbol: {} interval: {} not retrieved - ERROR MESSAGE: {}", symbol, interval.Name, result.ErrorMessage);
        }


        return result;
    }

    private ClientResult<T> HandleResponse<T>(HttpResponseMessage response) where T : class
    {
        var result = response.StatusCode switch
        {
            HttpStatusCode.TooManyRequests => new ClientResult<T>
            {
                IsError = true,
                ErrorMessage = TwelveDataStatusMessages.TooManyRequestsMessage
            },
            HttpStatusCode.Unauthorized => new ClientResult<T>
            {
                IsError = true,
                ErrorMessage = TwelveDataStatusMessages.UnauthorisedMessage
            },
            HttpStatusCode.NotFound => new ClientResult<T>
            {
                IsError = true,
                ErrorMessage = TwelveDataStatusMessages.NotFoundMessage
            },
            HttpStatusCode.BadRequest => new ClientResult<T>
            {
                IsError = true,
                ErrorMessage = TwelveDataStatusMessages.BadRequestMessage
            },
            _ => new ClientResult<T>()
        };

        return result;
    }
}
