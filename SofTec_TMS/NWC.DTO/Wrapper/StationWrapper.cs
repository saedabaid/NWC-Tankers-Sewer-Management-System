using NWC.DAL.NWCEntities;
using NWC.DTO.Common;
using NWC.DTO.Constants;
using NWC.DTO.Helpers;
using NWC.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace NWC.DTO.Wrapper
{
    public static class StationWrapper
    {
        private static bool LanguageIsEnglish
        {
            get => Thread.CurrentThread.CurrentCulture.Name == LanguagesKeys.English;
        }

        public static StationNWCSettingsDTO WrapToStationNWCSettingsDTO(this vw_NWC_LandmarkNWCSettings input)
        {
            return new StationNWCSettingsDTO()
            {
                StationId = input.Id,
                StationName = input.Name,
                AreaId = input.AreaId,
                AreaName = input.AreaName,
                CityId = input.CityId,
                CityName = input.CityName,
                StationServiceIds = !string.IsNullOrEmpty(input.StationServices) ? input.StationServices.Split(',').Select(Int32.Parse).ToList() : new List<int>(),
                StationServiceNames = LanguageIsEnglish ? input.EN_StationServiceNames : input.AR_StationServiceNames,
                CustomerClassIds = !string.IsNullOrEmpty(input.CustomerClasses) ? input.CustomerClasses.Split(',').Select(Int32.Parse).ToList() : new List<int>(),
                CustomerClassNames = LanguageIsEnglish ? input.EN_CustomerClassNames : input.AR_CustomerClassNames,
                StatusId = input.StatusId,
                StatusName = LanguageIsEnglish ? input.EN_StatusName : input.AR_StatusName,
                IsVirtual = input.IsVirtual,
                SelectedCapacities = !string.IsNullOrEmpty(input.StationDefaultList) ? input.StationDefaultList.Split(',').Select(x => new LookUpDTO<int>() { Id = Convert.ToInt32(x), Name = x}).ToList() : new List<LookUpDTO<int>>(),
            };
        }

        public static Landmark StationSettingDtoToEntity(this StationNWCSettingsDTO input)
        {
            var entity = new Landmark();
            entity.Id = input.StationId.HasValue ? input.StationId.Value : Guid.NewGuid();
            entity.isDeleted = false;
            entity.CreationTime = DateTimeHelper.GetDateTimeNow();
            return StationSettingDtoToEntity(input, entity);
        }

        public static Landmark StationSettingDtoToEntity(this StationNWCSettingsDTO input, Landmark entity)
        {
            entity.name = input.StationName;
            entity.branchId = input.CityId;
            entity.StatusID = input.StatusId;
            entity.IsVirtualStation = input.IsVirtual;
            return entity;
        }

        public static StationSizesDTO  WrapStationSizes(this vw_NWC_StationSizes input)
        {
            var entity = new StationSizesDTO();
            entity.id = input.id;
            entity.SizesList = input.SizesList;
            entity.dailyquota = input.dailyquota == null ? 0 : (int)input.dailyquota;
            return entity;
        }
    }
}
