using NWC.DAL.NWCEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class WorkOrderComplaintDTO
    {
        #region Properties
        public long WorkOrderID { set; get; }
        public string ComplaintNumber { set; get; }
        public string Description { set; get; }
        public Guid? RaisedBy { set; get; }
        public string RaisedByName { set; get; }
        public DateTime? RaisedTime { set; get; }
        public string Status { set; get; }
        public string Priority { set; get; }
        public string Category { set; get; }
        #endregion


        #region Constructors
        public WorkOrderComplaintDTO()
        {

        }

        public WorkOrderComplaintDTO(NWC_WorkOrderComplaint complaint)
        {
            this.WorkOrderID = complaint.WorkOrderID;
            this.ComplaintNumber = complaint.Number;
            this.RaisedBy = complaint.RaisedBy;
            this.RaisedTime = complaint.RaisedTime;
            this.Priority = complaint.NWC_ComplaintPriority.PriorityEn;
            this.Category = complaint.NWC_ComplaintCategory.CategoryEn;
            this.Status = complaint.NWC_ComplaintStatus.StatusEn;
        }
        #endregion
    }
}
