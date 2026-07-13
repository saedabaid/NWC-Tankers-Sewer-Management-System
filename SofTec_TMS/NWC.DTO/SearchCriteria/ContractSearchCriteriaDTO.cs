using NWC.DTO.Common;

namespace NWC.DTO.SearchCriteria
{
    public class ContractSearchCriteriaDTO
    {
        public Filters<string> FilterModel { get; set; }

        public long? Id { get; set; }

    }
}
