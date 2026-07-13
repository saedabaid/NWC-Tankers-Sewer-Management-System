using System;
using System.Collections.Generic;

namespace NWC.DTO.Models
{
    public class OrderReassignmentDTO
    {
        public long Id { get; set; }
        public string OrderNumber { get; set; }
        public string Area { get; set; }
        public string City { get; set; }
        public string Station { get; set; }
        public string NewDriver { get; set; }
        public string NewTankerCode { get; set; }
        public DateTime ReassignmentDate { get; set; }
        public string OldDriver { get; set; }
        public string OldTankerCode { get; set; }
        public string OrderStatusName { get; set; }
        public string Reason { get; set; }
        public string CreatedBy { get; set; }
    }
}
