using FinancialData.Api.Application.Services;
using FinancialData.Api.Endpoints;
using FinancialData.Domain.Repositories;
using FinancialData.Infrastructure;
using FinancialData.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((hostContext, LoggerConfiguration) =>
{
    LoggerConfiguration.ReadFrom.Configuration(hostContext.Configuration);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<FinancialDataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ITimeSeriesRepository, TimeSeriesRepository>();
builder.Services.AddScoped<ITimeSeriesService, TimeSeriesService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.AddTimeSeriesEndpoints();

app.Run();