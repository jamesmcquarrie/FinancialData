using FinancialData.Common.Abstractions;
using FinancialData.Common.Dtos;
using FinancialData.Common.Extensions;
using FinancialData.Domain.Repositories;
using FinancialData.Api.Application.ErrorMessages;
using Microsoft.Extensions.Logging;

namespace FinancialData.Api.Application.Services;

public class TimeSeriesService : ITimeSeriesService
{
    private readonly ILogger<TimeSeriesService> _logger;
    private readonly ITimeSeriesRepository _timeSeriesRepository;

    public TimeSeriesService(ILogger<TimeSeriesService> logger,
        ITimeSeriesRepository timeSeriesRepository)
    {
        _logger = logger;
        _timeSeriesRepository = timeSeriesRepository;
    }

    public async Task<ServiceResult<StockDto>> GetStockAsync(int id, int timeseriesOutputSize)
    {
        var serviceResult = new ServiceResult<StockDto>();

        var stock = await _timeSeriesRepository.GetStockAsync(id, timeseriesOutputSize);

        if (stock is null)
        {
            _logger.LogInformation("Stock does not exist in the database");

            serviceResult.IsError = true;
            serviceResult.ErrorMessage = TimeSeriesErrorMessages.StockNotFound;

            return serviceResult;
        }

        _logger.LogInformation("Stock has been retrieved from database");

        var stockDto = new StockDto
        {
            MetaData = stock.MetaData.ToDto(),
            TimeSeries = stock.TimeSeries.Select(ts => ts.ToDto())
                .ToList()
        };

        serviceResult.Payload = stockDto;
        return serviceResult;
    }

    public async Task<ServiceResult<IEnumerable<TimeSeriesDto>>> GetTimeSeriesAsync(int id, int timeseriesOutputSize)
    {
        var serviceResult = new ServiceResult<IEnumerable<TimeSeriesDto>>();

        var timeseries = await _timeSeriesRepository.GetTimeseriesAsync(id, timeseriesOutputSize);

        if (timeseries is null)
        {
            _logger.LogInformation("Timeseries data does not exist in the database");

            serviceResult.IsError = true;
            serviceResult.ErrorMessage = TimeSeriesErrorMessages.TimeSeriesNotFound;

            return serviceResult;
        }

        _logger.LogInformation("Timeseries has been retrieved from database");

        serviceResult.Payload = timeseries.Select(ts => ts.ToDto());
        return serviceResult;
    }

    public async Task<ServiceResult<MetaDataDto>> GetMetaDataAsync(int id)
    {
        var serviceResult = new ServiceResult<MetaDataDto>();

        var metadata = await _timeSeriesRepository.GetMetaDataAsync(id);

        if (metadata is null)
        {
            _logger.LogInformation("Metadata data does not exist in the database");

            serviceResult.IsError = true;
            serviceResult.ErrorMessage = TimeSeriesErrorMessages.MetaDataNotFound;

            return serviceResult;
        }

        _logger.LogInformation("Metadata has been retrieved from database");

        var metadataDto = metadata.ToDto();

        serviceResult.Payload = metadataDto;
        return serviceResult;
    }
}
