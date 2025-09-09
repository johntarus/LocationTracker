using System.Text;
using System.Text.Json;
using LocationTracker.Core.DTOs;
using LocationTracker.Core.UseCases;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LocationTracker.Infrastructure.Messaging;

public class RabbitConsumerService : IHostedService, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private RabbitMQ.Client.IConnection _connection;
    private RabbitMQ.Client.IModel _channel;
    private readonly string _queueName = "location_queue";
    private Task _executingTask;
    private CancellationTokenSource _cts;
    
    public RabbitConsumerService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        InitializeRabbitMQ();
        _executingTask = ExecuteAsync(_cts.Token);
        
        return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
    }
    
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_executingTask == null)
        {
            return;
        }
        
        try
        {
            _cts.Cancel();
        }
        finally
        {
            await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }
    }
    
    private void InitializeRabbitMQ()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        
        _channel.QueueDeclare(
            queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }
    
    private async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
    
        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
        
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var ingestLocationUseCase = scope.ServiceProvider.GetRequiredService<IngestLocationUseCase>();
                var locationDto = JsonSerializer.Deserialize<LocationDto>(message);
            
                if (locationDto != null)
                {
                    await ingestLocationUseCase.Execute(locationDto);
                    Console.WriteLine($"Processed location from device: {locationDto.DeviceId}");
                
                    _channel.BasicAck(ea.DeliveryTag, false);
                    Console.WriteLine($"-> Acknowledged message for {locationDto.DeviceId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
                // REQUEUE THE MESSAGE ON FAILURE
                _channel.BasicNack(ea.DeliveryTag, false, true);
                Console.WriteLine($"-> Re-queued message for retry");
            }
        };
    
        _channel.BasicConsume(
            queue: _queueName,
            autoAck: false,
            consumer: consumer);
    
        Console.WriteLine("RabbitMQ Consumer started. Waiting for messages...");
    
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
    
    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        _cts?.Dispose();
    }
}
