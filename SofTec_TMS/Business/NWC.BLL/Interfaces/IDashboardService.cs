using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;
using System.Collections.Generic;

namespace NWC.BLL.Interfaces
{
    public interface IDashboardService
    {
        DescriptiveResponse<int> GetWorkOrdersCountPerStatus(DashboardSC searchCriteria);
        DescriptiveResponse<List<DashboardXYChartDTO>> GetOrdersCountGroupByDayHours(DashboardSC searchCriteria);
        DescriptiveResponse<List<DashboardXYChartDTO>> GetOrdersCountGroupByTop10Zones(DashboardSC searchCriteria);
        DescriptiveResponse<List<DashboardXYChartDTO>> GetOrdersCountGroupByStatus(DashboardSC searchCriteria);
        DescriptiveResponse<List<DashboardXYChartDTO>> GetOrdersCountGroupByDate(DashboardSC searchCriteria);
        DescriptiveResponse<List<DashboardXYChartDTO>> GetOrdersCountGroupByExecuteTime(DashboardSC searchCriteria);
        DescriptiveResponse<SearchResult<ZonePriceListDTO>> GetAreasWithNoPrices(ZonePriceSCDTO searchCriteria);
        //DashboardSC WrapToVehicleViolationDTO(VehicleDataReportSC dto);

    }
}
