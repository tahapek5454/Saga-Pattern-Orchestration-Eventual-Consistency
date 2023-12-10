using MassTransit;
using SagaStateMachine.Service.StateInstances;
using Shared.Messages;
using Shared.OrderEvents;
using Shared.PaymentEvents;
using Shared.Settings;
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


        // states
        public State OrderCreated { get; set; }
        public State StockReserved { get; set; }
        public State StockNotReserved { get; set; }
        public State PaymentCompleted { get; set; }
        public State PaymentFailed { get; set; }

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


            // when triger event incoming. First initially function work
            // note: context.Instance is istance in database, contex.Data is incoming event data
            Initially(When(OrderStartedEvent)
                .Then(context =>
                {
                    context.Instance.OrderId = context.Data.OrderId;
                    context.Instance.BuyerId = context.Data.BuyerId;
                    context.Instance.TotalPrice = context.Data.TotalPrice;
                })
                .TransitionTo(OrderCreated)
                .Send(new Uri($"queue:{RabbitMQSettings.Stock_OrderCreatedEventQueue}"), 
                context => new OrderCreatedEvent(context.Instance.CorrelationId)
                {
                    OrderItems = context.Data.OrderItems,
                }));

            // incoming event after triger event - During function work 
            During(OrderCreated, 
                When(StockReservedEvent)
                .TransitionTo(StockReserved)
                .Send(new Uri($"queue:{RabbitMQSettings.Payment_PaymentStartedEventQueue}"),
                context => new PaymentStartedEvent(context.Instance.CorrelationId)
                {
                    OrderItems = context.Data.OrderItems,
                    TotalPrice = context.Data.OrderItems.Sum(x => x.Count * x.Price) // context.Instance.TotalPrice
                }),
                When(StockNotReservedEvent)
                .TransitionTo(StockNotReserved)
                .Send(new Uri($"queue:{RabbitMQSettings.Order_OrderFailedEventQueue}"),
                context => new OrderFailedEvent()
                {
                    OrderId = context.Instance.OrderId,
                    Message = context.Data.Message
                }));


            During(StockReserved,
                When(PaymentCompletedEvent)
                .TransitionTo(PaymentCompleted)
                .Send(new Uri($"queue:{RabbitMQSettings.Order_OrderCompletedEventQueue}"),
                context => new OrderCompletedEvent()
                {
                    OrderId = context.Instance.OrderId
                })
                .Finalize(), // final the sequence and delete stateInstance
                When(PaymentFailedEvent)
                .TransitionTo(PaymentFailed)
                .Send(new Uri($"queue:{RabbitMQSettings.Order_OrderFailedEventQueue}"),
                context => new OrderFailedEvent()
                {
                    OrderId = context.Instance.OrderId,
                    Message = context.Data.Message
                })
                .Send(new Uri($"queue:{RabbitMQSettings.Order_OrderFailedEventQueue}"),
                context => new StockRollbackMessage()
                {
                    OrderItems = context.Data.OrderItems
                }));


            SetCompletedWhenFinalized(); // final the sequence and delete stateInstance
        }
    }
}
