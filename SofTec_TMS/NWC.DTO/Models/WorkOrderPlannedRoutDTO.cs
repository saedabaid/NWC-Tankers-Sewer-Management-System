using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class WorkOrderPlannedRoutDTO
    {
        public long ID { set; get; }
        public long WorkOrderID { set; get; }
        public DateTime? CreatedTime { set; get; }
        public Guid CreatedBy { set; get; }
        public Boolean? IsDeleted { set; get; }
        public Guid? LastModifiedBy { set; get; }
        public DateTime? LastModifiedDate { set; get; }
        public string RouteJSON { set; get; }
        public string RouteLatLngString { set; get; }
        public long? DrivingTime { set; get; }
        public long? Distance { set; get; }
    }
}
