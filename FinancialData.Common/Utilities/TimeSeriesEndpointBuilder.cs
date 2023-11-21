using FinancialData.Domain.Enums;

namespace FinancialData.Common.Utilities;

public static class TimeSeriesEndpointBuilder
{
    public static string BuildTimeSeriesEndpoint(string symbol, Interval interval, int outputSize)
    {
        return $"time_series?symbol={symbol}&interval={interval.Name}&outputsize={Convert.ToString(outputSize)}";
    }
}
