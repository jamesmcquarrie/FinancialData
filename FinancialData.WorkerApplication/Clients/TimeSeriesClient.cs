using FinancialData.Domain.Enums;
using FinancialData.Common.Utilities;
using FinancialData.Common.Dtos;
using System.Text.Json;
using System.Net;
using System.Net.Http.Json;
using FinancialData.WorkerApplication.Abstractions;
using FinancialData.WorkerApplication.StatusMessages;

namespace FinancialData.WorkerApplication.Clients;

public class TimeSeriesClient : ITimeSeriesClient
{
    private readonly HttpClient _httpClient;

    public TimeSeriesClient(HttpClient httpClient)
    {
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

            result.Payload = stockDto;
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

            result.Payload = stockDto!.TimeSeries;
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
