using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;
using Shared.Settings;
using Stock.API.Consumers;
using Stock.API.Context;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<StockDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"));
});

builder.Services.AddMassTransit(configure =>
{

    configure.AddConsumer<OrderCreatedEventConsumer>();
    configure.AddConsumer<StockRollbackMessageConsumer>();

    configure.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(builder.Configuration.GetConnectionString("RabbitMQ"));


        configurator.ReceiveEndpoint(RabbitMQSettings.Stock_OrderCreatedEventQueue, e =>
        {
            e.ConfigureConsumer<OrderCreatedEventConsumer>(context);
            e.DiscardSkippedMessages();
        });

        configurator.ReceiveEndpoint(RabbitMQSettings.Stock_RollbackMessageQueue, e =>
        {
            e.ConfigureConsumer<StockRollbackMessageConsumer>(context);
            e.DiscardSkippedMessages();
        });
    });
});

var app = builder.Build();


using var scope = builder.Services.BuildServiceProvider().CreateScope();
StockDbContext context = scope.ServiceProvider.GetRequiredService<StockDbContext>();

var seedDataList = context.Stocks.ToList();
Console.WriteLine("Stock Seed Datas");
foreach (var item in seedDataList)
{
    Console.WriteLine($"id: {item.Id}, productId: {item.ProductId}, count: {item.Count}");
}


app.Run();
