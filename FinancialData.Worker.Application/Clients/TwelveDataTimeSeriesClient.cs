﻿using FinancialData.Worker.Application.StatusMessages;
using FinancialData.Worker.Application.Factories;
using FinancialData.Domain.Enums;
using FinancialData.Common.Abstractions;
using FinancialData.Common.Dtos;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace FinancialData.Worker.Application.Clients;

public class TwelveDataTimeSeriesClient : ITimeSeriesClient
{
    private readonly HttpClient _httpClient;
    private readonly ITimeSeriesEndpointFactory _endpointFactory;

    public TwelveDataTimeSeriesClient(
        HttpClient httpClient,
        ITimeSeriesEndpointFactory timeSeriesEndpointFactory)
    {
        _httpClient = httpClient;
        _endpointFactory = timeSeriesEndpointFactory;
    }

    public async Task<ServiceResult<StockDto>> GetStockAsync(string symbol, Interval interval, int outputSize)
    {
        var endpoint = _endpointFactory.Create(symbol, interval, outputSize);
        var response = await _httpClient.GetAsync(endpoint);

        var result = HandleResponse<StockDto>(response);

        if (!result.IsError) 
        {
            var twelveDataStockDto = await response.Content
                .ReadFromJsonAsync<TwelveDataStockDto>(new JsonSerializerOptions()
                { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });

            result.Payload = twelveDataStockDto;
        }

        return result;
    }

    public async Task<ServiceResult<IEnumerable<TimeSeriesDto>>> GetTimeSeriesAsync(string symbol, Interval interval, int outputSize)
    {
        var endpoint = _endpointFactory.Create(symbol, interval, outputSize);
        var response = await _httpClient.GetAsync(endpoint);

        var result = HandleResponse<IEnumerable<TimeSeriesDto>>(response);

        if (!result.IsError) 
        {
            var stockDto = await response.Content
                .ReadFromJsonAsync<TwelveDataStockDto>(new JsonSerializerOptions()
                { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });

            result.Payload = stockDto!.TimeSeries;
        }

        return result;
    }

    private ServiceResult<T> HandleResponse<T>(HttpResponseMessage response) where T : class
    {
        var result = response.StatusCode switch
        {
            HttpStatusCode.TooManyRequests => new ServiceResult<T>
            {
                IsError = true,
                ErrorMessage = TwelveDataStatusMessages.TooManyRequestsMessage
            },
            HttpStatusCode.Unauthorized => new ServiceResult<T>
            {
                IsError = true,
                ErrorMessage = TwelveDataStatusMessages.UnauthorisedMessage
            },
            HttpStatusCode.NotFound => new ServiceResult<T>
            {
                IsError = true,
                ErrorMessage = TwelveDataStatusMessages.NotFoundMessage
            },
            HttpStatusCode.BadRequest => new ServiceResult<T>
            {
                IsError = true,
                ErrorMessage = TwelveDataStatusMessages.BadRequestMessage
            },
            _ => new ServiceResult<T>()
        };

        return result;
    }
}
