using NWC.DTO.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.BL.Denormalizer
{
    public class ReturnResult
    {
        public long WorkOrderID { get; set; }
        public bool IsDone { get; set; }
        public string ErrorMessage { get; set; }
        public ErrorStatus ErrorStatus { get; set; }
    }
}
