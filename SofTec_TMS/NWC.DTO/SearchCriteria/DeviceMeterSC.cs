using NWC.DTO.Common;
using System;
using System.Collections.Generic;

namespace NWC.DTO.SearchCriteria
{
    public class DeviceMeterSC
    {
        public Filters<string> FilterModel { get; set; }

        public long? Id { get; set; }


        #region Advanced
        public string ConnectorTubeNumber { get; set; }
        public string MeterSerialNumber { get; set; }
        public string ManholeNumber { get; set; }
        public int? ServiceTypeID { get; set; }
        public Guid? StationID { get; set; }

        #endregion

    }
}
