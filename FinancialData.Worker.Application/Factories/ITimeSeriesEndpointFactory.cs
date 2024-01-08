using FinancialData.Domain.Enums;

namespace FinancialData.Worker.Application.Factories;

public interface ITimeSeriesEndpointFactory
{
    string Create(string symbol, Interval interval, int outputSize);
}
