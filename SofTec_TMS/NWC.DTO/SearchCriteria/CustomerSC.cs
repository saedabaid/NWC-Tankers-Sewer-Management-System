using NWC.DTO.Common;

namespace NWC.DTO.SearchCriteria
{
    public class CustomerSC
    {
        public PageFilter PageFilter { get; set; }

        public long? Id { get; set; }
        public string IntegartionId { get; set; }

        public string Customer { get; set; }
        public int? ServiceTypeId { get; set; }
        public long? ZoneId { get; set; }


        public bool ExcelFlage { get; set; }

    }
}
