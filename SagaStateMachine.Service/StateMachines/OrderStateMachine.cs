using MassTransit;
using SagaStateMachine.Service.StateInstances;
using Shared.OrderEvents;
using Shared.PaymentEvents;
using Shared.StockEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaStateMachine.Service.StateMachines
{
    public class OrderStateMachine: MassTransitStateMachine<OrderStateInstance>
    {
        // Declate incoming Events
        public Event<OrderStartedEvent> OrderStartedEvent { get; set; }
        public Event<StockReservedEvent> StockReservedEvent { get; set; }
        public Event<PaymentCompletedEvent> PaymentCompletedEvent { get; set; }
        public Event<StockNotReserved> StockNotReservedEvent { get; set; }
        public Event<PaymentFailedEvent> PaymentFailedEvent { get; set; }


        public OrderStateMachine()
        {
            // state bhavior assign to CurrentState property
            InstanceState(instance => instance.CurrentState);

            // Event functon allows us to take action on incoming events
            // check orderId if equals not create correlationId else creat
            // dont forget OrderStarterdEvent is triggerEvent that mean special event
            Event(() => OrderStartedEvent, (orderStateInstance) =>
            {
                 orderStateInstance.CorrelateBy<int>(database => database.OrderId, @event => @event.Message.OrderId)
                .SelectId(e => Guid.NewGuid());
            });



            // we action assigning which OrderStateInstance equals with incoming event  
            Event(() => StockReservedEvent, (orderStateInstance) =>
            {
                // not trigger event so we continue with was created CorrelationId we just bind Id 
                orderStateInstance.CorrelateById(@event => @event.Message.CorrelationId);
            });

            // we action assigning which OrderStateInstance equals with incoming event  
            Event(() => StockReservedEvent, (orderStateInstance) =>
            {
                // not trigger event so we continue with was created CorrelationId we just bind Id 
                orderStateInstance.CorrelateById(@event => @event.Message.CorrelationId);
            });

            // we action assigning which OrderStateInstance equals with incoming event  
            Event(() => PaymentCompletedEvent, (orderStateInstance) =>
            {
                // not trigger event so we continue with was created CorrelationId we just bind Id 
                orderStateInstance.CorrelateById(@event => @event.Message.CorrelationId);
            });

            // we action assigning which OrderStateInstance equals with incoming event  
            Event(() => PaymentFailedEvent, (orderStateInstance) =>
            {
                // not trigger event so we continue with was created CorrelationId we just bind Id 
                orderStateInstance.CorrelateById(@event => @event.Message.CorrelationId);
            });
        }
    }
}
