using DataTrackingService.Application.Commands;
using DataTrackingService.Application.Queries;
using DataTrackingService.Data.Mongo.Spreadsheets;
using DataTrackingService.Data.Mongo;
using DataTrackingService.Domain.Models.Spreadsheets;
using Mongo2Go;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using Xunit;

public class UserSpreadsheetServiceTests : IDisposable
{
    private readonly MongoDbRunner _runner;
    private readonly UserSpreadsheetWriteService _writeService;
    private readonly UserSpreadsheetReadService _readService;
    private readonly IMongoDbContextFactory _factory;
    private readonly ITenantRegistry _tenantRegistry = new TenantRegistry();
    private readonly IConfiguration _configuration = new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "MongoDb:ConnectionString", "mongodb://localhost:27017" },
            { "MongoDb:SharedDatabaseName", "shared_database" }
        })
        .Build();

    public UserSpreadsheetServiceTests()
    {
        _runner = MongoDbRunner.Start();
        var client = new MongoClient(_runner.ConnectionString);
        _factory = new MongoDbContextFactory(client, _tenantRegistry, _configuration);

        var sheetRepo = new UserSpreadsheetRepository(_factory);
        var rowRepo = new UserSpreadsheetRowRepository(_factory);
        _writeService = new UserSpreadsheetWriteService(sheetRepo, rowRepo);
        _readService = new UserSpreadsheetReadService(sheetRepo, rowRepo);
    }

    [Fact]
    public async Task AddColumn_ShouldAddColumnToSpreadsheet()
    {
        var sheet = await _writeService.CreateSpreadsheetAsync("user1", "My Sheet");

        var column = new SpreadsheetColumn
        {
            ColumnId = Guid.NewGuid().ToString(),
            Name = "Amount",
            Type = SpreadsheetColumnType.Number,
            Required = true
        };

        await _writeService.AddColumnAsync(sheet.Id, "user1", column);

        var updatedSheet = await _readService.GetSpreadsheetAsync(sheet.Id, "user1");
        Assert.Single(updatedSheet.Columns);
        Assert.Equal("Amount", updatedSheet.Columns[0].Name);
    }

    [Fact]
    public async Task AddRow_ShouldAddRowSuccessfully()
    {
        var sheet = await _writeService.CreateSpreadsheetAsync("user1", "My Sheet");
        var column = new SpreadsheetColumn
        {
            ColumnId = Guid.NewGuid().ToString(),
            Name = "Amount",
            Type = SpreadsheetColumnType.Number,
            Required = true
        };
        await _writeService.AddColumnAsync(sheet.Id, "user1", column);

        var values = new Dictionary<string, object?>
        {
            { column.ColumnId, 42 }
        };        
        await _writeService.AddRowAsync(sheet.Id, "user1", values);

        var rows = await _readService.GetRowsAsync(sheet.Id, "user1");
        Assert.Single(rows);
        Assert.Equal(42, rows[0].Values[column.ColumnId]);
    }

    [Fact]
    public async Task AddRow_ShouldThrowValidationError_WhenRequiredColumnMissing()
    {
        var sheet = await _writeService.CreateSpreadsheetAsync("user1", "My Sheet");
        var column = new SpreadsheetColumn
        {
            ColumnId = Guid.NewGuid().ToString(),
            Name = "Amount",
            Type = SpreadsheetColumnType.Number,
            Required = true
        };
        await _writeService.AddColumnAsync(sheet.Id, "user1", column);

        var values = new Dictionary<string, object?>(); // Missing "Amount"

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _writeService.AddRowAsync(sheet.Id, "user1", values)
        );

    }

    public void Dispose() => _runner.Dispose();
}
