using Microsoft.Extensions.Options;

namespace FinancialData.Worker.Options;

public class TwelveDataTokenBucketLimiterOptionsSetup : IConfigureOptions<TwelveDataTokenBucketLimiterOptions>
{
    private const string ConfigurationSectionName = nameof(TwelveDataTokenBucketLimiterOptions);
    private readonly IConfiguration _configuration;

    public TwelveDataTokenBucketLimiterOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(TwelveDataTokenBucketLimiterOptions options)
    {
        _configuration.GetSection(ConfigurationSectionName)
            .Bind(options);
    }
}
