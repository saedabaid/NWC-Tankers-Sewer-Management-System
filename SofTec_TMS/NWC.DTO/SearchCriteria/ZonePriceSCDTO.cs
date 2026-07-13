using NWC.DTO.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace NWC.DTO.SearchCriteria
{
    public class ZonePriceSCDTO
    {
        public enum OrderByExepression { Zone = 1, CustomerClassName = 2 }
        public OrderByExepression? OrderBy { get; set; }
        public SortDirection SortDirection { get; set; }
        public PageFilter PageFilter { get; set; }
        public List<Guid> StationIDs { get; set; }
    }
}
