using NWC.DTO.Common;
using System;
using System.Collections.Generic;

namespace NWC.DTO.SearchCriteria
{
    public class MeterReadingSC
    {
        public Filters<string> FilterModel { get; set; }

        public long? Id { get; set; }


        #region Advanced
        public List<long> DeviceMeterIDs { get; set; }
        public DateTime? DateTimeFrom { get; set; }
        public DateTime? DateTimeTo { get; set; }

        #endregion

    }
}
