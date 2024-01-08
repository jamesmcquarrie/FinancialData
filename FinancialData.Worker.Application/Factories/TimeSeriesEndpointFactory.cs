using FinancialData.Worker.Application.Factories.TwelveDataEndpointFactories;

namespace FinancialData.Worker.Application.Factories;

public class TimeSeriesEndpointFactory
{
    public ITimeSeriesEndpointFactory Create(string apiVersion)
    {
        switch (apiVersion)
        {
            case "V1":
                return new TwelveDataV1EndpointFactory();
            default:
                throw new NotImplementedException();
        }
    }
}
