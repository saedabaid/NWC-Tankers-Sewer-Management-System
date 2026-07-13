using NWC.DTO.Common;
using NWC.DTO.Models;

namespace NWC.BLL.Interfaces
{
    public interface ISignalRService
    {
        DescriptiveResponse<bool> WorkOrderCreated(WorkOrderDTO workOrderDTO);
        DescriptiveResponse<bool> WorkOrderConfirmed(SignalRWorkOrderEvent dto);
        DescriptiveResponse<bool> WorkOrderAssigned(SignalRWorkOrderEvent dto);
        DescriptiveResponse<bool> WorkOrderDeAssigned(WorkOrderDTO dto);
        DescriptiveResponse<bool> WorkOrderCancelled(SignalRWorkOrderEvent dto);
    }
}
