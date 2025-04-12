using System;
using System.Threading.Tasks;
using Confluent.Kafka;

class Producer
{
    public static async Task Run()  // ✅ Make method async
    {
        Console.WriteLine("Starting Kafka Producer...");
        string? bootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS");
        string topic = "test-topic";

        if (string.IsNullOrEmpty(bootstrapServers))
        {
            Console.WriteLine("KAFKA_BOOTSTRAP_SERVERS environment variable is not set.");
            return;  // Exit if no Kafka bootstrap server is configured
        }

        Console.WriteLine($"KAFKA_BOOTSTRAP_SERVERS: {bootstrapServers}");

        var config = new ProducerConfig { BootstrapServers = bootstrapServers };

        using var producer = new ProducerBuilder<Null, string>(config).Build();

        Console.WriteLine($"Producing messages to Kafka topic: {topic}");
        Console.WriteLine("Type your message and press Enter (type 'exit' to quit):");

        while (true)
        {
            Console.Write("> ");
            string? message = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(message)) continue;  // Ignore empty input
            if (message.ToLower() == "exit") break;

            try
            {
                var deliveryResult = await producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
                Console.WriteLine($"✅ Delivered message to {deliveryResult.TopicPartitionOffset}");
            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine($"❌ Delivery failed: {e.Error.Reason}");
            }
        }

        Console.WriteLine("Kafka producer stopped.");
    }
}