namespace FinancialData.Api.Application.ErrorMessages;

public static class TimeSeriesErrorMessages
{
    public const string StockNotFound = "Stock has not been found, please specify another id";
    public const string TimeSeriesNotFound = "Timeseries data for the specified stock has not been found, please specify another stock id";
    public const string MetaDataNotFound = "MetaData for the specified stock has not been found, please specify another stock id";
}
