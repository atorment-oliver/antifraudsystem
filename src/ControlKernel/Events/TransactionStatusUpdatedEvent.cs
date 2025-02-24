using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlKernel.Events
{
    public class TransactionStatusUpdatedEvent
    {
        public Guid TransactionExternalId { get; set; }
        public string ? Status { get; set; }
    }
}
