using FinancialData.Worker.Application.Repositories;
using FinancialData.Worker.Application.Services;
using FinancialData.Infrastructure;
using FinancialData.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Quartz;
using FinancialData.Worker.DependencyInjection;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddLogging();

        services.AddTwelveDataTokenBucketRateLimiter();
        services.AddTimeSeriesClient(hostContext.Configuration);

        services.AddDbContext<FinancialDataContext>(options =>
            options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ITimeSeriesScheduledService, TimeSeriesScheduledService>();
        services.AddScoped<ITimeSeriesScheduledRepository, TimeSeriesScheduledRepository>();

        services.AddTimeSeriesQuartzJobs(hostContext.Configuration);
    })
    .Build();

host.Run();
