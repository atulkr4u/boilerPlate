using boilerPlate.Middleware;
using boilerPlate.SerilogCustomSinks;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MongoDB;
using StackExchange.Redis;
using boilerPlate.Infra.Services;
using boilerPlate.Infra.ServiceContracts;
var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ICachingService, CachingService>();
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6379"));

//Add Serilog Start

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

//Add Serilog End
// Configure Redis connection

var app = builder.Build();

////Serilog Setup Start
///
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.MongoDB("mongodb://localhost:27017/logs", collectionName: "logs", restrictedToMinimumLevel: LogEventLevel.Information)
        .WriteTo.Sink(new TelegramCustomSink("6192549985:AAElMvWhByCK0saq8rth3CJ-KEBzr9iBmsk", LogEventLevel.Error))
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

