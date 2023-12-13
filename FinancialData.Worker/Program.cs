using FinancialData.Worker.Application.Repositories;
using FinancialData.Worker.Application.Services;
using FinancialData.Worker.DependencyInjection;
using FinancialData.Infrastructure;
using FinancialData.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddTwelveDataTokenBucketRateLimiter(hostContext.Configuration);
        services.AddTimeSeriesClient(hostContext.Configuration);

        services.AddDbContext<FinancialDataContext>(options =>
            options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ITimeSeriesScheduledService, TimeSeriesScheduledService>();
        services.AddScoped<ITimeSeriesScheduledRepository, TimeSeriesScheduledRepository>();

        services.AddTimeSeriesQuartzJobs(hostContext.Configuration);
    })
    .UseSerilog((hostContext, loggerConfiguration) =>
    {
        loggerConfiguration.ReadFrom.Configuration(hostContext.Configuration);
    })
    .Build();

host.Run();
