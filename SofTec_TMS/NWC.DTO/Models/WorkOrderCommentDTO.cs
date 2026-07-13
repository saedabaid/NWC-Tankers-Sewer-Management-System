using NWC.DAL.NWCEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class WorkOrderCommentDTO
    {
        #region Properties
        public long ID { set; get; }
        public long WorkOrderID { set; get; }
        public string Comment { set; get; }
        public DateTime? CreatedTime { set; get; }
        public Guid? CreatedBy { set; get; }
        public string CreatedByName { set; get; }
        public Boolean? IsDeleted { set; get; }
        public string Role { set; get; }
        //public DateTime? DeletedTime { set; get; }

        #endregion

        #region Constructors
        public WorkOrderCommentDTO()
        {

        }

        public WorkOrderCommentDTO(NWC_WorkOrderComment woComment)
        {
            this.ID = woComment.ID;
            this.WorkOrderID = woComment.WorkOrderID;
            this.Comment = woComment.Comment;
            this.CreatedTime = woComment.CreatedTime;
            this.CreatedBy = woComment.CreatedBy;
            this.IsDeleted = woComment.IsDeleted;
            this.Role = woComment.Staff.StaffRoleName;
        }
        #endregion
    }
}
