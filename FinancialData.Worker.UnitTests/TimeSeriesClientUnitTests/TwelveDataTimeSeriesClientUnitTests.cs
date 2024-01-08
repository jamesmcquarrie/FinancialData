using FinancialData.Common.Dtos;
using FinancialData.Domain.Enums;
using FinancialData.Worker.Application.Clients;
using FinancialData.Worker.Application.StatusMessages;
using FinancialData.Common.Configuration;
using FinancialData.Common.Customizations;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using RichardSzalay.MockHttp;
using FinancialData.Worker.Application.Factories;

namespace FinancialData.Worker.UnitTests.TimeSeriesClientUnitTests;

public class TwelveDataTimeSeriesClientUnitTests
{
    [Fact]
    public async Task GetStockAsync_GetsSuccessfully()
    {
        //Arrange
        var mockHttpClient = new MockHttpMessageHandler();
        var factory = Substitute.For<ITimeSeriesEndpointFactory>();

        var fixture = new Fixture();
        var timeseriesArg = fixture.Customize(new TimeSeriesArgumentsCustomization())
            .Create<TimeSeriesArguments>();

        var endpoint = $"time_series?symbol={timeseriesArg.Symbol}&interval={Interval.FromName(timeseriesArg.Interval)}&outputsize={timeseriesArg.OutputSize}";
        var uri = $"https://api.twelvedata.com/{endpoint}";

        var twelveDataStockDto = new TwelveDataStockDto
        {
            MetaData = fixture.Create<MetaDataDto>(),
            TimeSeries = fixture.CreateMany<TimeSeriesDto>()
            .ToList(),
        };

        mockHttpClient.When(uri)
            .Respond(HttpStatusCode.OK, JsonContent.Create(twelveDataStockDto,
                options: new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower }));

        var httpClient = mockHttpClient.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://api.twelvedata.com/");
        httpClient.Timeout = TimeSpan.FromMinutes(5);

        factory.Create(timeseriesArg.Symbol, Interval.FromName(timeseriesArg.Interval), timeseriesArg.OutputSize)
            .Returns(endpoint);

        var timeSeriesClient = new TwelveDataTimeSeriesClient(httpClient, factory);

        //Act
        var clientResult = await timeSeriesClient.GetStockAsync(timeseriesArg.Symbol, Interval.FromName(timeseriesArg.Interval), timeseriesArg.OutputSize);

        //Assert
        clientResult.IsError.Should().BeFalse();
        clientResult.ErrorMessage.Should().BeNull();
        clientResult.Payload.Should().BeEquivalentTo(twelveDataStockDto);
    }

    [Theory]
    [InlineData(HttpStatusCode.TooManyRequests, TwelveDataStatusMessages.TooManyRequestsMessage)]
    [InlineData(HttpStatusCode.Unauthorized, TwelveDataStatusMessages.UnauthorisedMessage)]
    [InlineData(HttpStatusCode.NotFound, TwelveDataStatusMessages.NotFoundMessage)]
    [InlineData(HttpStatusCode.BadRequest, TwelveDataStatusMessages.BadRequestMessage)]
    public async Task GetStockAsync_GetsUnsuccessfully(HttpStatusCode httpStatusCode, string errorMessage)
    {
        //Arrange
        var mockHttpClient = new MockHttpMessageHandler();
        var factory = Substitute.For<ITimeSeriesEndpointFactory>();

        var fixture = new Fixture();
        var timeseriesArg = fixture.Customize(new TimeSeriesArgumentsCustomization())
            .Create<TimeSeriesArguments>();

        var endpoint = $"time_series?symbol={timeseriesArg.Symbol}&interval={Interval.FromName(timeseriesArg.Interval)}&outputsize={timeseriesArg.OutputSize}";
        var uri = $"https://api.twelvedata.com/{endpoint}";

        mockHttpClient.When(uri)
            .Respond(httpStatusCode);

        var httpClient = mockHttpClient.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://api.twelvedata.com/");
        httpClient.Timeout = TimeSpan.FromMinutes(5);

        factory.Create(timeseriesArg.Symbol, Interval.FromName(timeseriesArg.Interval), timeseriesArg.OutputSize)
           .Returns(endpoint);

        var timeSeriesClient = new TwelveDataTimeSeriesClient(httpClient, factory);

        //Act
        var clientResult = await timeSeriesClient.GetStockAsync(timeseriesArg.Symbol, Interval.FromName(timeseriesArg.Interval), timeseriesArg.OutputSize);

        //Assert
        clientResult.IsError.Should().BeTrue();
        clientResult.ErrorMessage.Should().Be(errorMessage);
        clientResult.Payload.Should().BeNull();
    }

    [Fact]
    public async Task GetTimeSeriesAsync_GetsSuccessfully()
    {
        //Arrange
        var mockHttpClient = new MockHttpMessageHandler();
        var factory = Substitute.For<ITimeSeriesEndpointFactory>();

        var fixture = new Fixture();
        var timeseriesArg = fixture.Customize(new TimeSeriesArgumentsCustomization())
            .Create<TimeSeriesArguments>();

        var endpoint = $"time_series?symbol={timeseriesArg.Symbol}&interval={Interval.FromName(timeseriesArg.Interval)}&outputsize={timeseriesArg.OutputSize}";
        var uri = $"https://api.twelvedata.com/{endpoint}";

        var metadataDto = fixture.Create<MetaDataDto>();
        var timeseriesDtos = fixture.CreateMany<TimeSeriesDto>()
            .ToList();

        var twelveDataStockDto = new TwelveDataStockDto
        {
            MetaData = metadataDto,
            TimeSeries = timeseriesDtos
        };

        mockHttpClient.When(uri)
            .Respond(HttpStatusCode.OK, JsonContent.Create(twelveDataStockDto,
                options: new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower }));

        var httpClient = mockHttpClient.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://api.twelvedata.com/");
        httpClient.Timeout = TimeSpan.FromMinutes(5);

        factory.Create(timeseriesArg.Symbol, Interval.FromName(timeseriesArg.Interval), timeseriesArg.OutputSize)
           .Returns(endpoint);

        var timeSeriesClient = new TwelveDataTimeSeriesClient(httpClient, factory);

        //Act
        var clientResult = await timeSeriesClient.GetTimeSeriesAsync(timeseriesArg.Symbol, Interval.FromName(timeseriesArg.Interval), timeseriesArg.OutputSize);

        //Assert
        clientResult.IsError.Should().BeFalse();
        clientResult.ErrorMessage.Should().BeNull();
        clientResult.Payload.Should().BeEquivalentTo(timeseriesDtos);
    }

    [Theory]
    [InlineData(HttpStatusCode.TooManyRequests, TwelveDataStatusMessages.TooManyRequestsMessage)]
    [InlineData(HttpStatusCode.Unauthorized, TwelveDataStatusMessages.UnauthorisedMessage)]
    [InlineData(HttpStatusCode.NotFound, TwelveDataStatusMessages.NotFoundMessage)]
    [InlineData(HttpStatusCode.BadRequest, TwelveDataStatusMessages.BadRequestMessage)]
    public async Task GetTimeSeriesAsync_GetsUnsuccessfully(HttpStatusCode httpStatusCode, string errorMessage)
    {
        //Arrange
        var mockHttpClient = new MockHttpMessageHandler();
        var factory = Substitute.For<ITimeSeriesEndpointFactory>();

        var fixture = new Fixture();
        var timeseriesArg = fixture.Customize(new TimeSeriesArgumentsCustomization())
            .Create<TimeSeriesArguments>();

        var endpoint = $"time_series?symbol={timeseriesArg.Symbol}&interval={Interval.FromName(timeseriesArg.Interval)}&outputsize={timeseriesArg.OutputSize}";
        var uri = $"https://api.twelvedata.com/{endpoint}";

        mockHttpClient.When(uri)
            .Respond(httpStatusCode);

        var httpClient = mockHttpClient.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://api.twelvedata.com/");
        httpClient.Timeout = TimeSpan.FromMinutes(5);

        factory.Create(timeseriesArg.Symbol, Interval.FromName(timeseriesArg.Interval), timeseriesArg.OutputSize)
           .Returns(endpoint);

        var timeSeriesClient = new TwelveDataTimeSeriesClient(httpClient, factory);

        //Act
        var clientResult = await timeSeriesClient.GetTimeSeriesAsync(timeseriesArg.Symbol, Interval.FromName(timeseriesArg.Interval), timeseriesArg.OutputSize);

        //Assert
        clientResult.IsError.Should().BeTrue();
        clientResult.ErrorMessage.Should().Be(errorMessage);
        clientResult.Payload.Should().BeNull();
    }
}
