using FinancialData.WorkerApplication.Clients;
using FinancialData.WorkerApplication.Repositories;
using FinancialData.WorkerApplication.Services;
using FinancialData.Infrastructure;
using FinancialData.Infrastructure.Repositories;
using FinancialData.Worker.Options;
using FinancialData.Worker.TimeSeriesJobs;
using System.Text.Json;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Polly;
using Quartz;
using System.Threading.RateLimiting;
using System.Net.Http.Headers;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddLogging();

        services.AddHttpClient<ITimeSeriesClient, TimeSeriesClient>(client =>
        {
            var options = hostContext.Configuration.GetSection(nameof(RapidApiOptions))
                .Get<RapidApiOptions>();

            client.BaseAddress = new Uri(options.BaseUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("apikey", options.Key);
        })
        .ConfigurePrimaryHttpMessageHandler(() =>
        {
            return new SocketsHttpHandler()
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(2)
            };
        })
        .SetHandlerLifetime(Timeout.InfiniteTimeSpan)
        .AddResilienceHandler("Rate-Limiter", builder =>
            builder.AddRateLimiter(new TokenBucketRateLimiter(
                new TokenBucketRateLimiterOptions
                {
                    AutoReplenishment = true,
                    QueueLimit = 8,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    ReplenishmentPeriod = TimeSpan.FromSeconds(60),
                    TokenLimit = 8,
                    TokensPerPeriod = 8
                })
            )
        );

        services.AddDbContext<FinancialDataContext>(options =>
            options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection")));
        
        services.AddScoped<ITimeSeriesScheduledService, TimeSeriesScheduledService>();
        services.AddScoped<ITimeSeriesScheduledRepository, TimeSeriesScheduledRepository>();

        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();

            var symbols = hostContext.Configuration.GetSection("Symbols")
                .Get<string[]>();
            var intervalOutputSizesOptions = hostContext.Configuration.GetSection("IntervalOutputSizeOptions")
                .Get<IntervalOutputSizeOptions[]>();

            var serializedSymbols = JsonSerializer.Serialize<string[]>(symbols);

            foreach (var intervalOutputSize in intervalOutputSizesOptions)
            {
                var jobKeyOnce = new JobKey($"{intervalOutputSize.Interval}-once-job");
                var triggerKeyOnce = new TriggerKey($"{intervalOutputSize.Interval}-once-trigger");

                q.AddJob<GetStock>(options => options
                    .WithIdentity(jobKeyOnce)
                    .UsingJobData("symbols", serializedSymbols)
                    .UsingJobData("interval", intervalOutputSize.Interval)
                    .UsingJobData("outputSize", intervalOutputSize.OutputSize)
                );

                q.AddTrigger(options => options
                    .ForJob(jobKeyOnce)
                    .WithIdentity(triggerKeyOnce)
                    .StartNow()
                );

                var jobKeyRecurring = new JobKey($"{intervalOutputSize.Interval}-recurring-job");
                var triggerKeyRecurring = new TriggerKey($"{intervalOutputSize.Interval}-recurring-trigger");

                q.AddJob<GetTimeSeries>(options => options
                    .WithIdentity(jobKeyRecurring)
                    .UsingJobData("symbols", serializedSymbols)
                    .UsingJobData("interval", intervalOutputSize.Interval)
                    .UsingJobData("outputSize", intervalOutputSize.OutputSize)
                );

                q.AddTrigger(options => options
                    .ForJob(jobKeyRecurring)
                    .WithIdentity(triggerKeyRecurring)
                    .WithSimpleSchedule(s => s
                        .WithIntervalInMinutes(intervalOutputSize.OutputSize)
                        .RepeatForever())
                    .StartAt(DateTimeOffset.Now.
                        AddMinutes(intervalOutputSize.OutputSize)
                    )
                );
            }
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    })
    .Build();

host.Run();
