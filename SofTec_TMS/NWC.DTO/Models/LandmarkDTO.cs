using NWC.DAL.NWCEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class LandmarkDTO
    {
        public System.Guid Id { get; set; }
        public string name { get; set; }
        public string descr { get; set; }
        public string code { get; set; }
        public Nullable<System.Guid> landmarkTypeId { get; set; }
        public string landmarkTypeName { get; set; }
        public Nullable<System.Guid> branchId { get; set; }
        public string address { get; set; }
        public string telephone { get; set; }
        public string telephone2 { get; set; }
        public string mobile { get; set; }
        public string fax { get; set; }
        public Nullable<decimal> latitude { get; set; }
        public Nullable<decimal> longitude { get; set; }
        public Nullable<System.Guid> contactPerson { get; set; }
        public System.Guid SubId { get; set; }
        public System.Guid CreatedBy { get; set; }
        public System.DateTime CreationTime { get; set; }
        public Nullable<bool> isDeleted { get; set; }
        public Nullable<System.DateTime> LastModificationDate { get; set; }
        public Nullable<System.Guid> LastModifiedBy { get; set; }
        public Nullable<System.Guid> GeofenceId { get; set; }
        public Nullable<int> AVGAddonFillingTime { get; set; }
        public Nullable<int> WaterSourceID { get; set; }
        public Nullable<int> StationOwnershipID { get; set; }
        public Nullable<int> StatusID { get; set; }
    }
}
