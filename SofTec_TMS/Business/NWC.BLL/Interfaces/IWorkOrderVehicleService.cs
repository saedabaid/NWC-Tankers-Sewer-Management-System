using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.BLL.Interfaces
{
    public interface IWorkOrderVehicleService
    {
        DescriptiveResponse<Boolean> ArriveVehicleToStation(Guid vehicleID, List<int> customerClassesIds); //int customerClassId);

        DescriptiveResponse<SearchResult<StateWorkOrderVehicleDTO>> GetWorkOrderVehicles(WorkOrderVehicleSearchCriteriaDTO searchCriteria);
        DescriptiveResponse<SearchResult<StateVehicleDTO>> GetStateVehicles(StateVehicleSearchCriteriaDTO searchCriteria);
        DescriptiveResponse<SearchResult<StateVehicleDTO>> GetAssignableVehicles(WorkOderFilter filter);
        DescriptiveResponse<PrintDriverInvoice> GetPrintDriverInvoice(PrintDTO PrintDTO);
        DescriptiveResponse<PrintCustomerInvoice> GetPrintCustomerInvoice(PrintDTO PrintDTO);
        DescriptiveResponse<Boolean> OutForParking(Guid vehicleID);
        DescriptiveResponse<PrintVehicleInvoice> GetPrintVehicleInvoice(PrintDTO PrintDTO);
        DescriptiveResponse<SearchResult<OrderReassignmentDTO>> GetOrderReassignmentReport(OrderReassignmentReportSC searchCriteria);
    }
}
