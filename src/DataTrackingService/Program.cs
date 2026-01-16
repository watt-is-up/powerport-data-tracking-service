using System.Text.Json.Serialization;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.NpgSql;
using System.Text.Json;  
using MongoDB.Driver;

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
    new TenantRegistry(TenantBootstrap.LoadFromConfiguration(builder.Configuration)));

// Application services
builder.Services.AddScoped<ChargingSessionReadService>();
builder.Services.AddScoped<ChargingSessionWriteService>();

builder.Services.AddScoped<UsageAggregationService>();
builder.Services.AddScoped<ProviderAggregationService>();

builder.Services.AddScoped<UserSpreadsheetReadService>();
builder.Services.AddScoped<UserSpreadsheetWriteService>();

// Kafka consumers and producers
builder.Services.AddHostedService<BillingEventsConsumer>();

// Add SwaggerGen
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

});


// Add health checks
var mongoHost = builder.Configuration["Mongo:Host"];
var mongoPort = builder.Configuration["Mongo:Port"];
var mongoConnectionString = $"mongodb://{mongoHost}:{mongoPort}";

builder.Services.AddHealthChecks()
    .AddCheck(
        "self",
        () => HealthCheckResult.Healthy(),
        tags: new[] { "live" })
    .AddMongoDb(
        sp => new MongoClient(mongoConnectionString),
        name: "mongodb",
        tags: new[] { "ready" }
    );



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
    

var app = builder.Build();


/* if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
} */

// Always add Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Health endpoints
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("live"),
    ResponseWriter = WriteHealthResponse
})
.WithTags("Health")
.AllowAnonymous();

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("ready"),
    ResponseWriter = WriteHealthResponse
})
.WithTags("Health")
.AllowAnonymous();


app.UseHttpsRedirection();
app.MapControllers();
app.Run();


static Task WriteHealthResponse(HttpContext context, HealthReport report)
{
    context.Response.ContentType = "application/json";

    var result = JsonSerializer.Serialize(new
    {
        status = report.Status.ToString(),
        checks = report.Entries.Select(e => new
        {
            name = e.Key,
            status = e.Value.Status.ToString(),
            description = e.Value.Description
        })
    });

    return context.Response.WriteAsync(result);
}