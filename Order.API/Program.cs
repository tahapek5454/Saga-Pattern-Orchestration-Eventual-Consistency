using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Consumers;
using Order.API.Context;
using Order.API.ViewModels;
using Shared.OrderEvents;
using Shared.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrderDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"));
});

builder.Services.AddMassTransit(configure =>
{
    configure.AddConsumer<OrderCompletedEventConsumer>();
    configure.AddConsumer<OrderFailedEventConsumer>();

    configure.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(builder.Configuration.GetConnectionString("RabbitMQ"));


        configurator.ReceiveEndpoint(RabbitMQSettings.Order_OrderCompletedEventQueue, e =>
        {
            e.ConfigureConsumer<OrderCompletedEventConsumer>(context);
            e.DiscardSkippedMessages();

        });

        configurator.ReceiveEndpoint(RabbitMQSettings.Order_OrderFailedEventQueue, e =>
        {
            e.ConfigureConsumer<OrderFailedEventConsumer>(context);
            e.DiscardSkippedMessages();

        });

    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapPost("create-order", async (CreateOrderVM model, OrderDbContext context, ISendEndpointProvider sendEndpointProvider) =>
{
    Order.API.Models.Order order = new()
    {
        BuyerId = model.BuyerId,
        CreatedDate = DateTime.UtcNow,
        OrderStatus = Order.API.Enums.OrderStatus.Suspend,
        TotalPrice = model.OrderItems.Sum(x => x.Price * x.Count),
        OrderItems = model.OrderItems.Select(x => new Order.API.Models.OrderItem()
        {
            Count = x.Count,
            Price = x.Price,
            ProductId = x.ProductId,

        }).ToList()
    };

    await context.Orders.AddAsync(order);
    await context.SaveChangesAsync();

    // triger event
    OrderStartedEvent @orderStartedEvent = new()
    {
        BuyerId = order.BuyerId,
        OrderId = order.Id,
        TotalPrice = order.TotalPrice,
        OrderItems = order.OrderItems.Select(x => new Shared.Messages.OrderItemMessage()
        {
            Count = x.Count,
            Price = x.Price,
            ProductId = x.ProductId,
        }).ToList()
    };

    var sendEndPoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.StateMachineQueue}"));

    await sendEndPoint.Send(orderStartedEvent);

    // after that State Machine will take control. That will publish OrderCreatedEvent for StockAPI


});

app.Run();
