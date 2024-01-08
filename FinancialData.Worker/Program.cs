using FinancialData.Worker.Application.Repositories;
using FinancialData.Worker.Application.Services;
using FinancialData.Worker.DependencyInjection;
using FinancialData.Worker.Options.Setup;
using FinancialData.Infrastructure;
using FinancialData.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Quartz;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.ConfigureOptions<TimeSeriesArgumentsOptionsSetup>();
        services.ConfigureOptions<TwelveDataTokenBucketLimiterOptionsSetup>();
        services.ConfigureOptions<TwelveDataClientOptionsSetup>();

        services.AddTwelveDataTokenBucketRateLimiter();
        services.AddTwelveDataEndpointFactory();
        services.AddTwelveDataTimeSeriesClient();

        services.AddDbContext<FinancialDataContext>(options =>
            options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ITimeSeriesScheduledService, TimeSeriesScheduledService>();
        services.AddScoped<ITimeSeriesScheduledRepository, TimeSeriesScheduledRepository>();

        services.AddQuartz();
        services.AddTimeSeriesQuartzJobs();
    })
    .UseSerilog((hostContext, loggerConfiguration) =>
    {
        loggerConfiguration.ReadFrom.Configuration(hostContext.Configuration);
    })
    .Build();

host.Run();
