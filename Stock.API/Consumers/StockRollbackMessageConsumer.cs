using MassTransit;
using Shared.Messages;
using Stock.API.Context;

namespace Stock.API.Consumers
{
    public class StockRollbackMessageConsumer : IConsumer<StockRollbackMessage>
    {
        private readonly StockDbContext _dbContext;

        public StockRollbackMessageConsumer(StockDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task Consume(ConsumeContext<StockRollbackMessage> context)
        {
            var stocks = _dbContext.Stocks;

            List<Stock.API.Models.Stock> updateStock = new();

            foreach (var orderItem in context.Message.OrderItems)
            {
                var targetStock = stocks.First(x => x.ProductId == orderItem.ProductId);
                targetStock.Count += orderItem.Count;
                updateStock.Add(targetStock);
            }


            _dbContext.Stocks.UpdateRange(updateStock);
            await _dbContext.SaveChangesAsync();
        }
    }
}
