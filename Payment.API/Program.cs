using MassTransit;
using Payment.API.Consumers;
using Shared.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(configure =>
{
    configure.AddConsumer<PaymentStartedEventConsumer>();

    configure.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(builder.Configuration.GetConnectionString("RabbitMQ"));

        configurator.ReceiveEndpoint(RabbitMQSettings.Payment_PaymentStartedEventQueue, e =>
        {
            e.ConfigureConsumer<PaymentStartedEventConsumer>(context);
            e.DiscardSkippedMessages();
        });
    });
});

var app = builder.Build();


app.Run();
