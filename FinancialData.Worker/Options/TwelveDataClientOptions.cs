namespace FinancialData.Worker.Options;

public class TwelveDataClientOptions
{
    public required string BaseUrl { get; set; }
    public required string ApiKey { get; set; }
    public required string ApiVersion { get; set; }
    public int TimeoutMinutes { get; set; }
}
