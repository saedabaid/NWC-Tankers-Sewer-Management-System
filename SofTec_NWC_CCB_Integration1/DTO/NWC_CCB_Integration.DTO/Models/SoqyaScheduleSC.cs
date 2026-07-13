using NWC_CCB_Integration.DTO.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Models
{
    public class SoqyaScheduleSC
    {
        public PageFilter PageFilter { get; set; }
        public long? Id { get; set; }
        public long? CustomerAccountId { get; set; }
        public int? MonthYear { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
