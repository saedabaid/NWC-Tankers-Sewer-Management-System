using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Models
{
    public class VehicleViolationDTO
    {
        public long ID { get; set; }
        public DateTime IncidentTime { get; set; }
        public string ViolationTicketNumber { get; set; }
        public Guid VehicleId { get; set; }
        public string TermName { get; set; }
        public string TermCode { get; set; }
        public string TermCategory { get; set; }
        public string ViolationStatus { get; set; }
        public decimal TotalPenalty { get; set; }

    }
}
