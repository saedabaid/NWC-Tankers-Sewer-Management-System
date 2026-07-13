using NWC.DTO.Common;
using NWC.DTO.Models.TMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class ContractDTO
    {
        //public Guid SubID { get; set; }
        public long ID { get; set; }
        public string Code { get; set; }
        public long ContractorID { get; set; }
        public string ContractorFullName { get; set; }
        public int? ContractTypeID { get; set; }
        public string ContractTypeName { get; set; }
        public string ConfirmationNo { get; set; }
        public string AwardLetterNo { get; set; }
        public DateTime ContractStartDate { get; set; }
        public DateTime ContractEndDate { get; set; }
        public int? ContractStatusID { get; set; }
        public string ContractStatusName { get; set; }
        public int? ContractStatusEnumId { get; set; }
        //public DateTime? TerminatedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public int? StationsCount { get; set; }

        public string Description { get; set; }
        //public int? TerminationReasonID { get; set; }
        //public bool IsTerminated { get; set; } //for edit only


        public IEnumerable<AttachmentDTO> ContractAttachments { get; set; }

    }


    public class ViolationApprovalsDTO
    {
        public long? ID { get; set; }

        public StaffListDTO Staff { get; set; }

        public Guid? Branch { get; set; }
        public string BranchName { get; set; }
        public Guid? SubBranch { get; set; }
        public string SubBranchName { get; set; }
        public Guid? Landmark { get; set; }
        public string LandmarkName { get; set; }
        public int LevelNo { get; set; }
        public Guid StaffId { get; set; }

}
}
