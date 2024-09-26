using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OT.Assignment.Application.Interfaces;
using OT.Assignment.Application.Services;
using OT.Assignment.Infrastructure;
using OT.Assignment.Infrastructure.Messaging;
using OT.Assignment.Infrastructure.Messaging.Models;
using OT.Assignment.Infrastructure.Persistence;
using RabbitMQ.Client;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddTransient<IDbConnection>(sp =>
            new SqlConnection(context.Configuration.GetConnectionString("DatabaseConnection")));
        services.Configure<RabbitMqSettings>(context.Configuration.GetSection("RabbitMQSettings"));
        services.AddSingleton<IConnection>(sp =>
        {
            var rabbitMqSettings = sp.GetRequiredService<IOptions<RabbitMqSettings>>().Value;
            var factory = new ConnectionFactory()
            {
                HostName = rabbitMqSettings.HostName,
                UserName = rabbitMqSettings.UserName,
                Password = rabbitMqSettings.Password
            };

            return factory.CreateConnection();
        });
        services.AddSingleton(typeof(IQueuePublisher<>), typeof(RabbitMqQueuePublisher<>));
        services.AddScoped(typeof(IPlayerCasinoRepository), typeof(PlayerCasinoRepository));
        services.AddScoped<IWagerService, WagerService>();
        services.AddHostedService<RabbitMqWagerBackgroundService>();
    })
    .Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Application started {time:yyyy-MM-dd HH:mm:ss}", DateTime.Now);

await host.RunAsync();

logger.LogInformation("Application ended {time:yyyy-MM-dd HH:mm:ss}", DateTime.Now);