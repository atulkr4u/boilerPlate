using boilerPlate.Middleware;
using boilerPlate.SerilogCustomSinks;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MongoDB;
using StackExchange.Redis;
using boilerPlate.Infra.Services;
using boilerPlate.Infra.ServiceContracts;
using boilerPlate.DataService.Contracts;
using boilerPlate.DataService.Services;
using boilerPlate.InfraServices;
using boilerPlate.BGServices;
using Microsoft.AspNetCore.Hosting;
using boilerPlate.Controllers;
using MediatR;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddHostedService<DefaultBGService>();

builder.Services.AddMediatR(typeof(NewsItem).Assembly);



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ICachingService, CachingService>();
builder.Services.AddSingleton<IWeatherService, WeatherService>();
builder.Services.AddSingleton<IConfigService, ConfigService>();
//Steps to Start Redis:
//1.Run Command "redis-server"
//2.Check Redis: "redis-cli" then "lpush demos redis-macOS-demo" then "rpop demos"

//Add Serilog Start

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

//Add Serilog End
// Configure Redis connection

var app = builder.Build();
var configService = app.Services.GetService<IConfigService>();

////Serilog Setup Start
///
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.MongoDB("mongodb://localhost:27017/logs", collectionName: "logs", restrictedToMinimumLevel: LogEventLevel.Information)
        .WriteTo.Sink(new TelegramCustomSink(LogEventLevel.Error, configService))
      .CreateLogger();

////Serilog Setup End
///

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ExceptionMiddleware>();


app.Run();

