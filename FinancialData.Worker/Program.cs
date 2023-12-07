using FinancialData.Worker.Application.Clients;
using FinancialData.Worker.Application.Handlers;
using FinancialData.Worker.Application.Repositories;
using FinancialData.Worker.Application.Services;
using FinancialData.Infrastructure;
using FinancialData.Infrastructure.Repositories;
using FinancialData.Worker.Options;
using FinancialData.Worker.TimeSeriesJobs;
using System.Net.Http.Headers;
using System.Threading.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Polly;
using Quartz;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddLogging();

        var httpClientBuilder = services.AddHttpClient<ITimeSeriesClient, TimeSeriesClient>(client =>
        {
            var timeSeriesClientOptions = hostContext.Configuration
                .GetRequiredSection(nameof(TimeSeriesClientOptions))
                .Get<TimeSeriesClientOptions>();

            client.BaseAddress = new Uri(timeSeriesClientOptions.BaseUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("apikey", hostContext.Configuration
                .GetRequiredSection("ApiKey")
                .Get<string>());
            client.Timeout = TimeSpan.FromMinutes(timeSeriesClientOptions.TimeoutMinutes);
        })
        .ConfigurePrimaryHttpMessageHandler(() =>
        {
            return new SocketsHttpHandler()
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(2)
            };
        })
        .SetHandlerLifetime(Timeout.InfiniteTimeSpan);
        
        if (hostContext.HostingEnvironment.IsDevelopment())
        {
            services.AddTransient<DebugRateLimiterHandler>();
            httpClientBuilder.AddHttpMessageHandler<DebugRateLimiterHandler>();
        }

        var applicationTokenBucketLimiterOptions = hostContext.Configuration
            .GetRequiredSection(nameof(ApplicationTokenBucketLimiterOptions))
            .Get<ApplicationTokenBucketLimiterOptions>();

        var tokenBucket = new TokenBucketRateLimiter(
            new TokenBucketRateLimiterOptions
            {
                TokenLimit = applicationTokenBucketLimiterOptions.TokenLimit,
                TokensPerPeriod = applicationTokenBucketLimiterOptions.TokensPerPeriod,
                ReplenishmentPeriod = TimeSpan
                    .FromMinutes(applicationTokenBucketLimiterOptions.ReplenishmentPeriodMinutes),
                QueueLimit = applicationTokenBucketLimiterOptions.QueueLimit,
                QueueProcessingOrder = Enum
                    .Parse<QueueProcessingOrder>(applicationTokenBucketLimiterOptions.QueueProcessingOrder),
                AutoReplenishment = applicationTokenBucketLimiterOptions.AutoReplenishment,
            });

        services.AddSingleton<RateLimiter>(tokenBucket);

        httpClientBuilder.AddResilienceHandler("Rate-Limiter", builder =>
            builder.AddRateLimiter(tokenBucket));

        services.AddDbContext<FinancialDataContext>(options =>
            options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ITimeSeriesScheduledService, TimeSeriesScheduledService>();
        services.AddScoped<ITimeSeriesScheduledRepository, TimeSeriesScheduledRepository>();

        services.AddQuartz(q =>
        {
            var timeseriesOptionstring = File.ReadAllText("timeseriesOptions.json");
            var delay = hostContext.Configuration
                .GetRequiredSection("DelayMinutes")
                .Get<int>();

            var jobKeyOnce = new JobKey("once-job");
            var triggerKeyOnce = new TriggerKey("once-trigger");

            q.AddJob<GetStock>(options => options
                .WithIdentity(jobKeyOnce)
                .UsingJobData("timeseriesOptions", timeseriesOptionstring)
            );

            q.AddTrigger(options => options
                .ForJob(jobKeyOnce)
                .WithIdentity(triggerKeyOnce)
                .StartNow()
            );

            var jobKeyRecurring = new JobKey("recurring-job");
            var triggerKeyRecurring = new TriggerKey("recurring-trigger");

            q.AddJob<GetTimeSeries>(options => options
                .WithIdentity(jobKeyRecurring)
                .UsingJobData("timeseriesOptions", timeseriesOptionstring)
            );

            q.AddTrigger(options => options
                .ForJob(jobKeyRecurring)
                .WithIdentity(triggerKeyRecurring)
                .WithSimpleSchedule(s => s
                    .WithIntervalInMinutes(5)
                    .RepeatForever())
                .StartAt(DateTimeOffset.Now.
                    AddMinutes(1)
                )
            );
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
    })
    .Build();

host.Run();
