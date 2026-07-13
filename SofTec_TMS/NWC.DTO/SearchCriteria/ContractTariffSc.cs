using NWC.DTO.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.SearchCriteria
{
  public  class ContractTariffSc
    {
        public PageFilter PageFilter { get; set; }
        public List<long> ContractIDs { get; set; }
        public List<Guid> StationIDs { get; set; }

        public List<Guid> CityIDs { get; set; }
        public List<long> ZoneIDs { get; set; }
        public List<int> CustomerLocationClassIDs { get; set; }
        public List<int> ServiceTypeIDs { get; set; }
        public List<int> TankerCapacities { get; set; }
        public bool? IsDeleted { get; set; }

        public int? TarrifStatus { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public bool ExcelFlage { get; set; }
    }

    public enum ScheduleStatusEnum
    {
        FullScheduled = 1,
        PartiallyScheduled = 2,
        NotScheduled = 3,
    }
}
