using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using NWC.DTO.Common;

namespace NWC.DTO.Models
{
    public class VehicleTypeDTO
    {
        public PageFilter PageFilter { get; set; }
        public enum OrderByExepression { ContarctorName = 1, Code = 2, CommericalID = 3, MOI = 4, TaxNumber = 5, ContactPerson = 6 }
        public OrderByExepression? OrderBy { get; set; }
        public SortDirection SortDirection { get; set; }
        public string Name { get; set; }
        public short Category { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public Guid ID { get; set; }
    }
}
