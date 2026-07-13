using NWC.DTO.Common;
using System;

namespace NWC.DTO.SearchCriteria
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
