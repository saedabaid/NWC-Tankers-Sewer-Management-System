using NWC.DTO.Common;
using System;
using System.Collections.Generic;

namespace NWC.DTO.Models
{
    public class ContractorDTO
    {
        public long ID { get; set; }
        public string Code { get; set; }
        public string ContractorFullName { get; set; }
        public string CommercialIDNumber { get; set; }
        public string TaxNumber { get; set; }
        public string MOI { get; set; }
        public Guid CompanyAddressCityID { get; set; }
        public string CompanyAddressPostalCode { get; set; }
        public string CompanyAddress { get; set; }
        public bool IsActive { get; set; }
        public bool IsBlackListed { get; set; }

        //public bool IsDeleted { get; set; }

        // Contact Person
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

        public int? PersonalIDType { get; set; }
        public string PersonalIDNumber { get; set; }
        public string PersonAddressPostalCode { get; set; }
        public string PersonAddress { get; set; }
        public Guid? AreaId { get; set; }
        public string AreaName { get; set; }
        public string CityName { get; set; }

        public IEnumerable<AttachmentDTO> ContractorAttachments { get; set; }

    }
}
