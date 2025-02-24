using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlKernel.DTO
{
    public class TransactionStatusDTO
    {
        public Guid TransactionExternalId { get; set; }
        public string ? Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
