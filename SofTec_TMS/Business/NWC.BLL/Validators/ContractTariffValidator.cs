using FluentValidation;
using Infrastructure;
using NWC.DAL.NWCEntities;
using NWC.DTO.Enums;
using NWC.DTO.Helpers;
using NWC.DTO.Models;
using NWC.DTO.Resources;
using System.Collections.Generic;
using System.Linq;

namespace NWC.BLL.Validators
{
    public class ContractTariffValidator: AbstractValidator<ContractTariffDTO>
    {
        IRepository<NWC_ContractTariff> _contractTariffRepository;
        IRepository<NWC_Contract> _contractRepository;
        List<ContractTariffDTO> _entryList;

        public ContractTariffValidator(ValidationMode mode, IRepository<NWC_ContractTariff> contractTariffRepository, 
                                        IRepository<NWC_Contract> contractRepository, List<ContractTariffDTO> entryList = null)
        {
            this._contractTariffRepository = contractTariffRepository;
            this._contractRepository = contractRepository;
            this._entryList = entryList;

            switch (mode)
            {
                case ValidationMode.Create:
                    Initialize();
                    RuleFor(a => a.StationsAddIds).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseStation);
                    //RuleFor(a => a.ZoneAddIds).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseZone);
                    RuleFor(a => a.CustomerLocationClassAddIds).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseCustomerClass);
                    RuleFor(a => a.ServiceTypeAddIds).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseServiceType);
                    break;
                case ValidationMode.Update:
                    Initialize();
                    RuleFor(a => a.ID).NotEmpty();
                    RuleFor(a => a.StationID).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseStation);
                    RuleFor(a => a.ZoneID).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseZone);
                    RuleFor(a => a.CustomerLocationClassID).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseCustomerClass);
                    RuleFor(a => a.ServiceTypeID).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseServiceType);
                    RuleFor(a => a.ID).Must(IsOverLapedPeriod).WithMessage(ValidationMessagesKeys.OverlappedPeriod);
                    break;
                case ValidationMode.CreateLogic2:
                    Initialize();
                    
                    RuleFor(a => a.StationID).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseStation);
                    RuleFor(a => a.CustomerLocationClassID).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseCustomerClass);
                    RuleFor(a => a.ServiceTypeID).NotEmpty().WithMessage(ValidationMessagesKeys.ChooseServiceType);
                    
                    RuleFor(a => a.ZoneID).NotEqual(0).WithMessage(ValidationMessagesKeys.NotValidZone);
                    RuleFor(a => a.TanckerCapacityId).NotEqual(0).WithMessage(ValidationMessagesKeys.NotValidTanckerCapacity);

                    RuleFor(a => a).Must(IsRedundantInEntryList).WithMessage(ValidationMessagesKeys.RedundantOnSheet);
                    RuleFor(a => a.ID).Must(IsOverLapedPeriod).WithMessage(ValidationMessagesKeys.OverlappedPeriodWithDB);
                    break;
            }
        }


        private void Initialize()
        {
            RuleFor(a => a).NotEmpty().WithMessage(ValidationMessagesKeys.EmptyContractTariff);
            RuleFor(a => a.ContractID).NotEmpty().WithMessage(ValidationMessagesKeys.ContractIdIsRequired);
            RuleFor(a => a.CubicMeterCharge).NotNull().WithMessage(ValidationMessagesKeys.CubicMeterChargeIsRequired)
                .GreaterThanOrEqualTo(0).WithMessage(ValidationMessagesKeys.CubicMeterChargeIsRequired);
            RuleFor(a => a.DistanceCharge).NotNull().WithMessage(ValidationMessagesKeys.DistanceChargeIsRequired)
                .GreaterThanOrEqualTo(0).WithMessage(ValidationMessagesKeys.DistanceChargeIsRequired);
            RuleFor(a => a.AfterFirstKM).NotNull().WithMessage(ValidationMessagesKeys.AfterFirstKMIsRequired)
                .GreaterThanOrEqualTo(0).WithMessage(ValidationMessagesKeys.AfterFirstKMIsRequired);

            RuleFor(a => a.DateFromHijri).NotEmpty().WithMessage(ValidationMessagesKeys.InsertStartDate);
            RuleFor(a => a.DateToHijri).NotEmpty().WithMessage(ValidationMessagesKeys.InsertEndDate);
            RuleFor(a => a).Must(s => s.DateFromHijri < s.DateToHijri).WithMessage(ValidationMessagesKeys.StartDateMustBeAfterEndDate);

            //RuleFor(a => a.DateFrom).NotEmpty().WithMessage(ValidationMessagesKeys.InsertStartDate);
            //RuleFor(a => a.DateTo).NotEmpty().WithMessage(ValidationMessagesKeys.InsertEndDate);
            //RuleFor(a => a.DateFrom).LessThan(s => s.DateTo).WithMessage(ValidationMessagesKeys.StartDateMustBeAfterEndDate);


            RuleFor(a => a.ContractID).Must(NotInContractPeriod).WithMessage(ValidationMessagesKeys.TariffNotInContractPeriod);

        }



        private bool NotInContractPeriod(ContractTariffDTO model, long contractId)
        {
            if (model == null || contractId <= 0) return true;

            var contract = _contractRepository.GetQuery().FirstOrDefault(s => s.ID == contractId);

            if (contract != null)
            {
                var contractStartDateHijri = DateTimeHelper.ConvertDateToHijriAsLong(contract.ContractStartDate);
                var contractEndDateHijri = DateTimeHelper.ConvertDateToHijriAsLong(contract.ContractEndDate);

                return (contractStartDateHijri <= model.DateFromHijri) && (contractEndDateHijri >= model.DateToHijri);
            }

            return false;
            //return !_contractRepository.GetQuery().Any(s => s.ID == contractId 
            //                                && (s.ContractStartDate > model.DateFrom || s.ContractEndDate < model.DateTo));
        }

        private bool IsOverLapedPeriod(ContractTariffDTO model, long tariffId)
        {
            if (model == null) return true; // || tariffId <= 0

            return !this._contractTariffRepository.GetQuery().Any(a =>
                        a.ID != tariffId &&
                        !a.IsDeleted &&
                        a.ContractID == model.ContractID &&
                        a.StationID == model.StationID &&
                        (a.ZoneID == null || a.ZoneID == model.ZoneID) &&
                        (a.TanckerCapacityId == null || a.TanckerCapacityId == model.TanckerCapacityId) &&
                        a.CustomerLocationClassID == model.CustomerLocationClassID &&
                        a.ServiceTypeID == model.ServiceTypeID &&
                        (a.DateFromHijri <= model.DateToHijri) && (model.DateFromHijri <= a.DateToHijri));
            
            
            //(a.DateFrom <= model.DateTo) && (model.DateFrom <= a.DateTo));
        }


        private bool IsRedundantInEntryList(ContractTariffDTO model)
        {
            if (model == null) return true;

            return !this._entryList.Any(a =>
                        //a.ID != tariffId &&
                        a.ExcelSheetRowId != model.ExcelSheetRowId &&
                        //!a.IsDeleted &&
                        a.ContractID == model.ContractID &&
                        a.StationID == model.StationID &&
                        (a.ZoneID == null || model.ZoneID == null || a.ZoneID == model.ZoneID) &&
                        (a.TanckerCapacityId == null || model.TanckerCapacityId == null || a.TanckerCapacityId == model.TanckerCapacityId) &&
                        a.CustomerLocationClassID == model.CustomerLocationClassID &&
                        a.ServiceTypeID == model.ServiceTypeID &&
                        (a.DateFromHijri <= model.DateToHijri) && (model.DateFromHijri <= a.DateToHijri));
        }

    }
}
