using MassTransit;
using Order.API.Context;
using Shared.OrderEvents;

namespace Order.API.Consumers
{
    public class OrderCompletedEventConsumer : IConsumer<OrderCompletedEvent>
    {
        private readonly OrderDbContext _context;

        public OrderCompletedEventConsumer(OrderDbContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<OrderCompletedEvent> context)
        {
            var order = _context.Orders.First(x => x.Id == context.Message.OrderId);

            order.OrderStatus = Enums.OrderStatus.Completed;

            _context.Update(order);

            await _context.SaveChangesAsync();  
        }
    }
}
