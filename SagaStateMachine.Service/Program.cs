using MassTransit;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(configure =>
{
    configure.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(builder.Configuration.GetConnectionString("RabbitMQ"));
    });
});

var host = builder.Build();
host.Run();
