using NWC.DTO.Common;
using System;
using System.Collections.Generic;

namespace NWC.DTO.SearchCriteria
{
    public class DeferredOrderSC
    {
        public PageFilter PageFilter { get; set; }

        public DateTime? DateTimeFrom { get; set; }
        public DateTime? DateTimeTo { get; set; }
        public List<int> StatusIds { get; set; }
        public string OrderNo { get; set; }
        public string MobileNo { get; set; }
        public string PersonIdValue { get; set; }

        public long? Id { get; set; }

        public bool ExcelFlage { get; set; }


    }
}
