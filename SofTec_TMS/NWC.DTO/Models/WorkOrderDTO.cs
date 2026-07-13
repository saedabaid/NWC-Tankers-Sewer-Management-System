using System;
using NWC.DAL.NWCEntities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class WorkOrderDTO
    {
        #region Properties
        public long WorkOrderID { get; set; }
        public string OrderNumber { get; set; }
        public DateTime? RequestTime { get; set; }
        public string Status { get; set; }
        public Boolean? IsAssigned { get; set; }
        public DateTime? orderPlacedOn { get; set; }
        public string CustomerName { get; set; }
        public string CustomerMobile { get; set; }
        public string CustomerCode { get; set; }
        public string Comments { get; set; }
        public string CustomerAddress { get; set; }
        public int CustomerLocationClassID { get; set; }
        public string ClassName { get; set; }
        public int PriorityID { get; set; }
        public string PriorityName { get; set; }
        public long ZoneID { get; set; }
        public string ZoneName { get; set; }

        public string ZoneWithOutTankers { get; set; }
        public Guid? CityID { get; set; }
        public string AreaName { get; set; }
        public string CityName { get; set; }
        public string paymentType { get; set; }
        public decimal TotalCost { get; set; }
        public int OrderQuantity { get; set; }
        public DateTime? ScheduledDeliveryTime { get; set; }
        //public Guid? StationID { get; set; }
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
        public long? CustomerLocationID { get; set; }
        public int? PaymentTypeID { get; set; }
        public int? ServiceTypeID { get; set; }
        public long CustomerID { get; set; }
        public Guid? transporterID { get; set; }
        public string DriverName { get; set; }
        public int? LastStatusID { get; set; }
        public string LastStatusName { get; set; }
        public Guid? LastStatusBy { get; set; }
        public string LastStatusByUserName { get; set; }
       // public DateTime? LastStatusTime { get; set; }
        public string customerLocationLng { get; set; }
        public string customerLocationLat { get; set; }
        public string StationLng { get; set; }
        public string StationLat { get; set; }
        public List<AccessoryDTO> AccessoryDTOs { get; set; }
        public string VehicleStatusName { get; set; }
        public Nullable<int> VehicleCapacity { get; set; }
        public Nullable<int> VehicleCapacityUnit { get; set; }
        public Nullable<System.DateTime> LastStatusTime { get; set; }
        public Nullable<System.DateTime> LastStatusTimeVehicle { get; set; }
        public string RouteLatLngString { get; set; }
        public string CustomerLocationPriority { get; set; }
        public string ConfirmationCode { get; set; }
        public string LastStatusReason { get; set; }
        public string DriverMobileNumber { get; set; }
        public string DriverCode { get; set; }
        public string WorkOrderInvoiceStatus { get; set; }

        public List<WorkOrderStatusLogDTO> WorkOrderStatusLogs { get; set; }

        public string StatusColor { get; set; }

        public decimal NetCost { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? AssignedOn { get; set; }
        public DateTime? OutForDeliveryOn { get; set; }
        public DateTime? ArrivedOn { get; set; }
        public DateTime? DeliverOn { get; set; }
        public DateTime? FailedToDeliverOn { get; set; }
        public DateTime? CancelledOn { get; set; }
        public int? InvoiceStatusID { get; set; }
        public string InvoiceStatusName { get; set; }
        public string CreateToDeliveredTime { get; set; }

        public string CreateToOutTime { get; set; }
        public string OutToDeliveredTime { get; set; }

        public decimal CostBeforVAT { get; set; }
        public decimal VAT { get; set; }
        public decimal CostAfterVAT { get; set; }
        
        public string PaymentStatusAr { get; set; }
        public string PaymentStatusEn { get; set; }
        public long? CustomerAccountID { get; set; }
        public string SourceApplication { get; set; }
        public string LastStatusByCategory { get; set; }
        public int? CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string InvoiceNo { get; set; }

        public string ClosedByCode { get; set; }
        public bool ValidateConfermationCode { get; set; }
        public bool IsVirtualStation { get; set; }

        public List<Guid> PrevuosAssignedDriverIDs { get; set; }
        #endregion

        #region Constructors
        public WorkOrderDTO()
        {
            AccessoryDTOs = new List<AccessoryDTO>();
            WorkOrderStatusLogs = new List<WorkOrderStatusLogDTO>();
        }
        #endregion
    }
}
