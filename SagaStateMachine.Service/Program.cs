using MassTransit;
using Microsoft.EntityFrameworkCore;
using SagaStateMachine.Service.StateDbContext;
using SagaStateMachine.Service.StateInstances;
using SagaStateMachine.Service.StateMachines;
using Shared.Settings;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(configure =>
{

    configure.AddSagaStateMachine<OrderStateMachine, OrderStateInstance>()
    .EntityFrameworkRepository(options => 
    {

        options.AddDbContext<DbContext, OrderStateDbContext>((provider, _builder) =>
        {
            //_builder.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"));
            _builder.UseSqlServer(builder.Configuration.GetConnectionString("MSSQL"));
        });

    }); 

    configure.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(builder.Configuration.GetConnectionString("RabbitMQ"));

        configurator.ReceiveEndpoint(RabbitMQSettings.StateMachineQueue, e =>
        {
            e.ConfigureSaga<OrderStateInstance>(context);
            e.DiscardSkippedMessages();
        });
    });
});

var host = builder.Build();
host.Run();
