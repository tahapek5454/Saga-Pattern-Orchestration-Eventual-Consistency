using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;
using Stock.API.Context;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<StockDbContext>(options =>
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


using var scope = builder.Services.BuildServiceProvider().CreateScope();
StockDbContext context = scope.ServiceProvider.GetRequiredService<StockDbContext>();

var seedDataList = context.Stocks.ToList();
Console.WriteLine("Stock Seed Datas");
foreach (var item in seedDataList)
{
    Console.WriteLine($"id: {item.Id}, productId: {item.ProductId}, count: {item.Count}");
}


app.Run();
