using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Settings
{
    public static class RabbitMQSettings
    {
        public const string StateMachineQueue = "state-machine-queue";
        public const string Stock_OrderCreatedEventQueue = "stock_order_created_event_queue";
        public const string Order_OrderCompletedEventQueue = "order_order_completed_event_queue";
        public const string Order_OrderFailedEventQueue = "order_order_failed_event_queue";
        public const string Stock_RollbackMessageQueue = "stock_rollback_message_queue";
        public const string Payment_PaymentStartedEventQueue = "payment_payment_started_event_queue";
    }
}
