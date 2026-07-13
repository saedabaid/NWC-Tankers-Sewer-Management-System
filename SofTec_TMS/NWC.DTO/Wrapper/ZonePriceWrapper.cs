using NWC.DAL.NWCEntities;
using NWC.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.DTO.Wrapper
{
    public static class ZonePriceWrapper
    {
        public static ZonePriceListDTO WrapToZonePriceDTO(this vw_NWC_ZoneWithNoPrices zonePrice)
        {
            return new ZonePriceListDTO()
            {
                Zone = zonePrice.Zone,
                Station = zonePrice.Station,
                StationID = zonePrice.StationID,
                CustomerClassName = zonePrice.NameAr,
                PriceCharge = zonePrice.PriceCharge,
            };
        }
    }
}
