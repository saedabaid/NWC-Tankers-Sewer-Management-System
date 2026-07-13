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
    public interface ISoqyaService
    {
        DescriptiveResponse<SearchResult<SoqyaScheduleDTO>> SearchSoqyaSchedules(SoqyaScheduleSC searchCriteria);
        DescriptiveResponse<bool> AddSoqyeScheduleRecord(SoqyaScheduleDTO dto);
        DescriptiveResponse<bool> EditSoqyeScheduleRecord(SoqyaScheduleDTO dto);
        DescriptiveResponse<bool> DeleteSoqyeScheduleRecord(long scheduleId);
        DescriptiveResponse<List<SoqyaScheduleDTO>> AddRange(List<SoqyaScheduleDTO> SoqyaScheduleDTO);
        DescriptiveResponse<SoqyaBalanceDTO> GetBalanceAndUsed(long customerAccountId, int monthYear, long? excludedScheduleId = null, DateTime? _startDate = null);
        DescriptiveResponse<SearchResult<SoqyaScheduleReportDTO>> GetSoqyaSchedulesReport(SoqyaScheduleReportSC searchCriteria);
        DescriptiveResponse<bool> UpdateGeneratedSoqyaSchedule(List<SoqyaScheduleDTO> dtos);
    }
}
