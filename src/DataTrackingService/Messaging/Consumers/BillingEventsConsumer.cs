using Confluent.Kafka;
using System.Text.Json;
using DataTrackingService.Messaging.Events;
using DataTrackingService.Messaging.Events.Consuming;
using DataTrackingService.Application.Commands;

namespace DataTrackingService.Messaging.Consumers;

public class BillingEventsConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _config;

    public BillingEventsConsumer(
        IServiceScopeFactory scopeFactory,
        IConfiguration config)
    {
        _scopeFactory = scopeFactory;
        _config = config;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        
        var logger = _scopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<ILogger<BillingEventsConsumer>>();

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _config["Kafka:BootstrapServers"],
            GroupId = "data-tracking.billing.events.v1",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        consumer.Subscribe("billing.events");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);

                if (string.IsNullOrWhiteSpace(result.Message.Value))
                    continue; // Skip if empty

                using var scope = _scopeFactory.CreateScope();
                var writeService = scope.ServiceProvider
                    .GetRequiredService<ChargingSessionWriteService>();

                var envelope =
                    JsonSerializer.Deserialize<EventEnvelope<JsonElement>>(result.Message.Value)!;

                if (envelope.EventType == "ChargingSessionFinalized")
                {
                    var finalized =
                        envelope.Payload.Deserialize<ChargingSessionFinalized>()!;

                    await writeService.SaveSessionBillingForUserAsync(finalized);
                    await writeService.SaveSessionBillingForTenantAsync(finalized);

                    logger.LogInformation("Session saved successfully");
                }

                consumer.Commit(result);
            }
            catch (ConsumeException ex) when (ex.Error.Code == ErrorCode.UnknownTopicOrPart)
            {
                await Task.Delay(2000, stoppingToken);
            }
        }
    }
}
