using FluentValidation;
using Infrastructure;
using NWC.BLL.Interfaces;
using NWC.DAL.NWCEntities;
using NWC.DTO.Enums;
using NWC.DTO.Models;
using NWC.DTO.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NWC.BLL.Validators
{
    public class ZoneValidator : AbstractValidator<ZoneDTO>
    {
        IRepository<NWC_Zone> _ZoneRepository;
        IRepository<NWC_ContractTariff> _ContractTariffRepository;
        IRepository<NWC_CustomerLocation> _CustomerLocationRepository;
        ILoggedInUserService _loggedInUserService;
        List<ZoneDTO> _entryList;

        public ZoneValidator(ValidationMode mode, ILoggedInUserService loggedInUser,
            IRepository<NWC_Zone> ZoneRepository, IRepository<NWC_ContractTariff> ContractTariffRepository = null,
            IRepository<NWC_CustomerLocation> CustomerLocationRepository = null, List<ZoneDTO> entryList = null)
        {
            this._ZoneRepository = ZoneRepository;
            this._ContractTariffRepository = ContractTariffRepository;
            this._CustomerLocationRepository = CustomerLocationRepository;
            this._loggedInUserService = loggedInUser;
            this._entryList = entryList;

            switch (mode)
            {
                case ValidationMode.Create:
                    Initialize();
                    RuleFor(a => a.AreaID).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseArea);
                    RuleFor(a => a.AllowedTankerTypes.Count).GreaterThan(0).WithMessage(ValidationMessagesKeys.AtLeastOneAllowedTankerType);
                    break;
                case ValidationMode.Update:
                    Initialize();
                    RuleFor(a => a.ID).NotEmpty().WithMessage(ValidationMessagesKeys.IDNotExist); ;
                    RuleFor(a => a.AreaID).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseArea);
                    RuleFor(a => a.AllowedTankerTypes.Count).GreaterThan(0).WithMessage(ValidationMessagesKeys.AtLeastOneAllowedTankerType);
                    break;
                case ValidationMode.Delete:
                    RuleFor(a => HasCustomer(a)).Equal(false).WithMessage(ValidationMessagesKeys.ZoneHasCustomer);
                    RuleFor(a => HasTariff(a)).Equal(false).WithMessage(ValidationMessagesKeys.ZoneHasTariff);
                    break;
                case ValidationMode.CreateLogic2:
                    Initialize();
                    RuleFor(a => a.IntegrationID).NotEmpty().WithMessage(ValidationMessagesKeys.ZoneIntegrationIdRequired);
                    RuleFor(a => a).Must(IsRedundantInEntryList).WithMessage(ValidationMessagesKeys.RedundantOnSheet);

                    break;

            }
        }


        private void Initialize()
        {
            RuleFor(a => a).NotEmpty().WithMessage(ValidationMessagesKeys.zoneModelEmpety);
            RuleFor(a => a.Code).NotEmpty().WithMessage(ValidationMessagesKeys.InsertCode);
            RuleFor(a => a.CityID).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseCity);
            //RuleFor(a => a.AreaID).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseArea);
            RuleFor(a => a.Name).NotEmpty().WithMessage(ValidationMessagesKeys.InsertZoneName);
            RuleFor(a => a.MainStation.ID).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseMainStation);
            RuleFor(a => a.MainStation.Distance).GreaterThan(0).WithMessage(ValidationMessagesKeys.MainStationDistanceshouldBePostiveNum);

            RuleFor(a => a.BackupStations.Where(s => s.ID == null).Count()).Equal(0).WithMessage(ValidationMessagesKeys.ChooseBackupStation);
            RuleFor(a => a.BackupStations.Where(s => s.Distance == null).Count()).Equal(0).WithMessage(ValidationMessagesKeys.EnterBackupStationsDistance);
            RuleFor(a => a.BackupStations.Where(s => s.Distance < 0).Count()).Equal(0).WithMessage(ValidationMessagesKeys.BackupStationDistanceshouldBePostiveNum);
            RuleFor(a => a.BackupStations.Count())
                .Equal(a => a.BackupStations.Distinct().Count()).WithMessage(ValidationMessagesKeys.BackupStationsShouldBeDifferant);

            //RuleFor(a => a.BackupStations.Contains(a.MainStation)).Equal(false).WithMessage(ValidationMessagesKeys.BackupStationsNotEqualMainStation);
            RuleFor(a => a).Must(a => !a.BackupStations.Contains(a.MainStation)).WithMessage(ValidationMessagesKeys.BackupStationsNotEqualMainStation);

            //RuleFor(a => a.AllowedTankerTypes.Count).GreaterThan(0).WithMessage(ValidationMessagesKeys.AtLeastOneAllowedTankerType);
            RuleFor(a => a.Code).Must(IsCodeUnique).WithMessage(ValidationMessagesKeys.CodeUnique);


        }


        private bool IsCodeUnique(ZoneDTO model, string ZoneCode)
        {
            if (model == null || string.IsNullOrEmpty(ZoneCode)) return true;

            var searchCode = ZoneCode.Trim();
            return !_ZoneRepository.GetQuery()
                .Any(s => s.Code == searchCode && s.ID != model.ID && s.IsDeleted == false && s.SubID == _loggedInUserService.LoggedInUser.SubscriberId);
        }

        private bool HasTariff(ZoneDTO model)
        {
            return _ContractTariffRepository.GetQuery().Any(s => s.ZoneID == model.ID);
        }
        private bool HasCustomer(ZoneDTO model)
        {
            return _CustomerLocationRepository.GetQuery().Any(s => s.ZoneID == model.ID);
        }

        private bool IsRedundantInEntryList(ZoneDTO model)
        {
            if (model == null) return true;

            return !this._entryList.Any(a =>
                        a.ExcelSheetRowId != model.ExcelSheetRowId &&
                        a.Name == model.Name &&
                        a.Code == model.Code
                        );
        }

    }
}
