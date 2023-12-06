using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(configure =>
{
    configure.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(builder.Configuration.GetConnectionString("RabbitMQ"));
    });
});

var app = builder.Build();


app.Run();
