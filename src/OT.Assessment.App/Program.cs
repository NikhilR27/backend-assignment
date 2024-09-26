using System.Data;
using System.Reflection;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using OT.Assignment.Application.Interfaces;
using OT.Assignment.Application.Services;
using OT.Assignment.Infrastructure.Messaging;
using OT.Assignment.Infrastructure.Messaging.Models;
using OT.Assignment.Infrastructure.Persistence;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckl
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddTransient<IDbConnection>(sp => 
    new SqlConnection(builder.Configuration.GetConnectionString("DatabaseConnection")));
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQSettings"));
builder.Services.AddSingleton<IConnection>(sp =>
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

// Force initialization of RabbitMQ connection
builder.Services.AddScoped(typeof(IPlayerCasinoRepository), typeof(PlayerCasinoRepository));
builder.Services.AddSingleton(typeof(IQueuePublisher<>), typeof(RabbitMqQueuePublisher<>));
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IWagerService, WagerService>();


var app = builder.Build();

var rabbitMqConnection = app.Services.GetRequiredService<IConnection>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(opts =>
    {
        opts.EnableTryItOutByDefault();
        opts.DocumentTitle = "OT Assessment App";
        opts.DisplayRequestDuration();
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
