using MassTransit;
using Order.API.Context;
using Shared.OrderEvents;

namespace Order.API.Consumers
{
    public class OrderFailedEventConsumer : IConsumer<OrderFailedEvent>
    {
        private readonly OrderDbContext _context;

        public OrderFailedEventConsumer(OrderDbContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<OrderFailedEvent> context)
        {
            var order = _context.Orders.First(x => x.Id == context.Message.OrderId);

            order.OrderStatus = Enums.OrderStatus.Failed;

            _context.Update(order);

            await _context.SaveChangesAsync();
        }
    }
}
