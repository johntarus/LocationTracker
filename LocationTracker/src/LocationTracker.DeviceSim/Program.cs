using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace DeviceSim.DeviceSim;

class Program
{
    private static readonly Random _random = new();
    private static readonly string[] _deviceIds = { "DEV-001", "DEV-002", "DEV-003", "DEV-004" };
    
    static async Task Main(string[] args)
    {
        Console.Title = "Location Tracker Device Simulator";
        Console.WriteLine("=== Location Tracker Device Simulator ===");
        Console.WriteLine("Press Ctrl+C to exit\n");

        var messageDelayMs = 1000;
        var batchDelayMs = 5000; 
        var maxMessages = 1000;

        Console.WriteLine($"Configuration: {messageDelayMs}ms between messages, {batchDelayMs}ms between batches");

        try
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                Port = 5672
            };

            Console.WriteLine($"Connecting to RabbitMQ at {factory.HostName}:{factory.Port}...");

            await using var connection = await factory.CreateConnectionAsync("DeviceSimConnection");
            await using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "location_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            Console.WriteLine("Connected to RabbitMQ successfully!");
            Console.WriteLine("Sending location data...\n");

            var messageCount = 0;
            
            while (true)
            {
                foreach (var deviceId in _deviceIds)
                {
                    var locationData = new
                    {
                        DeviceId = deviceId,
                        Latitude = Math.Round(39.95 + (_random.NextDouble() - 0.5) * 0.01, 6),
                        Longitude = Math.Round(-75.15 + (_random.NextDouble() - 0.5) * 0.01, 6),
                        Timestamp = DateTime.UtcNow,
                        Speed = _random.Next(0, 100),
                        Bearing = _random.Next(0, 360),
                        Accuracy = _random.Next(1, 50),
                        Altitude = _random.Next(0, 500)
                    };

                    var message = JsonSerializer.Serialize(locationData);
                    var body = Encoding.UTF8.GetBytes(message);

                    await channel.BasicPublishAsync(
                        exchange: "",
                        routingKey: "location_queue",
                        body: body);

                    messageCount++;
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] #{messageCount:D4} Sent: {deviceId}");
                    
                    // Delay between individual messages
                    await Task.Delay(messageDelayMs);
                }

                Console.WriteLine($"--- Batch completed. Total messages: {messageCount} ---\n");
                
                // Optional: Limit total messages
                if (maxMessages > 0 && messageCount >= maxMessages)
                {
                    Console.WriteLine($"Reached maximum message limit of {maxMessages}. Exiting.");
                    break;
                }
                
                // Delay between batches
                await Task.Delay(batchDelayMs);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine("Make sure RabbitMQ is running on localhost:5672");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}