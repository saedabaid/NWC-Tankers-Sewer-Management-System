using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Models.ELM
{
    public class ELMTransactionDTO
    {
        public Nullable<System.Guid> TransactionId { get; set; }
        public Nullable<System.DateTime> ResponseTime { get; set; }
        public Nullable<bool> value { get; set; }
    }
}
