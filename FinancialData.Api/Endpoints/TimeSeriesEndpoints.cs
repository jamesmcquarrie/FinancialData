using FinancialData.Api.Application.Services;

namespace FinancialData.Api.Endpoints;

public static class TimeSeriesEndpoints
{
    public static void AddTimeSeriesEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/timeseries-data/stocks/{Id}", async (int id, ITimeSeriesService timeseriesService, int outputsize = 5) =>
        {
            var serviceResult = await timeseriesService.GetStockAsync(id, outputsize);

            return serviceResult.IsError ? Results.BadRequest(serviceResult.ErrorMessage) : Results.Ok(serviceResult.Payload);
        })
        .WithName("GetStockById")
        .WithOpenApi();

        app.MapGet("/api/timeseries-data/timeseries{Id}", async (int id, ITimeSeriesService timeseriesService, int outputsize = 5) =>
        {
            var serviceResult = await timeseriesService.GetTimeSeriesAsync(id, outputsize);

            return serviceResult.IsError ? Results.BadRequest(serviceResult.ErrorMessage) : Results.Ok(serviceResult.Payload);
        })
        .WithName("GetTimeseriesByStockId")
        .WithOpenApi();

        app.MapGet("/api/timeseries-data/metadata/{Id}", async (int id, ITimeSeriesService timeseriesService) =>
        {
            var serviceResult = await timeseriesService.GetMetaDataAsync(id);

            return serviceResult.IsError ? Results.BadRequest(serviceResult.ErrorMessage) : Results.Ok(serviceResult.Payload);
        })
        .WithName("GetMetaDataById")
        .WithOpenApi();
    }
}
