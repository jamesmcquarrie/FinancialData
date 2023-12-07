using Microsoft.Extensions.Options;

namespace FinancialData.Worker.Options;

public class TimeSeriesClientOptionsSetup : IConfigureOptions<TimeSeriesClientOptions>
{
    private const string ConfigurationSectionName = nameof(TimeSeriesClientOptions);
    private readonly IConfiguration _configuration;

    public TimeSeriesClientOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(TimeSeriesClientOptions options)
    {
        _configuration.GetSection(ConfigurationSectionName)
            .Bind(options);
    }
}
