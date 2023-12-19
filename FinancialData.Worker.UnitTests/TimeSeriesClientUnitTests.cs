using FinancialData.Common.Dtos;
using FinancialData.Domain.Enums;
using FinancialData.Worker.Application.Clients;
using FinancialData.Worker.Application.StatusMessages;
using FinancialData.Worker.UnitTests.Customizations;
using FinancialData.Common.Configuration;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using RichardSzalay.MockHttp;

namespace FinancialData.Worker.UnitTests;

public class TimeSeriesClientUnitTests
{
    [Fact]
    public async Task GetStockAsync_GetsSuccessfully()
    {
        //Arrange
        var mockHttpClient = new MockHttpMessageHandler();

        var fixture = new Fixture();
        var timeseriesArg = fixture.Customize(new TimeSeriesArgumentsCustomization())
            .Create<TimeSeriesArguments>();

        var uri = $"https://api.twelvedata.com/time_series?symbol={timeseriesArg.Symbol}&interval={Interval.FromName(timeseriesArg.Interval)}&outputsize={timeseriesArg.OutputSize}";

        var stockDto = new StockDto
        {
            Metadata = fixture.Create<MetadataDto>(),
            TimeSeries = fixture.CreateMany<TimeSeriesDto>()
            .ToList(),
        };

        mockHttpClient.When(uri)
            .Respond(HttpStatusCode.OK, JsonContent.Create(stockDto,
                options: new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower }));

        var httpClient = mockHttpClient.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://api.twelvedata.com/");
        httpClient.Timeout = TimeSpan.FromMinutes(5);

        var timeSeriesClient = new TimeSeriesClient(httpClient);

        //Act
        var clientResult = await timeSeriesClient.GetStockAsync(timeseriesArg.Symbol, Interval.FromName(timeseriesArg.Interval), timeseriesArg.OutputSize);

        //Assert
        clientResult.IsError.Should().BeFalse();
        clientResult.ErrorMessage.Should().BeNull();
        clientResult.Payload.Should().BeEquivalentTo(stockDto);
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

        var fixture = new Fixture();
        var timeseriesArg = fixture.Customize(new TimeSeriesArgumentsCustomization())
            .Create<TimeSeriesArguments>();

        var uri = $"https://api.twelvedata.com/time_series?symbol={timeseriesArg.Symbol}&interval={Interval.FromName(timeseriesArg.Interval)}&outputsize={timeseriesArg.OutputSize}";

        mockHttpClient.When(uri)
            .Respond(httpStatusCode);

        var httpClient = mockHttpClient.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://api.twelvedata.com/");
        httpClient.Timeout = TimeSpan.FromMinutes(5);

        var timeSeriesClient = new TimeSeriesClient(httpClient);

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

        var fixture = new Fixture();
        var timeseriesArg = fixture.Customize(new TimeSeriesArgumentsCustomization())
            .Create<TimeSeriesArguments>();

        var uri = $"https://api.twelvedata.com/time_series?symbol={timeseriesArg.Symbol}&interval={Interval.FromName(timeseriesArg.Interval)}&outputsize={timeseriesArg.OutputSize}";

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

        var fixture = new Fixture();
        var timeseriesArg = fixture.Customize(new TimeSeriesArgumentsCustomization())
            .Create<TimeSeriesArguments>();

        var uri = $"https://api.twelvedata.com/time_series?symbol={timeseriesArg.Symbol}&interval={Interval.FromName(timeseriesArg.Interval)}&outputsize={timeseriesArg.OutputSize}";

        mockHttpClient.When(uri)
            .Respond(httpStatusCode);

        var httpClient = mockHttpClient.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://api.twelvedata.com/");
        httpClient.Timeout = TimeSpan.FromMinutes(5);

        var timeSeriesClient = new TimeSeriesClient(httpClient);

        //Act
        var clientResult = await timeSeriesClient.GetTimeSeriesAsync(timeseriesArg.Symbol, Interval.FromName(timeseriesArg.Interval), timeseriesArg.OutputSize);

        //Assert
        clientResult.IsError.Should().BeTrue();
        clientResult.ErrorMessage.Should().Be(errorMessage);
        clientResult.Payload.Should().BeNull();
    }
}
