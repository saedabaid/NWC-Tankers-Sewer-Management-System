using NWC.DAL.NWCEntities;
using NWC.DTO.Common;
using NWC.DTO.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace NWC.DTO.Models
{
    public class ZoneDTO
    {
        #region Properties
        public long ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? ZoneWithoutTanker { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public Guid CityID { get; set; }
        public Guid? AreaID { get; set; }
        public List<Station> BackupStations { get; set; }
        public Station MainStation { get; set; }
        public List<LookUpDTO< Guid>> AllowedTankerTypes { get; set; }
        public List<LookUpDTO< Guid>> RestrictedTankerTypes { get; set; }
        
        public long ExcelSheetRowId { get; set; }
        public string ExcelValidation { get; set; }
        public string CityName { get; set; }
        //public string MainStationName { get; set; }
        //public Guid MainStationID { get; set; }
        //public string BackupStationName { get; set; }
        //public Guid BackupStationID { get; set; }
        public string IntegrationID { get; set; }

        #endregion

        #region Constructors
        public ZoneDTO()
        {

        }

        public ZoneDTO(NWC_Zone zone)
        {
            this.ID = zone.ID;
            this.Code = zone.Code;
            this.Name = zone.Name ;
            //this.NameEn = zone.NameEn;
            this.IsDeleted = zone.IsDeleted;
            this.CreatedBy = zone.CreatedBy;
            this.CreatedDate = zone.CreatedDate;
            this.UpdatedBy = zone.UpdatedBy;
            this.CityID = zone.CityID;
         
        }
        #endregion

        #region Helper
        public static NWC_Zone MapToZone(ZoneDTO dto)
        {
            return new NWC_Zone()
            {
                ID = dto.ID,
                Code = dto.Code,
                Name = dto.Name,
                CityID = dto.CityID,
                ZoneWithoutTanker = dto.ZoneWithoutTanker
            };
        }

        public static NWC_Zone MapToZoneWithStations(ZoneDTO dto)
        {
            var zone = new NWC_Zone()
            {
                ID = dto.ID,
                Code = dto.Code,
                Name = dto.Name,
                CityID = dto.CityID,
                IntegrationId = dto.IntegrationID
            };

            var mainStation = new NWC_ZoneStations
            {
                IsMain = true,
                StationID = dto.MainStation.ID,
                Distance = dto.MainStation.Distance
            };
            zone.NWC_ZoneStations.Add(mainStation);

            if (dto.BackupStations != null && dto.BackupStations.Any())
            {
                foreach (var item in dto.BackupStations)
                {
                    var backupStation = new NWC_ZoneStations
                    {
                        IsMain = false,
                        StationID = item.ID,
                        Distance = item.Distance
                    };
                    zone.NWC_ZoneStations.Add(backupStation);
                }
            }

            return zone;
        }

        #region Helper
        public class Station: IEquatable<Station>
        {
            public Guid ID { set; get; }
            public decimal? Distance { set; get; }
            public string StationName { get; set; }

            public bool Equals(Station other)
            {
                if (other == null) return false;
                return this.ID.Equals(other.ID);
            }
        }

        private static bool LanguageIsEnglish
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture.Name == LanguagesKeys.English;
            }
        }
        #endregion
        #endregion
    }
}
