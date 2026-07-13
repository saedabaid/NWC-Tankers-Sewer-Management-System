using NWC.DAL.NWCEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class WorkOrderVehicleDTO
    {
        public long ID { set; get; }
        public Guid VehicleID { set; get; }
        public long WorkOrderID { set; get; }
        public string WorkOrderNumber { set; get; }
        public DateTime WorkOrderTime { set; get; }
        public int? LastStatusID { set; get; }
        public Guid? LastStatusBy { set; get; }
        public DateTime? LastStatusTime { set; get; }
        public Guid? DriverID { set; get; }
        public Boolean? IsAssigned { set; get; }
        public Guid? AssignedByID { set; get; }
        public DateTime? AssignTime { set; get; }
        public Boolean? IsInService { set; get; }
        public Guid? InServiceByID { set; get; }
        public DateTime? InServiceTime { set; get; }
        public Boolean? IsDeassigned { set; get; }
        public Guid? DeassignedByID { set; get; }
        public DateTime? DeassignedTime { set; get; }
        public int? DeassignedReason { set; get; }
        public string DeassignedComment { set; get; }

        #region Constructors
        public WorkOrderVehicleDTO()
        {

        }

        //public WorkOrderVehicleDTO(NWC_StateWorkOrderVehicle WOV)
        //{
        //    this.ID = WOV.ID;
        //    this.VehicleID = WOV.VehicleID;
        //    this.WorkOrderID = WOV.WorkOrderID;
        //    this.WorkOrderNumber = WOV.WorkOrderNumber;
        //    this.WorkOrderTime = WOV.WorkOrderTime;
        //    this.LastStatusID = WOV.LastStatusID;
        //    this.LastStatusBy = WOV.LastStatusBy;
        //    this.LastStatusTime = WOV.LastStatusTime;
        //    this.DriverID = WOV.DriverID;
        //    this.IsAssigned = WOV.IsAssigned;
        //    this.AssignedByID = WOV.AssignedByID;
        //    this.AssignTime = WOV.AssignTime;
        //    this.IsInService = WOV.IsInService;
        //    this.InServiceByID = WOV.InServiceByID;
        //    this.InServiceTime = WOV.InServiceTime;
        //    this.IsDeassigned = WOV.IsDeassigned;
        //    this.DeassignedByID = WOV.DeassignedByID;
        //    this.DeassignedTime = WOV.DeassignedTime;
        //    this.DeassignedReason = WOV.DeassignedReason;
        //    this.DeassignedComment = WOV.DeassignedComment;
        //}
        #endregion
    }
}
