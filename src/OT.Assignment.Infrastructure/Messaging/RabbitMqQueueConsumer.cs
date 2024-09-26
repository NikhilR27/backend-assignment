using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using OT.Assignment.Application.Interfaces;
using OT.Assignment.Infrastructure.Messaging.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OT.Assignment.Infrastructure.Messaging;

public class RabbitMqQueueConsumer<T> : IQueueConsumer
{
    private readonly IModel _channel;
    private readonly RabbitMqSettings _rabbitMqSetting;
    private event Func<T, Task> OnMessageReceived;

    public RabbitMqQueueConsumer(IConnection connection, IOptions<RabbitMqSettings> rabbitMqSetting)
    {
        _rabbitMqSetting = rabbitMqSetting.Value;
        _channel = connection.CreateModel();
        _channel.QueueDeclare(queue: _rabbitMqSetting.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    public async Task ConsumeMessage(CancellationToken cancellationToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var receivedMessage = JsonSerializer.Deserialize<T>(message);

            if (receivedMessage != null)
            {
                await OnMessageReceived(receivedMessage);
            }

            _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };

        _channel.BasicConsume(queue: _rabbitMqSetting.QueueName,
            autoAck: false,
            consumer: consumer);

        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(1000, cancellationToken);
        }
    }
}