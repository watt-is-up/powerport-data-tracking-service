using System.Text.Json.Serialization;
using MongoDB.Driver;

using DataTrackingService.Data.Mongo;
using DataTrackingService.Data.Mongo.Spreadsheets;
using DataTrackingService.Data.Mongo.Usage;

using DataTrackingService.Application.Commands;
using DataTrackingService.Application.Queries;
using DataTrackingService.Application.Services;

var logger = LoggerFactory
    .Create(builder => builder.AddConsole())
    .CreateLogger("Program");
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });

// MongoDB
builder.Services.AddSingleton<IMongoClient>(_ =>
    new MongoClient(builder.Configuration["MongoDb:ConnectionString"]));
builder.Services.AddSingleton<ITenantRegistry, TenantRegistry>();
builder.Services.AddScoped<IMongoDbContextFactory, MongoDbContextFactory>();

builder.Services.AddScoped<IMongoDbContextFactory, MongoDbContextFactory>();
builder.Services.AddScoped<MongoIndexInitializer>();

builder.Services.AddScoped<ChargingSessionRepository>();

builder.Services.AddScoped<UserMonthlyUsageRepository>();
builder.Services.AddScoped<ProviderMonthlyRevenueRepository>();
builder.Services.AddScoped<ProviderDailyPowerUsageRepository>();

builder.Services.AddScoped<UserSpreadsheetRepository>();
builder.Services.AddScoped<UserSpreadsheetRowRepository>();


// Application services
builder.Services.AddScoped<ChargingSessionReadService>();
builder.Services.AddScoped<ChargingSessionWriteService>();

builder.Services.AddScoped<UsageAggregationService>();
builder.Services.AddScoped<ProviderAggregationService>();

builder.Services.AddScoped<UserSpreadsheetReadService>();
builder.Services.AddScoped<UserSpreadsheetWriteService>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
    

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider
        .GetRequiredService<MongoIndexInitializer>();

    try
    {
        await initializer.EnsureIndexesAsync();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to initialize MongoDB indexes");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
