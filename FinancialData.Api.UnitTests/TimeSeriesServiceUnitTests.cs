using FinancialData.Api.Application.Services;
using FinancialData.Domain.Entities;
using FinancialData.Domain.Repositories;
using FinancialData.Common.Customizations;
using FinancialData.Common.Dtos;
using FinancialData.Common.Extensions;
using FinancialData.Api.Application.ErrorMessages;
using NSubstitute.ReturnsExtensions;
using Microsoft.Extensions.Logging;

namespace FinancialData.Api.UnitTests;

public class TimeSeriesServiceUnitTests
{
    [Fact]
    public async Task GetStockAsync_ShouldReturnStock()
    {
        //Arrange
        var logger = Substitute.For<ILogger<TimeSeriesService>>();
        var repository = Substitute.For<ITimeSeriesRepository>();
        var timeseriesService = new TimeSeriesService(logger, repository);

        var fixture = new Fixture();

        var stock = new Stock
        {
            Metadata = fixture.Build<Metadata>()
                .Without(m => m.Stock)
                .Create(),
            TimeSeries = fixture.Customize(new TimeSeriesCustomization())
                .CreateMany<TimeSeries>()
                .ToList()
        };

        var stockDto = new StockDto
        {
            Metadata = stock.Metadata.ToDto(),
            TimeSeries = stock.TimeSeries.Select(ts => ts.ToDto()).ToList()
        };

        repository.GetStockAsync(1, 5)
            .Returns(Task.FromResult(stock));

        //Act
        var result = await timeseriesService.GetStockAsync(1, 5);

        //Assert
        result.IsError.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.Payload.Should().BeEquivalentTo(stockDto);
    }

    [Fact]
    public async Task GetStockAsync_RetrievedUnSuccessfullyFromRepository()
    {
        //Arrange
        var logger = Substitute.For<ILogger<TimeSeriesService>>();
        var repository = Substitute.For<ITimeSeriesRepository>();
        var timeseriesService = new TimeSeriesService(logger, repository);

        repository.GetStockAsync(1, 5)
            .ReturnsNull();

        //Act
        var result = await timeseriesService.GetStockAsync(1, 5);

        //Assert
        result.IsError.Should().BeTrue();
        result.ErrorMessage.Should().Be(TimeSeriesErrorMessages.StockNotFound);
        result.Payload.Should().BeNull();
    }

    [Fact]
    public async Task GetTimeSeriesAsync_ShouldReturnTimeSeries()
    {
        //Arrange
        var logger = Substitute.For<ILogger<TimeSeriesService>>();
        var repository = Substitute.For<ITimeSeriesRepository>();
        var timeseriesService = new TimeSeriesService(logger, repository);

        var fixture = new Fixture();

        var timeseries = fixture.Customize(new TimeSeriesCustomization())
            .CreateMany<TimeSeries>();
        var timeseriesDto = timeseries.Select(ts => ts.ToDto());

        repository.GetTimeseriesAsync(1, 5)
            .Returns(Task.FromResult(timeseries));

        //Act
        var result = await timeseriesService.GetTimeSeriesAsync(1, 5);

        //Assert
        result.IsError.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.Payload.Should().BeEquivalentTo(timeseriesDto);
    }

    [Fact]
    public async Task GetTimeSeriesAsync_RetrievedUnSuccessfullyFromRepository()
    {
        //Arrange
        var logger = Substitute.For<ILogger<TimeSeriesService>>();
        var repository = Substitute.For<ITimeSeriesRepository>();
        var timeseriesService = new TimeSeriesService(logger, repository);

        repository.GetTimeseriesAsync(1, 5)
            .ReturnsNull();

        //Act
        var result = await timeseriesService.GetTimeSeriesAsync(1, 5);

        //Assert
        result.IsError.Should().BeTrue();
        result.ErrorMessage.Should().Be(TimeSeriesErrorMessages.TimeSeriesNotFound);
        result.Payload.Should().BeNull();
    }

    [Fact]
    public async Task GetMetadataAsync_ShouldReturnMetadata()
    {
        //Arrange
        var logger = Substitute.For<ILogger<TimeSeriesService>>();
        var repository = Substitute.For<ITimeSeriesRepository>();
        var timeseriesService = new TimeSeriesService(logger, repository);

        var fixture = new Fixture();

        var metadata = fixture.Build<Metadata>()
            .Without(m => m.Stock)
            .Create();
        var metadataDto = metadata.ToDto();

        repository.GetMetadataAsync(1)
            .Returns(Task.FromResult(metadata));

        //Act
        var result = await timeseriesService.GetMetadataAsync(1);

        //Assert
        result.IsError.Should().BeFalse();
        result.ErrorMessage.Should().BeNull();
        result.Payload.Should().BeEquivalentTo(metadataDto);
    }

    [Fact]
    public async Task GetMetadataAsync_RetrievedUnSuccessfullyFromRepository()
    {
        //Arrange
        var logger = Substitute.For<ILogger<TimeSeriesService>>();
        var repository = Substitute.For<ITimeSeriesRepository>();
        var timeseriesService = new TimeSeriesService(logger, repository);

        repository.GetMetadataAsync(1)
            .ReturnsNull();

        //Act
        var result = await timeseriesService.GetMetadataAsync(1);

        //Assert
        result.IsError.Should().BeTrue();
        result.ErrorMessage.Should().Be(TimeSeriesErrorMessages.MetaDataNotFound);
        result.Payload.Should().BeNull();
    }
}
