using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlKernel.Events
{
    public class TransactionCreatedEvent
    {
        public Guid TransactionExternalId { get; set; }
        public decimal Value { get; set; }
    }
}
