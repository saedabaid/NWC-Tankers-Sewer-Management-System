using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC_CCB_Integration.DTO.Models
{
    public class WorkOrderDTO
    {
        #region Properties
        public long WorkOrderID { get; set; }
        public string OrderNumber { get; set; }
        public DateTime? RequestTime { get; set; }
        public Boolean? IsAssigned { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerMobile { get; set; }
        public string CustomerAddress { get; set; }
        public int CustomerLocationClassID { get; set; }
        public string ClassName { get; set; }
        public int PriorityID { get; set; }
        public string PriorityName { get; set; }
        public long ZoneID { get; set; }
        public string ZoneName { get; set; }
        public Guid CityID { get; set; }
        public string CityName { get; set; }
        public int OrderQuantity { get; set; }
        public DateTime? ScheduledDeliveryTime { get; set; }
        public string StationName { get; set; }
        public string ServiceType { get; set; }
        public string TankerPlateNo { get; set; }
        public int AccessoryID { get; set; }
        public string AccessoryNames { get; set; }
        public Guid? AssignedDriverID { get; set; }
        public Guid? AssignedStationID { get; set; }
        public Guid LandmarkID { get; set; }
        public Guid? AssignedVehicleID { get; set; }
        public string VehicleCodePlateNo { get; set; }
        public long CustomerLocationID { get; set; }
        public int ServiceTypeID { get; set; }
        public long CustomerID { get; set; }
        public int? LastStatusID { get; set; }
        public string LastStatusName { get; set; }
        public Guid? LastStatusBy { get; set; }
        public string LastStatusByUserName { get; set; }
        public string customerLocationLng { get; set; }
        public string customerLocationLat { get; set; }
        public List<AccessoryDTO> AccessoryDTOs { get; set; }
        public string VehicleStatusName { get; set; }
        public Nullable<int> VehicleCapacity { get; set; }
        public Nullable<int> VehicleCapacityUnit { get; set; }
        public Nullable<System.DateTime> LastStatusTime { get; set; }
        public Nullable<System.DateTime> LastStatusTimeVehicle { get; set; }
        public string ConfirmationCode { get; set; }

        public decimal CostBeforVAT { get; set; }
        public decimal VAT { get; set; }
        public decimal CostAfterVAT { get; set; }

        public string PaymentStatusAr { get; set; }
        public string PaymentStatusEn { get; set; }
        #endregion

        #region Constructors
        public WorkOrderDTO()
        {
            AccessoryDTOs = new List<AccessoryDTO>();
        }
        #endregion
    }
}
