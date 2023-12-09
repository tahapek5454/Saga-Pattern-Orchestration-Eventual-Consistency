using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.StockEvents
{
    public class StockNotReserved : CorrelatedBy<Guid>
    {
        public StockNotReserved(Guid correlationId)
        {
            CorrelationId = correlationId;
        }
        public Guid CorrelationId { get; }
        public string Message { get; set; }
    }
}
