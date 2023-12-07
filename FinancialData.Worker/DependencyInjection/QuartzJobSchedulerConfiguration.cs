using FinancialData.Worker.TimeSeriesJobs;
using Quartz;

namespace FinancialData.Worker.DependencyInjection;

public static class QuartzJobSchedulerConfiguration
{
    public static IServiceCollection AddTimeSeriesQuartzJobs(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddQuartz(configurator =>
        {
            var path = Path.Combine(configuration["ArgumentsPath"]!, "timeseriesArguments.json");
            var timeseriesArgumentsString = File.ReadAllText(path);
            var delay = configuration
                .GetRequiredSection("DelayMinutes")
                .Get<int>();

            var getStockJobKey = new JobKey($"{nameof(GetStock)}-job");
            var getStockTriggerKey = new TriggerKey($"{nameof(GetStock)}-trigger");

            configurator.AddJob<GetStock>(options => options
                .WithIdentity(getStockJobKey)
                .UsingJobData("timeseriesArguments", timeseriesArgumentsString)
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
                .UsingJobData("timeseriesArguments", timeseriesArgumentsString)
            );

            configurator.AddTrigger(options => options
                .ForJob(getTimeseriesJobKey)
                .WithIdentity(getTimeseriesTriggerKey)
                .WithSimpleSchedule(s => s
                    .WithIntervalInMinutes(delay)
                    .RepeatForever())
                .StartAt(DateTimeOffset.Now.
                    AddMinutes(delay)
                )
            );
        });

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        return services;
    }
}
