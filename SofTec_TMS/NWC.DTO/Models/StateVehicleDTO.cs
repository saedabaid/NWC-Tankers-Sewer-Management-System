using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class StateVehicleDTO
    {
        public long ID { set; get; }
        public Guid VehicleID { set; get; }
        public string VehicleCodePlateNo { get; set; }
        public Guid? DriverID { set; get; }
        public string DriverName { get; set; }
        public string DriverMobileNumber { get; set; }
        public string DriverCode { get; set; }
        public int? VehicleStatusId { set; get; }
        public string VehicleStatusName { get; set; }
        public string VehicleStatusNameAr { get; set; }
        public Boolean? IsAvailable { set; get; }
        public Guid? AvailableByID { set; get; }
        public DateTime? AvailableTime { set; get; }
        public Boolean? IsOutOfService { set; get; }
        public Guid? OutOfServiceByID { set; get; }
        public DateTime? OutOfServiceTime { set; get; }
        public int? OutOfServiceReason { set; get; }
        public string OutOfServiceComment { set; get; }
        public Boolean? IsBlacklisted { set; get; }
        public Guid? BlacklistedByID { set; get; }
        public DateTime? BlacklistedTime { set; get; }
        public int? BlacklistedReason { set; get; }
        public string BlacklistedComment { set; get; }
        public Boolean? IsParking { set; get; }
        public Guid? ParkingByID { set; get; }
        public DateTime? ParkingTime { set; get; }
        public Nullable<int> Capacity { get; set; }
        public Nullable<int> AccessoryID { get; set; }
        public string NameEn { get; set; }
        public string AmountUnit { get; set; }
        public Nullable<long> CustomerLocationClassID { get; set; }
        public Nullable<long> ZoneID { get; set; }
        public string VehicleStatusColorCode { get; set; }
        public Nullable<bool> CitySettings_ShowInvoice { get; set; }
        public Nullable<bool> CitySettings_ShowCustomerClassEntryGate { get; set; }
        //public Nullable<int> VehicleCustomerLocationClassId { get; set; }
        public List<int> VehicleCustomerLocationClassesIds { get; set; }
        public string PermitNumber { get; set; }
        public string PermitStatus { get; set; }        
        public Nullable<bool> IsHold { get; set; }
        public DateTime? Expirationdate { set; get; }
        public StateVehicleDTO()
        {

        }
    }
}
