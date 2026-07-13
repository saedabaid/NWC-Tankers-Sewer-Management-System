using System;

namespace NWC.DTO.Models
{
    public class WorkOrderChangeLogDTO
    {
        public long ID { get; set; }
        public long WorkOrderId { get; set; }
        public int ActionLogTypeID { get; set; }
        //public long EventOrderId { get; set; }
        //public long? ParentWorkOrderId { get; set; }
        //public long EventId { get; set; }
        public Guid? CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime? CreateTime { get; set; }
        //public int EventTypeID { get; set; }
        public int? StatusID { get; set; }
        public string StatusName { get; set; }
        //public string WOStatusReason { get; set; }
        public string StatusComment { get; set; }
        public int? DeassignReasonID { get; set; }
        public string DeassignReasonName { get; set; }
        public int? OrderQuantity { get; set; }
        public long? CustomerLocationID { get; set; }
        public int? ServiceTypeID { get; set; }
        public decimal? NetCost { get; set; }
        public decimal? TotalCost { get; set; }
        public string Accessories { get; set; }
        public decimal? Distance { get; set; }
        //public long? EventOrderVehicleId { get; set; }
        public Guid? VehicleID { get; set; }
        public Guid? DriverID { get; set; }
        public int? VehicleStatusId { get; set; }
        public string ActionLogTypeName { get; set; }
        public string VehicleStatusName { get; set; }
        public string ServiceTypeName { get; set; }
        public string VehicleCodePlateNo { get; set; }
        public string DriverName { get; set; }
        //public string OrderNumber { get; set; }
        public DateTime? ScheduledDeliveryTime { get; set; }
        public DateTime? StatusTime { get; set; }
        //public bool? IsPaid { get; set; }
        public long? CommentId { get; set; }
        public string Comment { get; set; }
        public bool? CommentIsDeleted { get; set; }

        public string StationName { get; set; }
        public string CustomerAddress { get; set; }
        public string ZoneName { get; set; }
        public string CityName { get; set; }



        public string PreviousStatusName { get; set; }
        public DateTime? PreviousStatusTime { get; set; }
        public string PreviousStatusComment { get; set; }
        public string PreviousVehicleCodePlateNo { get; set; }
        public string PreviousVehicleStatusName { get; set; }
        public string PreviousDriverName { get; set; }
        public string PreviousDeassignReasonName { get; set; }
        public string PreviousAccessories { get; set; }
        public decimal? PreviousDistance { get; set; }
        public int? PreviousOrderQuantity { get; set; }
        public DateTime? PreviousScheduledDeliveryTime { get; set; }
        public bool? PreviousIsPaid { get; set; }

        public string PreviousServiceTypeName { get; set; }
        public string PreviousStationName { get; set; }
        public string PreviousCustomerAddress { get; set; }
        public string PreviousZoneName { get; set; }
        public string PreviousCityName { get; set; }
        public decimal? PreviousTotalCost { get; set; }

    }
}
