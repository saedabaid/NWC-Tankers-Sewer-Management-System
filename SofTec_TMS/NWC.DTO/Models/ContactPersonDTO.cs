using NWC.DAL.NWCEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class ContactPersonDTO
    {
        // NWC_ContactPerson
        public long ID { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public string MobileCode { get; set; }
        public string Mobile { get; set; }
        public string LandlineNumbeCode { get; set; }
        public string LandlineNumber { get; set; }
        public string Email { get; set; }
        public Nullable<int> PersonalIDType { get; set; }
        public string PersonalIDNumber { get; set; }
        public string PersonAddressPostalCode { get; set; }
        public string PersonAddress { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<System.Guid> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<System.Guid> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public System.Guid SubID { get; set; }
    }
}
