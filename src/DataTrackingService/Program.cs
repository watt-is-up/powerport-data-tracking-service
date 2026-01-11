using System.Text.Json.Serialization;

using DataTrackingService.Infrastructure.Multitenancy;

using DataTrackingService.Data.Mongo;
using DataTrackingService.Data.Mongo.Migrations;
using DataTrackingService.Data.Mongo.Multitenancy;
using DataTrackingService.Data.Mongo.Spreadsheets;
using DataTrackingService.Data.Mongo.Usage;

using DataTrackingService.Application.Commands;
using DataTrackingService.Application.Queries;
using DataTrackingService.Application.Services;

using DataTrackingService.Messaging.Consumers;

var logger = LoggerFactory
    .Create(builder => builder.AddConsole())
    .CreateLogger("Program");
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });

// ------------------ Add migrations --------------
var migrations = new IMongoMigration[]
{
    // new AddPaymentStatusMigration(),
    // new RenameEnergyFieldMigration()
    // Add new migrations here manually
};
// Register each migration in DI
foreach (var migration in migrations)
{
    builder.Services.AddSingleton<IMongoMigration>(migration);
}

// MongoDB
builder.Services.Configure<MongoOptions>(
    builder.Configuration.GetSection("Mongo"));
builder.Services.AddSingleton<ITenantRegistry, TenantRegistry>();
builder.Services.AddScoped<IMongoDbContextFactory, MongoDbContextFactory>();

builder.Services.AddSingleton<MongoMigrationRunner>();
builder.Services.AddScoped<MongoIndexInitializer>(); // scoped because it uses IMongoDbContextFactory
builder.Services.AddHostedService<MongoStartupTask>();


builder.Services.AddScoped<ChargingSessionWriteService>();
builder.Services.AddScoped<ChargingSessionRepository>();
builder.Services.AddScoped<UserMonthlyUsageRepository>();
builder.Services.AddScoped<ProviderMonthlyRevenueRepository>();
builder.Services.AddScoped<ProviderDailyPowerUsageRepository>();

builder.Services.AddScoped<UserSpreadsheetRepository>();
builder.Services.AddScoped<UserSpreadsheetRowRepository>();

// Register in Memmory tenants
builder.Services.AddSingleton<ITenantRegistry>(_ =>
    new TenantRegistry(TenantBootstrap.GetMockTenants()));

// Application services
builder.Services.AddScoped<ChargingSessionReadService>();
builder.Services.AddScoped<ChargingSessionWriteService>();

builder.Services.AddScoped<UsageAggregationService>();
builder.Services.AddScoped<ProviderAggregationService>();

builder.Services.AddScoped<UserSpreadsheetReadService>();
builder.Services.AddScoped<UserSpreadsheetWriteService>();

// Kafka consumers and producers
builder.Services.AddHostedService<BillingEventsConsumer>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
    

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
