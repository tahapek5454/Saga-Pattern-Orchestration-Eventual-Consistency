using MassTransit;
using SagaStateMachine.Service.StateInstances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SagaStateMachine.Service.StateMachines
{
    public class OrderStateMachine: MassTransitStateMachine<OrderStateInstance>
    {
        public OrderStateMachine()
        {
            // state bhavior assign to CurrentState property
            InstanceState(instance => instance.CurrentState);
        }
    }
}
