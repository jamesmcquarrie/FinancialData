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
using FinancialData.Worker;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddLogging();
        services.AddTransient<RateLimiterHandler>();

        var tokenBucket = new TokenBucketRateLimiter(
                new TokenBucketRateLimiterOptions
                {
                    AutoReplenishment = true,
                    QueueLimit = 40,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    ReplenishmentPeriod = TimeSpan.FromSeconds(60),
                    TokenLimit = 8,
                    TokensPerPeriod = 8
                });

        services.AddSingleton<RateLimiter>(tokenBucket);

        services.AddHttpClient<ITimeSeriesClient, TimeSeriesClient>(client =>
        {
            var options = hostContext.Configuration.GetSection(nameof(TimeSeriesClientOptions))
                .Get<TimeSeriesClientOptions>();

            client.BaseAddress = new Uri(options.BaseUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("apikey", options.Key);
            client.Timeout = TimeSpan.FromMinutes(5);
        })
        .ConfigurePrimaryHttpMessageHandler(() =>
        {
            return new SocketsHttpHandler()
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(2)
            };
        })
        .SetHandlerLifetime(Timeout.InfiniteTimeSpan)
        .AddHttpMessageHandler<RateLimiterHandler>()
        .AddResilienceHandler("Rate-Limiter", builder =>
            builder.AddRateLimiter(tokenBucket
            )
        );


        services.AddDbContext<FinancialDataContext>(options =>
            options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection")));
        
        services.AddScoped<ITimeSeriesScheduledService, TimeSeriesScheduledService>();
        services.AddScoped<ITimeSeriesScheduledRepository, TimeSeriesScheduledRepository>();

        services.AddQuartz(q =>
        {
            var symbols = hostContext.Configuration.GetSection("Symbols")
                .Get<string[]>();
            var timeseriesOptions = hostContext.Configuration.GetSection("TimeSeriesOptions")
                .Get<TimeSeriesJobOptions[]>();

            var serializedSymbols = JsonSerializer.Serialize<string[]>(symbols);

            foreach (var timeseriesOption in timeseriesOptions)
            {
                var jobKeyOnce = new JobKey($"{timeseriesOption.Interval}-once-job");
                var triggerKeyOnce = new TriggerKey($"{timeseriesOption.Interval}-once-trigger");

                q.AddJob<GetStock>(options => options
                    .WithIdentity(jobKeyOnce)
                    .UsingJobData("symbols", serializedSymbols)
                    .UsingJobData("interval", timeseriesOption.Interval)
                    .UsingJobData("outputSize", timeseriesOption.OutputSize)
                );

                q.AddTrigger(options => options
                    .ForJob(jobKeyOnce)
                    .WithIdentity(triggerKeyOnce)
                    .StartNow()
                );

                var jobKeyRecurring = new JobKey($"{timeseriesOption.Interval}-recurring-job");
                var triggerKeyRecurring = new TriggerKey($"{timeseriesOption.Interval}-recurring-trigger");

                q.AddJob<GetTimeSeries>(options => options
                    .WithIdentity(jobKeyRecurring)
                    .UsingJobData("symbols", serializedSymbols)
                    .UsingJobData("interval", timeseriesOption.Interval)
                    .UsingJobData("outputSize", timeseriesOption.OutputSize)
                );

                q.AddTrigger(options => options
                    .ForJob(jobKeyRecurring)
                    .WithIdentity(triggerKeyRecurring)
                    .WithSimpleSchedule(s => s
                        .WithIntervalInMinutes(8)
                        .RepeatForever())
                    .StartAt(DateTimeOffset.Now.
                        AddMinutes(7)
                    )
                );
            }
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    })
    .Build();

host.Run();
