using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Confluent.Kafka;

public class KafkaConsumerService : BackgroundService
{
    private readonly IHubContext<KafkaHub> _hubContext;
    private readonly string _bootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS");
    private readonly string _topic = "test-topic";

    public KafkaConsumerService(IHubContext<KafkaHub> hubContext)
    {
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _bootstrapServers,
            GroupId = "test-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe(_topic);

        Console.WriteLine("Kafka Consumer started and subscribed to topic.");

        // Run Kafka consumer asynchronously
        await Task.Run(() => StartConsuming(consumer, stoppingToken), stoppingToken);
    }

    private async Task StartConsuming(IConsumer<Ignore, string> consumer, CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Console.WriteLine("Waiting for messages...");
                var consumeResult = consumer.Consume(stoppingToken);

                if (consumeResult != null)
                {
                    Console.WriteLine($"Received message: {consumeResult.Message.Value}");
                    await _hubContext.Clients.All.SendAsync("ReceiveMessage", consumeResult.Message.Value);
                }
            }
            catch (ConsumeException ex)
            {
                Console.WriteLine($"Kafka Error: {ex.Error.Reason}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
            }
        }

        consumer.Close();
        Console.WriteLine("Kafka Consumer stopped.");
    }
}