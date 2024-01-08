using Microsoft.Extensions.Options;

namespace FinancialData.Worker.Options.Setup;

public class TimeSeriesArgumentsOptionsSetup : IConfigureOptions<TimeSeriesArgumentsOptions>
{
    private const string ConfigurationSectionName = nameof(TimeSeriesArgumentsOptions);
    private readonly IConfiguration _configuration;

    public TimeSeriesArgumentsOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(TimeSeriesArgumentsOptions options)
    {
        _configuration.GetSection(ConfigurationSectionName)
            .Bind(options);
    }
}
