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
    public interface ISewerService
    {
        DescriptiveResponse<List<long>> SewerVehicleArrivedStation(Guid vehicleID);
        DescriptiveResponse<List<long>> SewerVehicleArrivedStation(WOVArrivedStationDTO dto);
        DescriptiveResponse<Boolean> ArriveSewerVehicleWithOutOrderToStation(Guid vehicleID);
        DescriptiveResponse<Boolean> OutForWork(Guid vehicleID);
    }

}
