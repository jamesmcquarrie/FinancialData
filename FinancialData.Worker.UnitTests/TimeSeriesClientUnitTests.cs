using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Castle.Core.Logging;
using FinancialData.Common.Dtos;
using FinancialData.Domain.Enums;
using FinancialData.WorkerApplication.Clients;
using FinancialData.WorkerApplication.StatusMessages;
using Microsoft.Extensions.Logging;
using RichardSzalay.MockHttp;

namespace FinancialData.Worker.UnitTests;

public class TimeSeriesClientUnitTests
{
    [Fact]
    public async Task GetStockAsync_GetsSuccessfully()
    {
        //Arrange
        var logger = Substitute.For<ILogger<TimeSeriesClient>>();
        var mockHttpClient = new MockHttpMessageHandler();
        var uri = "https://api.twelvedata.com/time_series?symbol=MSFT&interval=5min&outputsize=2";

        var metaDataDto = new MetadataDto
        {
            Symbol = "MSFT",
            Type = "Common Stock",
            Currency = "USD",
            Exchange = "NASDAQ",
            ExchangeTimezone = "America/New_York",
            MicCode = "XNGS",
            Interval = "5min"
        };

        var timeseriesDto = new List<TimeSeriesDto>()
        {
            new TimeSeriesDto
            {
                Datetime = "2023-11-28 15:55:00",
                High = "382.76001",
                Low = "382.03000",
                Open = "382.03000",
                Close = "382.70001",
                Volume = "1118579"
            },
            new TimeSeriesDto
            {
                Datetime = "2023-11-28 15:50:00",
                High = "382.04999",
                Low = "381.26999",
                Open = "381.29001",
                Close = "382.03000",
                Volume = "501357"
            }
        };

        var stockDto = new StockDto
        {
            Metadata = metaDataDto,
            TimeSeries = timeseriesDto,
        };

        mockHttpClient.When(uri)
            .Respond(HttpStatusCode.OK, JsonContent.Create(stockDto,
                options: new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower }));

        var httpClient = mockHttpClient.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://api.twelvedata.com/");
        httpClient.Timeout = TimeSpan.FromMinutes(5);

        var timeSeriesClient = new TimeSeriesClient(logger, httpClient);

        //Act
        var clientResult = await timeSeriesClient.GetStockAsync("MSFT", Interval.FiveMinute, 2);

        //Assert
        clientResult.Should().NotBeNull();
        clientResult.IsError.Should().BeFalse();
        clientResult.ErrorMessage.Should().BeNull();
        clientResult.Result.Should().BeEquivalentTo(stockDto);
    }

    [Fact]
    public async Task GetStockAsync_TooManyRequests_GetsUnsuccessfully()
    {
        //Arrange
        var logger = Substitute.For<ILogger<TimeSeriesClient>>();
        var mockHttpClient = new MockHttpMessageHandler();
        var uri = "https://api.twelvedata.com/time_series?symbol=MSFT&interval=5min&outputsize=2";

        mockHttpClient.When(uri)
            .Respond(HttpStatusCode.TooManyRequests);

        var httpClient = mockHttpClient.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://api.twelvedata.com/");
        httpClient.Timeout = TimeSpan.FromMinutes(5);

        var timeSeriesClient = new TimeSeriesClient(logger, httpClient);

        //Act
        var clientResult = await timeSeriesClient.GetStockAsync("MSFT", Interval.FiveMinute, 2);

        //Assert
        clientResult.Should().NotBeNull();
        clientResult.IsError.Should().BeTrue();
        clientResult.ErrorMessage.Should().Be(TwelveDataStatusMessages.TooManyRequestsMessage);
        clientResult.Result.Should().BeNull();
    }

    [Fact]
    public async Task GetStockAsync_Unauthorised_GetsUnsuccessfully()
    {
        //Arrange
        var logger = Substitute.For<ILogger<TimeSeriesClient>>();
        var mockHttpClient = new MockHttpMessageHandler();
        var uri = "https://api.twelvedata.com/time_series?symbol=MSFT&interval=5min&outputsize=2";

        mockHttpClient.When(uri)
            .Respond(HttpStatusCode.Unauthorized);

        var httpClient = mockHttpClient.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://api.twelvedata.com/");
        httpClient.Timeout = TimeSpan.FromMinutes(5);

        var timeSeriesClient = new TimeSeriesClient(logger, httpClient);

        //Act
        var clientResult = await timeSeriesClient.GetStockAsync("MSFT", Interval.FiveMinute, 2);

        //Assert
        clientResult.Should().NotBeNull();
        clientResult.IsError.Should().BeTrue();
        clientResult.ErrorMessage.Should().Be(TwelveDataStatusMessages.UnauthorisedMessage);
        clientResult.Result.Should().BeNull();
    }

    [Fact]
    public async Task GetStockAsync_NotFound_GetsUnsuccessfully()
    {
        //Arrange
        var logger = Substitute.For<ILogger<TimeSeriesClient>>();
        var mockHttpClient = new MockHttpMessageHandler();
        var uri = "https://api.twelvedata.com/time_series?symbol=MSFT&interval=5min&outputsize=2";

        mockHttpClient.When(uri)
            .Respond(HttpStatusCode.NotFound);

        var httpClient = mockHttpClient.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://api.twelvedata.com/");
        httpClient.Timeout = TimeSpan.FromMinutes(5);

        var timeSeriesClient = new TimeSeriesClient(logger, httpClient);

        //Act
        var clientResult = await timeSeriesClient.GetStockAsync("MSFT", Interval.FiveMinute, 2);

        //Assert
        clientResult.Should().NotBeNull();
        clientResult.IsError.Should().BeTrue();
        clientResult.ErrorMessage.Should().Be(TwelveDataStatusMessages.NotFoundMessage);
        clientResult.Result.Should().BeNull();
    }

    [Fact]
    public async Task GetStockAsync_BadRequest_GetsUnsuccessfully()
    {
        //Arrange
        var logger = Substitute.For<ILogger<TimeSeriesClient>>();
        var mockHttpClient = new MockHttpMessageHandler();
        var uri = "https://api.twelvedata.com/time_series?symbol=MSFT&interval=5min&outputsize=2";

        mockHttpClient.When(uri)
            .Respond(HttpStatusCode.BadRequest);

        var httpClient = mockHttpClient.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://api.twelvedata.com/");
        httpClient.Timeout = TimeSpan.FromMinutes(5);

        var timeSeriesClient = new TimeSeriesClient(logger, httpClient);

        //Act
        var clientResult = await timeSeriesClient.GetStockAsync("MSFT", Interval.FiveMinute, 2);

        //Assert
        clientResult.Should().NotBeNull();
        clientResult.IsError.Should().BeTrue();
        clientResult.ErrorMessage.Should().Be(TwelveDataStatusMessages.BadRequestMessage);
        clientResult.Result.Should().BeNull();
    }

    [Fact]
    public async Task GetTimeSeriesAsync_GetsSuccessfully()
    {
        //Arrange
        var logger = Substitute.For<ILogger<TimeSeriesClient>>();
        var mockHttpClient = new MockHttpMessageHandler();
        var uri = "https://api.twelvedata.com/time_series?symbol=MSFT&interval=5min&outputsize=2";

        var metaDataDto = new MetadataDto
        {
            Symbol = "MSFT",
            Type = "Common Stock",
            Currency = "USD",
            Exchange = "NASDAQ",
            ExchangeTimezone = "America/New_York",
            MicCode = "XNGS",
            Interval = "5min"
        };

        var timeseriesDto = new List<TimeSeriesDto>()
        {
            new TimeSeriesDto
            {
                Datetime = "2023-11-28 15:55:00",
                High = "382.76001",
                Low = "382.03000",
                Open = "382.03000",
                Close = "382.70001",
                Volume = "1118579"
            },
            new TimeSeriesDto
            {
                Datetime = "2023-11-28 15:50:00",
                High = "382.04999",
                Low = "381.26999",
                Open = "381.29001",
                Close = "382.03000",
                Volume = "501357"
            }
        };

        var stockDto = new StockDto
        {
            Metadata = metaDataDto,
            TimeSeries = timeseriesDto,
        };

        mockHttpClient.When(uri)
            .Respond(HttpStatusCode.OK, JsonContent.Create(stockDto,
                options: new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower }));

        var httpClient = mockHttpClient.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://api.twelvedata.com/");
        httpClient.Timeout = TimeSpan.FromMinutes(5);

        var timeSeriesClient = new TimeSeriesClient(logger, httpClient);

        //Act
        var clientResult = await timeSeriesClient.GetTimeSeriesAsync("MSFT", Interval.FiveMinute, 2);

        //Assert
        clientResult.Should().NotBeNull();
        clientResult.IsError.Should().BeFalse();
        clientResult.ErrorMessage.Should().BeNull();
        clientResult.Result.Should().BeEquivalentTo(timeseriesDto);
    }

    [Fact]
    public async Task GetTimeSeriesAsync_TooManyRequests_GetsUnsuccessfully()
    {
        //Arrange
        var logger = Substitute.For<ILogger<TimeSeriesClient>>();
        var mockHttpClient = new MockHttpMessageHandler();
        var uri = "https://api.twelvedata.com/time_series?symbol=MSFT&interval=5min&outputsize=2";

        mockHttpClient.When(uri)
            .Respond(HttpStatusCode.TooManyRequests);

        var httpClient = mockHttpClient.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://api.twelvedata.com/");
        httpClient.Timeout = TimeSpan.FromMinutes(5);

        var timeSeriesClient = new TimeSeriesClient(logger, httpClient);

        //Act
        var clientResult = await timeSeriesClient.GetTimeSeriesAsync("MSFT", Interval.FiveMinute, 2);

        //Assert
        clientResult.Should().NotBeNull();
        clientResult.IsError.Should().BeTrue();
        clientResult.ErrorMessage.Should().Be(TwelveDataStatusMessages.TooManyRequestsMessage);
        clientResult.Result.Should().BeNull();
    }

    [Fact]
    public async Task GetTimeSeriesAsync_Unauthorised_GetsUnsuccessfully()
    {
        //Arrange
        var logger = Substitute.For<ILogger<TimeSeriesClient>>();
        var mockHttpClient = new MockHttpMessageHandler();
        var uri = "https://api.twelvedata.com/time_series?symbol=MSFT&interval=5min&outputsize=2";

        mockHttpClient.When(uri)
            .Respond(HttpStatusCode.Unauthorized);

        var httpClient = mockHttpClient.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://api.twelvedata.com/");
        httpClient.Timeout = TimeSpan.FromMinutes(5);

        var timeSeriesClient = new TimeSeriesClient(logger, httpClient);

        //Act
        var clientResult = await timeSeriesClient.GetTimeSeriesAsync("MSFT", Interval.FiveMinute, 2);

        //Assert
        clientResult.Should().NotBeNull();
        clientResult.IsError.Should().BeTrue();
        clientResult.ErrorMessage.Should().Be(TwelveDataStatusMessages.UnauthorisedMessage);
        clientResult.Result.Should().BeNull();
    }

    [Fact]
    public async Task GetTimeSeriesAsync_NotFound_GetsUnsuccessfully()
    {
        //Arrange
        var logger = Substitute.For<ILogger<TimeSeriesClient>>();
        var mockHttpClient = new MockHttpMessageHandler();
        var uri = "https://api.twelvedata.com/time_series?symbol=MSFT&interval=5min&outputsize=2";

        mockHttpClient.When(uri)
            .Respond(HttpStatusCode.NotFound);

        var httpClient = mockHttpClient.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://api.twelvedata.com/");
        httpClient.Timeout = TimeSpan.FromMinutes(5);

        var timeSeriesClient = new TimeSeriesClient(logger, httpClient);

        //Act
        var clientResult = await timeSeriesClient.GetTimeSeriesAsync("MSFT", Interval.FiveMinute, 2);

        //Assert
        clientResult.Should().NotBeNull();
        clientResult.IsError.Should().BeTrue();
        clientResult.ErrorMessage.Should().Be(TwelveDataStatusMessages.NotFoundMessage);
        clientResult.Result.Should().BeNull();
    }

    [Fact]
    public async Task GetTimeSeriesAsync_BadRequest_GetsUnsuccessfully()
    {
        //Arrange
        var logger = Substitute.For<ILogger<TimeSeriesClient>>();
        var mockHttpClient = new MockHttpMessageHandler();
        var uri = "https://api.twelvedata.com/time_series?symbol=MSFT&interval=5min&outputsize=2";

        mockHttpClient.When(uri)
            .Respond(HttpStatusCode.BadRequest);

        var httpClient = mockHttpClient.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://api.twelvedata.com/");
        httpClient.Timeout = TimeSpan.FromMinutes(5);

        var timeSeriesClient = new TimeSeriesClient(logger, httpClient);

        //Act
        var clientResult = await timeSeriesClient.GetTimeSeriesAsync("MSFT", Interval.FiveMinute, 2);

        //Assert
        clientResult.Should().NotBeNull();
        clientResult.IsError.Should().BeTrue();
        clientResult.ErrorMessage.Should().Be(TwelveDataStatusMessages.BadRequestMessage);
        clientResult.Result.Should().BeNull();
    }
}
