using MassTransit;
using Shared.PaymentEvents;
using Shared.Settings;

namespace Payment.API.Consumers
{
    public class PaymentStartedEventConsumer : IConsumer<PaymentStartedEvent>
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public PaymentStartedEventConsumer(ISendEndpointProvider sendEndpointProvider)
        {
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task Consume(ConsumeContext<PaymentStartedEvent> context)
        {
            var array = new int[10] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Random rnd = new Random();
            int index = rnd.Next(array.Length);


            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.StateMachineQueue}"));

            if (index < 9)
            {
                // everything okey
                Console.WriteLine($"{context.Message.TotalPrice} TL tutarlı ödeme başarılı bir " +
                    $"şekilde gerçekleşti");

                PaymentCompletedEvent paymentCompletedEvent = new(context.Message.CorrelationId)
                {
                    
                };

                await sendEndpoint.Send(paymentCompletedEvent);
            }
            else
            {
                Console.WriteLine($"{context.Message.TotalPrice} TL tutarlı ödeme başarısız. ");

                PaymentFailedEvent paymentFaildEvent = new(context.Message.CorrelationId)
                {
                    Message = $"{context.Message.TotalPrice} TL tutarlı ödeme başarısız. ",
                    OrderItems = context.Message.OrderItems
                };

                await sendEndpoint.Send(paymentFaildEvent);

            }
        }
    }
}
