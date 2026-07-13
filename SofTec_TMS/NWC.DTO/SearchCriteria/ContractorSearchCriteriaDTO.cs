using Infrastructure;
using NWC.DTO.Common;

namespace NWC.DTO.SearchCriteria
{
    public class ContractorSearchCriteriaDTO
    {
        #region Sort
        public enum OrderByExepression { ContarctorName = 1, Code = 2, CommericalID = 3, MOI = 4, TaxNumber = 5, ContactPerson = 6 }
        public OrderByExepression? OrderBy { get; set; }
        public SortDirection SortDirection { get; set; }
        #endregion

        public Filters<string> FilterModel { get; set; }

        public long? Id { get; set; }


        #region advanced
        public string CommercialIDNumber { get; set; }
        public string TaxNumber { get; set; }
        public string MOI { get; set; }
        public string ContactPersonName { get; set; }

        #endregion

    }
}
