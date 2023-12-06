using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Context;
using Order.API.ViewModels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrderDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"));
});

builder.Services.AddMassTransit(configure =>
{
    configure.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(builder.Configuration.GetConnectionString("RabbitMQ"));
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapPost("create-order", async (CreateOrderVM model, OrderDbContext context) =>
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
});

app.Run();
