using NWC.DTO.Common;
using NWC.DTO.Models.TMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class PermitDTO
    {
        public System.Guid ID { get; set; }
        public Guid? DriverID { get; set; }
        public string DriverIDSTR { get; set; }
        public string DriverMobile { get; set; }
        public string TankerNumber { get; set; }
        public Nullable<System.Guid> TransporterID { get; set; }
        public string PermitNumber { get; set; }
        public string Discerption { get; set; }
        public string OrganizationName { get; set; }
        public string CRnumber { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> Expirationdate { get; set; }
        public string Availabletimefrom { get; set; }
        public string Availabletimeto { get; set; }
        public string TankerCategory { get; set; }
        public Nullable<System.DateTime> LastValidationDate { get; set; }
        public Nullable<System.DateTime> LastMaintenanceDate { get; set; }
        public Nullable<bool> Ismaramy { get; set; }
        public Nullable<bool> IsHold { get; set; }
        public string PermitStatus { get; set; }
        public string Maramu { get; set; }
        public string TripsNumber { get; set; }
        public string DetectionformFile { get; set; }
        public string DeclarationFile { get; set; }
        public string OtherFile { get; set; }
        public Nullable<System.Guid> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreationTime { get; set; }
        public Nullable<System.Guid> LastModifiedBy { get; set; }
        public Nullable<System.DateTime> LastModificationDate { get; set; }
        public virtual StaffDTO StaffDTO { get; set; }
        public virtual TransporterDTO TransporterDTO { get; set; }

        public string Area { get; set; }
        public string City { get; set; }
        public string Station { get; set; }

        public IEnumerable<AttachmentDTO> DeclarationFileAttachments { get; set; }
        public IEnumerable<AttachmentDTO> DetectionformFileAttachments { get; set; }
        public IEnumerable<AttachmentDTO> OtherFileAttachments { get; set; }
    }
}
