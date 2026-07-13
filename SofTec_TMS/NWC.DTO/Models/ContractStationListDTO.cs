using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class ContractStationListDTO
    {
        public System.Guid StationID { get; set; }
        public long ContractID { get; set; }
        public Nullable<long> ContactPersonID { get; set; }
        public string stationName { get; set; }
        public string stationCode { get; set; }
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; }
        public string LandlineNumber { get; set; }
        public string LandlineNumbeCode { get; set; }
        public string MobileCode { get; set; }
        public string Email { get; set; }
        public string PersonAddress { get; set; }
        public string PersonAddressPostalCode { get; set; }
        public string Position { get; set; }
        public Nullable<int> PersonalIDType { get; set; }
        public Nullable<bool> isDeleted { get; set; }
        public Nullable<System.Guid> branchId { get; set; }
        public Nullable<System.Guid> AreaId { get; set; }
        public System.Guid SubId { get; set; }
        public string PersonalIDNumber { get; set; }
        public long contractStationID { get; set; }
        public string BranchName { get; set; }

    }
}
