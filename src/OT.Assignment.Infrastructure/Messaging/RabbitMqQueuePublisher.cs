using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using OT.Assignment.Application.Interfaces;
using OT.Assignment.Infrastructure.Messaging.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace OT.Assignment.Infrastructure.Messaging;

public class RabbitMqQueuePublisher<T> : IQueuePublisher<T>
{
    private readonly IModel _channel;
    private readonly RabbitMqSettings _rabbitMqSettings;

    public RabbitMqQueuePublisher(IConnection connection, IOptions<RabbitMqSettings> rabbitMqSettings)
    {
        _rabbitMqSettings = rabbitMqSettings.Value;
        _channel = RetryPolicy(() => connection.CreateModel(), retryCount: 5, delayBetweenRetries: TimeSpan.FromSeconds(5));
        _channel.QueueDeclare(queue: _rabbitMqSettings.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    private IModel RetryPolicy(Func<IModel> action, int retryCount, TimeSpan delayBetweenRetries)
    {
        int attempt = 0;

        while (true)
        {
            try
            {
                attempt++;
                return action();
            }
            catch (BrokerUnreachableException ex) when (attempt <= retryCount)
            {
                Console.WriteLine($"Attempt {attempt}: Could not reach RabbitMQ broker. Retrying in {delayBetweenRetries.TotalSeconds} seconds... {ex.Message}");
            }
            catch (OperationInterruptedException ex) when (attempt <= retryCount)
            {
                Console.WriteLine($"Attempt {attempt}: Operation interrupted. Retrying in {delayBetweenRetries.TotalSeconds} seconds... {ex.Message}");
            }

            if (attempt >= retryCount)
            {
                throw new Exception($"Failed to establish connection after {retryCount} attempts.");
            }

            Thread.Sleep(delayBetweenRetries); 
        }
    }
    
    public void PublishMessage(T message)
    {
        try
        {
            var messageBody = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(messageBody);

            _channel.BasicPublish(exchange: "",
                routingKey: _rabbitMqSettings.QueueName,
                basicProperties: null,
                body: body);

            Console.WriteLine($"Sent message to queue {_rabbitMqSettings.QueueName}: {messageBody}");
        }
        catch (BrokerUnreachableException ex)
        {
            Console.WriteLine($"Could not reach RabbitMQ broker: {ex.Message}");
            throw;
        }
        catch (OperationInterruptedException ex)
        {
            Console.WriteLine($"Operation interrupted: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while publishing message: {ex.Message}");
            throw;
        }
    }
}