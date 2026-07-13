using System;

namespace NWC.DTO.Models
{
    public class SignalRWorkOrderEvent
    {
        public long WorkOrderId { get; set; }
        public Guid? CityId { get; set; }
    }
}
