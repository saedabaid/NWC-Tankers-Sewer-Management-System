using NWC.DTO.Common;
using NWC.DTO.Models;
using NWC.DTO.SearchCriteria;

namespace NWC.BLL.Interfaces
{
    public interface IDeviceMeterService
    {
        DescriptiveResponse<SearchResult<DeviceMeterDTO>> SearchDeviceMeter(DeviceMeterSC searchCriteria);
        DescriptiveResponse<SearchResult<MeterReadingDTO>> SearchReadingList(MeterReadingSC searchCriteria);

        DescriptiveResponse<long?> AddDeviceMeter(DeviceMeterDTO dto);
        DescriptiveResponse<long?> AddReading(MeterReadingDTO dto);
        DescriptiveResponse<bool> DeleteReading(long readingId);

    }
}
