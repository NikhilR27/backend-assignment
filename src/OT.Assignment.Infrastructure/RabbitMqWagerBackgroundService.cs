using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OT.Assignment.Application.DTOs;
using OT.Assignment.Application.Interfaces;
using OT.Assignment.Infrastructure.Messaging.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OT.Assignment.Infrastructure;

public class RabbitMqWagerBackgroundService : BackgroundService
{
    private readonly ILogger<RabbitMqWagerBackgroundService> _logger;
    private readonly IWagerService _wagerService;
    private readonly IModel _channel;
    private readonly RabbitMqSettings _rabbitMqSettings;


    public RabbitMqWagerBackgroundService(ILogger<RabbitMqWagerBackgroundService> logger, IWagerService wagerService,
        IConnection connection,
        IOptions<RabbitMqSettings> rabbitMqSetting)
    {
        _logger = logger;
        _wagerService = wagerService;
        _rabbitMqSettings = rabbitMqSetting.Value;
        _channel = connection.CreateModel();
        _channel.QueueDeclare(queue: _rabbitMqSettings.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            Console.WriteLine("RabbitMQ service is starting");

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var wagerDto = JsonSerializer.Deserialize<WagerPublishedEventDto>(message);

                    if (wagerDto != null)
                    {
                        await HandleWagerMessageAsync(wagerDto);
                    }

                    _channel.BasicAck(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message from the queue.");
                }
            };

            _channel.BasicConsume(queue: _rabbitMqSettings.QueueName, autoAck: false, consumer: consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

            Console.WriteLine("RabbitMQ Service is stopping");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting RabbitMQ service.");
        }
    }

    private async Task HandleWagerMessageAsync(WagerPublishedEventDto wagerDto)
    {
        try
        {
            await _wagerService.SaveWagerAsync(wagerDto);

            _logger.LogInformation("Wager processed and saved: {WagerDtoWagerId}", wagerDto.WagerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing wager: {WagerDtoWagerId}", wagerDto.WagerId);
        }
    }
}