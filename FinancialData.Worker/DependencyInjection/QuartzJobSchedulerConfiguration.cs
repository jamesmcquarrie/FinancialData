using FinancialData.Worker.TimeSeriesJobs;
using FinancialData.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Quartz;

namespace FinancialData.Worker.DependencyInjection;

public static class QuartzJobSchedulerConfiguration
{
    public static IServiceCollection AddTimeSeriesQuartzJobs(this IServiceCollection services)
    {        
        services.AddOptions<QuartzOptions>()
            .Configure<IOptions<TimeSeriesArgumentsOptions>>((configurator, timeSeriesArgumentsOptions) =>
            {
                var getStockJobKey = new JobKey($"{nameof(GetStock)}-job");
                var getStockTriggerKey = new TriggerKey($"{nameof(GetStock)}-trigger");

                configurator.AddJob<GetStock>(options => options
                    .WithIdentity(getStockJobKey)
                );

                configurator.AddTrigger(options => options
                    .ForJob(getStockJobKey)
                    .WithIdentity(getStockTriggerKey)
                    .StartNow()
                );

                var getTimeseriesJobKey = new JobKey($"{nameof(GetTimeSeries)}-job");
                var getTimeseriesTriggerKey = new TriggerKey($"{nameof(GetTimeSeries)}-trigger");

                configurator.AddJob<GetTimeSeries>(options => options
                    .WithIdentity(getTimeseriesJobKey)
                );

                configurator.AddTrigger(options => options
                    .ForJob(getTimeseriesJobKey)
                    .WithIdentity(getTimeseriesTriggerKey)
                    .WithSimpleSchedule(s => s
                        .WithIntervalInMinutes(timeSeriesArgumentsOptions.Value.DelayMinutes)
                        .RepeatForever())
                    .StartAt(DateTimeOffset.Now.
                        AddMinutes(timeSeriesArgumentsOptions.Value.DelayMinutes)
                    )
                );
            });

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        return services;
    }
}
