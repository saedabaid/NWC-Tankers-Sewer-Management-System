using System;
using System.Collections.Generic;

namespace NWC.DTO.Models
{
    public class StateWorkOrderVehicleDTO
    {
        public long ID { get; set; }
        public Guid? VehicleID { get; set; }
        public int? VehicleStatusId { get; set; }
        public string VehicleStatusName { get; set; }
        // public string VehicleStatusNameAr { get; set; }
        public string DriverName { get; set; }
        public string DriverMobileNumber { get; set; }
        public string DriverCode { get; set; }
        public bool? IsAssigned { get; set; }
        public bool? IsDeassigned { get; set; }
        public bool? IsInService { get; set; }
        public long WorkOrderID { get; set; }
        public string WorkOrderNumber { get; set; }
        public decimal TotalCost { get; set; }
        public string VehicleCode { get; set; }
        public string VehicleCodePlateNo { get; set; }
        public DateTime? LastStatusTime { get; set; }
        public string VehicleStatusColorCode {get; set;}
        public int LastStatusID { get; set;}
        public DateTime? OrderCreateTime { get; set; }
        public Nullable<int> Capacity { get; set; }
        public Nullable<System.Guid> CityId { get; set; }
        public Nullable<bool> CitySettings_ShowInvoice { get; set; }
        public Nullable<bool> CitySettings_ShowCustomerClassEntryGate { get; set; }
        //public Nullable<int> VehicleCustomerLocationClassId { get; set; }
        public List<int> VehicleCustomerLocationClassesIds { get; set; }
        public string PermitNumber { get; set; }
        public string PermitStatus { get; set; }
        public Nullable<bool> IsHold { get; set; }
        public DateTime? Expirationdate { set; get; }
    }
}
