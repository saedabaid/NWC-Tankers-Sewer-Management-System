using NWC.DAL.NWCEntities;
using NWC.DTO.Constants;
using NWC.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NWC.DTO.Wrapper
{
    public static class ZoneWrapper
    {
        public static ZoneListDTO WrapToZoneListDTO(this vw_NWC_ZoneList zone)
        {
            return new ZoneListDTO()
            {
                Id = zone.Id,
                CityID = zone.CityID,
                Code = zone.Code,
                Name = zone.ZoneName,
                IsDeleted = zone.IsDeleted,
                SubID = zone.SubID,
                CityName = zone.CityName,
                RestrictedZoneVehicleTypes = zone.RestrictedZoneVehicleTypes,
                MainStationName = zone.MainStationName ,
                MainStationCode = zone.MainStationCode ,
                MainStationId = zone.MainStationId ,
                CountOfBackupStations = zone.CountOfBackupStations

            };
        }
        public static ZoneDTO WrapToZoneDTO(this NWC_Zone zone)
        {
            if (zone == null)
                return null;

            return new ZoneDTO()
            {
                ID = zone.ID,
                CityID = zone.CityID,
                Code = zone.Code,
                Name = zone.Name,
                IsDeleted = zone.IsDeleted,
                ZoneWithoutTanker = zone.ZoneWithoutTanker

            };
        }
        #region Helper
        private static bool LanguageIsEnglish
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture.Name == LanguagesKeys.English;
            }
        }
        #endregion

    }
}
