using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using System.Collections.Generic;

namespace NWC.BLL.Interfaces
{
    public interface IReportService
    {
        DescriptiveResponse<SearchResult<Report_OrderPerZone>> GetOrdersPerZone(ReportSC searchCriteria);
        DescriptiveResponse<SearchResult<Report_OrderPerZone>> GetStationOrderCapacity(ReportSC searchCriteria);
        DescriptiveResponse<SearchResult<Report_OrderPerZone>> GetStationServiceTime(ReportSC searchCriteria);
        DescriptiveResponse<SearchResult<Report_TankersPermissionsStatus>> GetTankerPermissionStatus(ReportTankersPermissionsStatusSC searchCriteria);
        DescriptiveResponse<IEnumerable<Report_SoqyaScheduledPerDay>> GetSoqyaSchedulePerDay(ReportScheduledPerDaySC searchCriteria);

        DescriptiveResponse<SearchResult<ContractTariffDTO>> ContractTariffReport(ContractTariffSc searchCriteria);
    }
}
