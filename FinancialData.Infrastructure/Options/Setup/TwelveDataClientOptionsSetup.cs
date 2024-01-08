using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace FinancialData.Infrastructure.Options.Setup;

public class TwelveDataClientOptionsSetup : IConfigureOptions<TwelveDataClientOptions>
{
    private const string ConfigurationSectionName = nameof(TwelveDataClientOptions);
    private readonly IConfiguration _configuration;

    public TwelveDataClientOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(TwelveDataClientOptions options)
    {
        _configuration.GetSection(ConfigurationSectionName)
            .Bind(options);
    }
}
