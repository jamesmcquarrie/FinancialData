using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FinancialData.Common.Dtos;
using FinancialData.Domain.Enums;
using FinancialData.WorkerApplication.Clients;
using FinancialData.WorkerApplication.StatusMessages;
using RichardSzalay.MockHttp;

namespace FinancialData.Worker.UnitTests;

public class TimeSeriesClientUnitTests
{
    [Fact]
    public async Task GetStockAsync_GetsSuccessfully()
    {
        //Arrange
        var mockHttpClient = new MockHttpMessageHandler();
        var uri = "https://api.twelvedata.com/time_series?symbol=MSFT&interval=5min&outputsize=2";

        var fixture = new Fixture();
        var metadataDto = fixture.Create<MetadataDto>();
        var timeseriesDtos = fixture.CreateMany<TimeSeriesDto>()
            .ToList();
        var stockDto = new StockDto
        {
            Metadata = metadataDto,
            TimeSeries = timeseriesDtos,
        };

        mockHttpClient.When(uri)
            .Respond(HttpStatusCode.OK, JsonContent.Create(stockDto,
                options: new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower }));

        var httpClient = mockHttpClient.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://api.twelvedata.com/");
        httpClient.Timeout = TimeSpan.FromMinutes(5);

        var timeSeriesClient = new TimeSeriesClient(httpClient);

        //Act
        var clientResult = await timeSeriesClient.GetStockAsync("MSFT", Interval.FiveMinute, 2);

        //Assert
        clientResult.Should().NotBeNull();
        clientResult.IsError.Should().BeFalse();
        clientResult.ErrorMessage.Should().BeNull();
        clientResult.Payload.Should().BeEquivalentTo(stockDto);
    }

    [Fact]
    public async Task GetStockAsync_TooManyRequests_GetsUnsuccessfully()
    {
        //Arrange
        var mockHttpClient = new MockHttpMessageHandler();
        var uri = "https://api.twelvedata.com/time_series?symbol=MSFT&interval=5min&outputsize=2";

        mockHttpClient.When(uri)
            .Respond(HttpStatusCode.TooManyRequests);

        var httpClient = mockHttpClient.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://api.twelvedata.com/");
        httpClient.Timeout = TimeSpan.FromMinutes(5);

        var timeSeriesClient = new TimeSeriesClient(httpClient);

        //Act
        var clientResult = await timeSeriesClient.GetStockAsync("MSFT", Interval.FiveMinute, 2);

        //Assert
        clientResult.Should().NotBeNull();
        clientResult.IsError.Should().BeTrue();
        clientResult.ErrorMessage.Should().Be(TwelveDataStatusMessages.TooManyRequestsMessage);
        clientResult.Payload.Should().BeNull();
    }

    [Fact]
    public async Task GetStockAsync_Unauthorised_GetsUnsuccessfully()
    {
        //Arrange

        var mockHttpClient = new MockHttpMessageHandler();
        var uri = "https://api.twelvedata.com/time_series?symbol=MSFT&interval=5min&outputsize=2";

        mockHttpClient.When(uri)
            .Respond(HttpStatusCode.Unauthorized);

        var httpClient = mockHttpClient.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://api.twelvedata.com/");
        httpClient.Timeout = TimeSpan.FromMinutes(5);

        var timeSeriesClient = new TimeSeriesClient(httpClient);

        //Act
        var clientResult = await timeSeriesClient.GetStockAsync("MSFT", Interval.FiveMinute, 2);

        //Assert
        clientResult.Should().NotBeNull();
        clientResult.IsError.Should().BeTrue();
        clientResult.ErrorMessage.Should().Be(TwelveDataStatusMessages.UnauthorisedMessage);
        clientResult.Payload.Should().BeNull();
    }

    [Fact]
    public async Task GetStockAsync_NotFound_GetsUnsuccessfully()
    {
        //Arrange

        var mockHttpClient = new MockHttpMessageHandler();
        var uri = "https://api.twelvedata.com/time_series?symbol=MSFT&interval=5min&outputsize=2";

        mockHttpClient.When(uri)
            .Respond(HttpStatusCode.NotFound);

        var httpClient = mockHttpClient.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://api.twelvedata.com/");
        httpClient.Timeout = TimeSpan.FromMinutes(5);

        var timeSeriesClient = new TimeSeriesClient(httpClient);

        //Act
        var clientResult = await timeSeriesClient.GetStockAsync("MSFT", Interval.FiveMinute, 2);

        //Assert
        clientResult.Should().NotBeNull();
        clientResult.IsError.Should().BeTrue();
        clientResult.ErrorMessage.Should().Be(TwelveDataStatusMessages.NotFoundMessage);
        clientResult.Payload.Should().BeNull();
    }

    [Fact]
    public async Task GetStockAsync_BadRequest_GetsUnsuccessfully()
    {
        //Arrange

        var mockHttpClient = new MockHttpMessageHandler();
        var uri = "https://api.twelvedata.com/time_series?symbol=MSFT&interval=5min&outputsize=2";

        mockHttpClient.When(uri)
            .Respond(HttpStatusCode.BadRequest);

        var httpClient = mockHttpClient.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://api.twelvedata.com/");
        httpClient.Timeout = TimeSpan.FromMinutes(5);

        var timeSeriesClient = new TimeSeriesClient(httpClient);

        //Act
        var clientResult = await timeSeriesClient.GetStockAsync("MSFT", Interval.FiveMinute, 2);

        //Assert
        clientResult.Should().NotBeNull();
        clientResult.IsError.Should().BeTrue();
        clientResult.ErrorMessage.Should().Be(TwelveDataStatusMessages.BadRequestMessage);
        clientResult.Payload.Should().BeNull();
    }

    [Fact]
    public async Task GetTimeSeriesAsync_GetsSuccessfully()
    {
        //Arrange

        var mockHttpClient = new MockHttpMessageHandler();
        var uri = "https://api.twelvedata.com/time_series?symbol=MSFT&interval=5min&outputsize=2";

        var fixture = new Fixture();
        var metadataDto = fixture.Create<MetadataDto>();
        var timeseriesDtos = fixture.CreateMany<TimeSeriesDto>()
            .ToList();
        var stockDto = new StockDto
        {
            Metadata = metadataDto,
            TimeSeries = timeseriesDtos,
        };

        mockHttpClient.When(uri)
            .Respond(HttpStatusCode.OK, JsonContent.Create(stockDto,
                options: new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower }));

        var httpClient = mockHttpClient.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://api.twelvedata.com/");
        httpClient.Timeout = TimeSpan.FromMinutes(5);

        var timeSeriesClient = new TimeSeriesClient(httpClient);

        //Act
        var clientResult = await timeSeriesClient.GetTimeSeriesAsync("MSFT", Interval.FiveMinute, 2);

        //Assert
        clientResult.Should().NotBeNull();
        clientResult.IsError.Should().BeFalse();
        clientResult.ErrorMessage.Should().BeNull();
        clientResult.Payload.Should().BeEquivalentTo(timeseriesDtos);
    }

    [Fact]
    public async Task GetTimeSeriesAsync_TooManyRequests_GetsUnsuccessfully()
    {
        //Arrange

        var mockHttpClient = new MockHttpMessageHandler();
        var uri = "https://api.twelvedata.com/time_series?symbol=MSFT&interval=5min&outputsize=2";

        mockHttpClient.When(uri)
            .Respond(HttpStatusCode.TooManyRequests);

        var httpClient = mockHttpClient.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://api.twelvedata.com/");
        httpClient.Timeout = TimeSpan.FromMinutes(5);

        var timeSeriesClient = new TimeSeriesClient(httpClient);

        //Act
        var clientResult = await timeSeriesClient.GetTimeSeriesAsync("MSFT", Interval.FiveMinute, 2);

        //Assert
        clientResult.Should().NotBeNull();
        clientResult.IsError.Should().BeTrue();
        clientResult.ErrorMessage.Should().Be(TwelveDataStatusMessages.TooManyRequestsMessage);
        clientResult.Payload.Should().BeNull();
    }

    [Fact]
    public async Task GetTimeSeriesAsync_Unauthorised_GetsUnsuccessfully()
    {
        //Arrange

        var mockHttpClient = new MockHttpMessageHandler();
        var uri = "https://api.twelvedata.com/time_series?symbol=MSFT&interval=5min&outputsize=2";

        mockHttpClient.When(uri)
            .Respond(HttpStatusCode.Unauthorized);

        var httpClient = mockHttpClient.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://api.twelvedata.com/");
        httpClient.Timeout = TimeSpan.FromMinutes(5);

        var timeSeriesClient = new TimeSeriesClient(httpClient);

        //Act
        var clientResult = await timeSeriesClient.GetTimeSeriesAsync("MSFT", Interval.FiveMinute, 2);

        //Assert
        clientResult.Should().NotBeNull();
        clientResult.IsError.Should().BeTrue();
        clientResult.ErrorMessage.Should().Be(TwelveDataStatusMessages.UnauthorisedMessage);
        clientResult.Payload.Should().BeNull();
    }

    [Fact]
    public async Task GetTimeSeriesAsync_NotFound_GetsUnsuccessfully()
    {
        //Arrange

        var mockHttpClient = new MockHttpMessageHandler();
        var uri = "https://api.twelvedata.com/time_series?symbol=MSFT&interval=5min&outputsize=2";

        mockHttpClient.When(uri)
            .Respond(HttpStatusCode.NotFound);

        var httpClient = mockHttpClient.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://api.twelvedata.com/");
        httpClient.Timeout = TimeSpan.FromMinutes(5);

        var timeSeriesClient = new TimeSeriesClient(httpClient);

        //Act
        var clientResult = await timeSeriesClient.GetTimeSeriesAsync("MSFT", Interval.FiveMinute, 2);

        //Assert
        clientResult.Should().NotBeNull();
        clientResult.IsError.Should().BeTrue();
        clientResult.ErrorMessage.Should().Be(TwelveDataStatusMessages.NotFoundMessage);
        clientResult.Payload.Should().BeNull();
    }

    [Fact]
    public async Task GetTimeSeriesAsync_BadRequest_GetsUnsuccessfully()
    {
        //Arrange

        var mockHttpClient = new MockHttpMessageHandler();
        var uri = "https://api.twelvedata.com/time_series?symbol=MSFT&interval=5min&outputsize=2";

        mockHttpClient.When(uri)
            .Respond(HttpStatusCode.BadRequest);

        var httpClient = mockHttpClient.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://api.twelvedata.com/");
        httpClient.Timeout = TimeSpan.FromMinutes(5);

        var timeSeriesClient = new TimeSeriesClient(httpClient);

        //Act
        var clientResult = await timeSeriesClient.GetTimeSeriesAsync("MSFT", Interval.FiveMinute, 2);

        //Assert
        clientResult.Should().NotBeNull();
        clientResult.IsError.Should().BeTrue();
        clientResult.ErrorMessage.Should().Be(TwelveDataStatusMessages.BadRequestMessage);
        clientResult.Payload.Should().BeNull();
    }
}
