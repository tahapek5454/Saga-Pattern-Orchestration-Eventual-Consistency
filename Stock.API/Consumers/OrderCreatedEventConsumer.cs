using MassTransit;
using MassTransit.Transports;
using Microsoft.EntityFrameworkCore;
using Shared.OrderEvents;
using Shared.Settings;
using Shared.StockEvents;
using Stock.API.Context;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer: IConsumer<OrderCreatedEvent>
    {
        private readonly StockDbContext stockAPIDbContext;
        private readonly ISendEndpointProvider sendEndpointProvider;

        public OrderCreatedEventConsumer(StockDbContext dbContext, ISendEndpointProvider sendEndpointProvider)
        {
            stockAPIDbContext = dbContext;
            this.sendEndpointProvider = sendEndpointProvider;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            List<bool> stockResult = new List<bool>();


            var stocks = stockAPIDbContext.Stocks.AsQueryable();
            List<Stock.API.Models.Stock> updateStock = new();

            foreach (var orderItem in context.Message.OrderItems)
            {
                var isExisit = stocks.Where(x => x.ProductId == orderItem.ProductId && x.Count >= orderItem.Count).Any();
                stockResult.Add(isExisit);
            }



            var sendEndpoint = 
                await sendEndpointProvider.GetSendEndpoint
                (new Uri($"queue:{RabbitMQSettings.StateMachineQueue}"));

            if (stockResult.TrueForAll(x => x.Equals(true)))
            {
                foreach (var orderItem in context.Message.OrderItems)
                {
                    var stock = await stocks.FirstAsync(x => x.ProductId == orderItem.ProductId);
                    stock.Count -= orderItem.Count;

                    updateStock.Add(stock);
                }

                stockAPIDbContext.Stocks.UpdateRange(updateStock);
                stockAPIDbContext.SaveChanges();


                StockReservedEvent stockreservedevent = new(context.Message.CorrelationId)
                {
                    OrderItems = context.Message.OrderItems
                };

                await sendEndpoint.Send(stockreservedevent);
            }
            else
            {

                StockNotReservedEvent stockNotReservedEvent = new(context.Message.CorrelationId)
                {
                    Message = "Stok Yetersiz"
                };

                await sendEndpoint.Send(stockNotReservedEvent);
            }
        }
    }
}
