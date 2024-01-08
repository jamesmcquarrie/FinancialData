using FinancialData.Domain.Enums;

namespace FinancialData.Worker.Application.Factories.TwelveDataEndpointFactories;

public class TwelveDataV1EndpointFactory : ITimeSeriesEndpointFactory
{
    public string Create(string symbol, Interval interval, int outputSize)
    {
        return $"time_series?symbol={symbol}&interval={interval.Name}&outputsize={Convert.ToString(outputSize)}";
    }
}
