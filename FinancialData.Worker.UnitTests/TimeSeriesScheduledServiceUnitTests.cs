using FinancialData.Common.Abstractions;
using FinancialData.Common.Configuration;
using FinancialData.Common.Dtos;
using FinancialData.Common.Extensions;
using FinancialData.Domain.Entities;
using FinancialData.Domain.Enums;
using FinancialData.Worker.Application.Clients;
using FinancialData.Worker.Application.Repositories;
using FinancialData.Worker.Application.Services;
using FinancialData.Worker.Application.StatusMessages;
using FinancialData.Worker.UnitTests.Customizations;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;

namespace FinancialData.Worker.UnitTests;

public class TimeSeriesScheduledServiceUnitTests
{
    [Fact]
    public async Task GetStocksAsync_RetrievedSuccessfullyFromApi()
    {
        //Arrange
        var logger = Substitute.For<ILogger<TimeSeriesScheduledService>>();
        var client = Substitute.For<ITimeSeriesClient>();
        var repository = Substitute.For<ITimeSeriesScheduledRepository>();
        var timeseriesScheduledService = new TimeSeriesScheduledService(logger, client, repository);

        var fixture = new Fixture();

        var timeseriesArgs = fixture.Customize(new TimeSeriesArgumentsCustomization())
                .Create<TimeSeriesArguments>();

        var serviceResult = new ServiceResult<StockDto>
        {
            Payload = new StockDto
            {
                Metadata = fixture.Customize(new MetadataDtoCustomization(timeseriesArgs))
                    .Create<MetadataDto>(),
                TimeSeries = fixture.Customize(new TimeSeriesDtoCustomization())
                    .CreateMany<TimeSeriesDto>()
                    .ToList()
            }
        };

        repository.GetStockAsync(Arg.Any<string>(), Arg.Any<Interval>())
           .ReturnsNull();

        client.GetStockAsync(Arg.Any<string>(), Arg.Any<Interval>(), Arg.Any<int>())
            .Returns(Task.FromResult(serviceResult));

        var stock = new Stock
        {
            Metadata = serviceResult.Payload.Metadata.ToEntity(),
            TimeSeries = serviceResult.Payload.TimeSeries.Select(ts => ts.ToEntity()).ToList()
        };

        //Act
        var result = await timeseriesScheduledService.GetStocksAsync(new TimeSeriesArguments[] { timeseriesArgs });

        //Assert
        result.Should().BeEquivalentTo(new Stock[] { stock });
    }

    [Theory]
    [InlineData(TwelveDataStatusMessages.TooManyRequestsMessage)]
    [InlineData(TwelveDataStatusMessages.UnauthorisedMessage)]
    [InlineData(TwelveDataStatusMessages.NotFoundMessage)]
    [InlineData(TwelveDataStatusMessages.BadRequestMessage)]
    public async Task GetStocksAsync_RetrievedUnSuccessfullyFromApi(string errorMessage)
    {
        //Arrange
        var logger = Substitute.For<ILogger<TimeSeriesScheduledService>>();
        var client = Substitute.For<ITimeSeriesClient>();
        var repository = Substitute.For<ITimeSeriesScheduledRepository>();
        var timeseriesScheduledService = new TimeSeriesScheduledService(logger, client, repository);

        var fixture = new Fixture();

        var timeseriesArgs = fixture.Customize(new TimeSeriesArgumentsCustomization())
                .Create<TimeSeriesArguments>();

        var serviceResult = new ServiceResult<StockDto>
        {
            IsError = true,
            ErrorMessage = errorMessage
        };

        repository.GetStockAsync(Arg.Any<string>(), Arg.Any<Interval>())
           .ReturnsNull();

        client.GetStockAsync(Arg.Any<string>(), Arg.Any<Interval>(), Arg.Any<int>())
            .Returns(Task.FromResult(serviceResult));

        //Act
        var result = await timeseriesScheduledService.GetStocksAsync(new TimeSeriesArguments[] { timeseriesArgs });

        //Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetStocksAsync_RetrievedSuccessfullyFromRepository()
    {
        //Arrange
        var logger = Substitute.For<ILogger<TimeSeriesScheduledService>>();
        var client = Substitute.For<ITimeSeriesClient>();
        var repository = Substitute.For<ITimeSeriesScheduledRepository>();
        var timeseriesScheduledService = new TimeSeriesScheduledService(logger, client, repository);

        var fixture = new Fixture();

        var timeseriesArgs = fixture.Customize(new TimeSeriesArgumentsCustomization())
                .Create<TimeSeriesArguments>();

        var stock = new Stock
        {
            Metadata = fixture.Customize(new MetadataCustomization(timeseriesArgs))
                .Create<Metadata>(),
            TimeSeries = fixture.Customize(new TimeSeriesCustomization())
                .CreateMany<TimeSeries>()
                .ToList()
        };

        repository.GetStockAsync(Arg.Any<string>(), Arg.Any<Interval>())
           .Returns(Task.FromResult(stock));

        //Act
        var result = await timeseriesScheduledService.GetStocksAsync(new TimeSeriesArguments[] { timeseriesArgs });

        //Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetStocksAsync_ShouldThrowTaskCancelledException()
    {
        //Arrange
        var logger = Substitute.For<ILogger<TimeSeriesScheduledService>>();
        var client = Substitute.For<ITimeSeriesClient>();
        var repository = Substitute.For<ITimeSeriesScheduledRepository>();
        var timeseriesScheduledService = new TimeSeriesScheduledService(logger, client, repository);

        var fixture = new Fixture();

        var timeseriesArgs = fixture.Customize(new TimeSeriesArgumentsCustomization())
                .Create<TimeSeriesArguments>();

        client.GetStockAsync(Arg.Any<string>(), Arg.Any<Interval>(), Arg.Any<int>())
            .ThrowsAsync<TaskCanceledException>();

        //Act
        Func<Task> act = async () => await timeseriesScheduledService.GetStocksAsync(new TimeSeriesArguments[] { timeseriesArgs });

        //Assert
        await act.Should().ThrowAsync<TaskCanceledException>();
    }

    [Fact]
    public async Task GetStocksAsync_ShouldThrowHttpRequestException()
    {
        //Arrange
        var logger = Substitute.For<ILogger<TimeSeriesScheduledService>>();
        var client = Substitute.For<ITimeSeriesClient>();
        var repository = Substitute.For<ITimeSeriesScheduledRepository>();
        var timeseriesScheduledService = new TimeSeriesScheduledService(logger, client, repository);

        var fixture = new Fixture();

        var timeseriesArgs = fixture.Customize(new TimeSeriesArgumentsCustomization())
                .Create<TimeSeriesArguments>();

        client.GetStockAsync(Arg.Any<string>(), Arg.Any<Interval>(), Arg.Any<int>())
            .ThrowsAsync<HttpRequestException>();

        //Act
        Func<Task> act = async () => await timeseriesScheduledService.GetStocksAsync(new TimeSeriesArguments[] { timeseriesArgs });

        //Assert
        await act.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task GetStocksAsync_ShouldThrowGeneralException()
    {
        //Arrange
        var logger = Substitute.For<ILogger<TimeSeriesScheduledService>>();
        var client = Substitute.For<ITimeSeriesClient>();
        var repository = Substitute.For<ITimeSeriesScheduledRepository>();
        var timeseriesScheduledService = new TimeSeriesScheduledService(logger, client, repository);

        var fixture = new Fixture();

        var timeseriesArgs = fixture.Customize(new TimeSeriesArgumentsCustomization())
                .Create<TimeSeriesArguments>();

        client.GetStockAsync(Arg.Any<string>(), Arg.Any<Interval>(), Arg.Any<int>())
            .ThrowsAsync<Exception>();

        //Act
        Func<Task> act = async () => await timeseriesScheduledService.GetStocksAsync(new TimeSeriesArguments[] { timeseriesArgs });

        //Assert
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task GetTimeSeriesAsync_RetrievedSuccessfullyFromApi()
    {
        //Arrange
        var logger = Substitute.For<ILogger<TimeSeriesScheduledService>>();
        var client = Substitute.For<ITimeSeriesClient>();
        var repository = Substitute.For<ITimeSeriesScheduledRepository>();
        var timeseriesScheduledService = new TimeSeriesScheduledService(logger, client, repository);

        var fixture = new Fixture();

        var timeseriesArgs = fixture.Customize(new TimeSeriesArgumentsCustomization())
                .Create<TimeSeriesArguments>();            

        var serviceResult = new ServiceResult<IEnumerable<TimeSeriesDto>>
        {
            Payload = fixture.Customize(new TimeSeriesDtoCustomization())
                    .CreateMany<TimeSeriesDto>(3)
                    .ToList()
        };

        var timeseriesDictionary = new Dictionary<TimeSeriesArguments, IEnumerable<TimeSeries>>
        {
            {
                timeseriesArgs,
                serviceResult.Payload.Select(ts => ts.ToEntity())
            }
        };

        var existingTimeseries = serviceResult.Payload.Take(1).Select(ts => ts.ToEntity());

        repository.GetTimeSeriesAsync(Arg.Any<string>(), Arg.Any<Interval>())
           .Returns(Task.FromResult(existingTimeseries));

        client.GetTimeSeriesAsync(Arg.Any<string>(), Arg.Any<Interval>(), Arg.Any<int>())
            .Returns(Task.FromResult(serviceResult));

        //Act
        var result = await timeseriesScheduledService.GetTimeSeriesAsync(new TimeSeriesArguments[] { timeseriesArgs });

        //Assert
        result[timeseriesArgs].Should().HaveCount(2);
        result[timeseriesArgs].Should().BeEquivalentTo(timeseriesDictionary[timeseriesArgs].Skip(1));
    }

    [Theory]
    [InlineData(TwelveDataStatusMessages.TooManyRequestsMessage)]
    [InlineData(TwelveDataStatusMessages.UnauthorisedMessage)]
    [InlineData(TwelveDataStatusMessages.NotFoundMessage)]
    [InlineData(TwelveDataStatusMessages.BadRequestMessage)]
    public async Task GetTimeSeriesAsync_RetrievedUnSuccessfullyFromApi(string errorMessage)
    {
        //Arrange
        var logger = Substitute.For<ILogger<TimeSeriesScheduledService>>();
        var client = Substitute.For<ITimeSeriesClient>();
        var repository = Substitute.For<ITimeSeriesScheduledRepository>();
        var timeseriesScheduledService = new TimeSeriesScheduledService(logger, client, repository);

        var fixture = new Fixture();

        var timeseriesArgs = fixture.Customize(new TimeSeriesArgumentsCustomization())
                .Create<TimeSeriesArguments>();

        var serviceResult = new ServiceResult<StockDto>
        {
            IsError = true,
            ErrorMessage = errorMessage
        };

        client.GetStockAsync(Arg.Any<string>(), Arg.Any<Interval>(), Arg.Any<int>())
            .Returns(Task.FromResult(serviceResult));

        //Act
        var result = await timeseriesScheduledService.GetStocksAsync(new TimeSeriesArguments[] { timeseriesArgs });

        //Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTimeSeriesAsync_ShouldThrowTaskCancelledException()
    {
        //Arrange
        var logger = Substitute.For<ILogger<TimeSeriesScheduledService>>();
        var client = Substitute.For<ITimeSeriesClient>();
        var repository = Substitute.For<ITimeSeriesScheduledRepository>();
        var timeseriesScheduledService = new TimeSeriesScheduledService(logger, client, repository);

        var fixture = new Fixture();

        var timeseriesArgs = fixture.Customize(new TimeSeriesArgumentsCustomization())
                .Create<TimeSeriesArguments>();

        client.GetTimeSeriesAsync(Arg.Any<string>(), Arg.Any<Interval>(), Arg.Any<int>())
            .ThrowsAsync<TaskCanceledException>();

        //Act
        Func<Task> act = async () => await timeseriesScheduledService.GetTimeSeriesAsync(new TimeSeriesArguments[] { timeseriesArgs });

        //Assert
        await act.Should().ThrowAsync<TaskCanceledException>();
    }

    [Fact]
    public async Task GetTimeSeriesAsync_ShouldThrowHttpRequestException()
    {
        //Arrange
        var logger = Substitute.For<ILogger<TimeSeriesScheduledService>>();
        var client = Substitute.For<ITimeSeriesClient>();
        var repository = Substitute.For<ITimeSeriesScheduledRepository>();
        var timeseriesScheduledService = new TimeSeriesScheduledService(logger, client, repository);

        var fixture = new Fixture();

        var timeseriesArgs = fixture.Customize(new TimeSeriesArgumentsCustomization())
                .Create<TimeSeriesArguments>();

        client.GetTimeSeriesAsync(Arg.Any<string>(), Arg.Any<Interval>(), Arg.Any<int>())
            .ThrowsAsync<HttpRequestException>();

        //Act
        Func<Task> act = async () => await timeseriesScheduledService.GetTimeSeriesAsync(new TimeSeriesArguments[] { timeseriesArgs });

        //Assert
        await act.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task GetTimeSeriesAsync_ShouldThrowGeneralException()
    {
        //Arrange
        var logger = Substitute.For<ILogger<TimeSeriesScheduledService>>();
        var client = Substitute.For<ITimeSeriesClient>();
        var repository = Substitute.For<ITimeSeriesScheduledRepository>();
        var timeseriesScheduledService = new TimeSeriesScheduledService(logger, client, repository);

        var fixture = new Fixture();

        var timeseriesArgs = fixture.Customize(new TimeSeriesArgumentsCustomization())
                .Create<TimeSeriesArguments>();

        client.GetTimeSeriesAsync(Arg.Any<string>(), Arg.Any<Interval>(), Arg.Any<int>())
            .ThrowsAsync<Exception>();

        //Act
        Func<Task> act = async () => await timeseriesScheduledService.GetTimeSeriesAsync(new TimeSeriesArguments[] { timeseriesArgs });

        //Assert
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task CreateStocksAsync_ShouldReturnTask()
    {
        //Arrange
        var logger = Substitute.For<ILogger<TimeSeriesScheduledService>>();
        var client = Substitute.For<ITimeSeriesClient>();
        var repository = Substitute.For<ITimeSeriesScheduledRepository>();
        var timeseriesScheduledService = new TimeSeriesScheduledService(logger, client, repository);

        var fixture = new Fixture();

        var timeseriesArgs = fixture.Customize(new TimeSeriesArgumentsCustomization())
                .Create<TimeSeriesArguments>();

        var stock = new Stock
        {
            Metadata = fixture.Customize(new MetadataCustomization(timeseriesArgs))
                .Create<Metadata>(),
            TimeSeries = fixture.Customize(new TimeSeriesCustomization())
                .CreateMany<TimeSeries>()
                .ToList()
        };

        var stocks = new Stock[] { stock };

        //Act
        await timeseriesScheduledService.CreateStocksAsync(stocks);

        //Assert
        await repository.Received().CreateStocksAsync(stocks);
    }

    [Fact]
    public async Task AddTimeSeriesToStockAsync_ShouldReturnTask()
    {
        //Arrange
        var logger = Substitute.For<ILogger<TimeSeriesScheduledService>>();
        var client = Substitute.For<ITimeSeriesClient>();
        var repository = Substitute.For<ITimeSeriesScheduledRepository>();
        var timeseriesScheduledService = new TimeSeriesScheduledService(logger, client, repository);

        var fixture = new Fixture();

        var timeseriesArgs = fixture.Customize(new TimeSeriesArgumentsCustomization())
                .Create<TimeSeriesArguments>();

        var timeseries = fixture.Customize(new TimeSeriesCustomization())
            .CreateMany<TimeSeries>();

        //Act
        await timeseriesScheduledService.AddTimeSeriesToStockAsync(timeseriesArgs.Symbol, Interval.FromName(timeseriesArgs.Interval), timeseries);

        //Assert
        await repository.Received().AddTimeSeriesToStockAsync(timeseriesArgs.Symbol, Interval.FromName(timeseriesArgs.Interval), timeseries);
    }
}
